using Microsoft.Data.SqlClient;
using Polly;
using Polly.Retry;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TryIT.SqlAdo.MicrosoftSqlClient.Models;

namespace TryIT.SqlAdo.MicrosoftSqlClient.Core
{
    /// <summary>
    /// Core SQL command executor that handles connection management, retries, and logging.
    /// This is shared between standalone calls and transactional scopes.
    /// </summary>
    internal class SqlCommandExecutor
    {
        private const string EXCEPTION_DATA_RETRY_ATTEMPTS = "RetryAttempts";

        private readonly ConnectorConfig _config;
        private readonly ResiliencePipeline _pipeline;
        private readonly List<RetryResult> _retryResults;
        private readonly DbLogDelegate _dbLogDelegate;
        private readonly string _dataSource;
        private readonly string _database;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlCommandExecutor"/> class.
        /// </summary>
        public SqlCommandExecutor(
            ConnectorConfig config,
            ResiliencePipeline pipeline,
            List<RetryResult> retryResults,
            DbLogDelegate dbLogDelegate)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));
            _retryResults = retryResults ?? throw new ArgumentNullException(nameof(retryResults));
            _dbLogDelegate = dbLogDelegate;

            var builder = new SqlConnectionStringBuilder(config.ConnectionString);
            _dataSource = builder.DataSource;
            _database = builder.InitialCatalog;
        }

        /// <summary>
        /// Gets the timeout in seconds configured for commands.
        /// </summary>
        public int TimeoutSecond => _config.TimeoutSecond;

        /// <summary>
        /// Opens a new SqlConnection synchronously.
        /// </summary>
        internal SqlConnection OpenConnection()
        {
            return OpenConnectionAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Opens a new SqlConnection asynchronously.
        /// </summary>
        internal async Task<SqlConnection> OpenConnectionAsync(CancellationToken cancellationToken = default)
        {
            var conn = new SqlConnection(_config.ConnectionString);
            if (!string.IsNullOrEmpty(_config.AccessToken))
            {
                conn.AccessToken = _config.AccessToken;
            }
            await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
            return conn;
        }

        /// <summary>
        /// Clones an array of SqlParameter to avoid "already contained in another collection" errors on retry.
        /// </summary>
        internal static SqlParameter[] CloneParameters(SqlParameter[] original)
        {
            if (original == null) return null;
            var cloned = new SqlParameter[original.Length];
            for (int i = 0; i < original.Length; i++)
                cloned[i] = (SqlParameter)((ICloneable)original[i]).Clone();
            return cloned;
        }

        /// <summary>
        /// Executes a command with retry and logging (standalone calls).
        /// </summary>
        internal async Task<TResult> ExecuteWithRetryAndLogAsync<TResult>(
            string sql,
            CommandType commandType,
            Func<SqlConnection, SqlTransaction, CancellationToken, Task<TResult>> executionFunc,
            SqlParameter[] parameters,
            bool inTransaction,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(sql))
                throw new ArgumentNullException(nameof(sql));

            var context = CorrelationContextAccessor.Current;
            var startTime = DateTimeOffset.UtcNow;
            string traceId = GetTraceId();
            string correlationId = context?.CorrelationId ?? Guid.NewGuid().ToString();
            Exception exception = null;
            TResult result = default;

            // Fire-and-forget BEFORE log
            _ = Task.Run(async () =>
            {
                try
                {
                    var logContext = new DbLogContext
                    {
                        TraceId = traceId,
                        Stage = LogStage.BeforeExecute,
                        Provider = "SqlServer",
                        Database = _database,
                        DataSource = _dataSource,
                        CommandText = SanitizeSql(sql),
                        CommandType = commandType,
                        Parameters = BuildParameters(parameters),
                        StartTimeUtc = startTime,
                        CorrelationId = correlationId,
                        CorrelationExtra = context?.CorrelationExtra,
                        InTransaction = inTransaction
                    };
                    await SafeLogAsync(logContext).ConfigureAwait(false);
                }
                catch { /* never throw from logging */ }
            });

            try
            {
                // Use the retry pipeline
                result = await _pipeline.ExecuteAsync(async (ctx) =>
                {
                    using (var connection = await OpenConnectionAsync(cancellationToken).ConfigureAwait(false))
                    {
                        // For standalone calls we don't have a transaction, so pass null.
                        // But the delegate expects a transaction – we pass null.
                        return await executionFunc(connection, null, cancellationToken).ConfigureAwait(false);
                    }
                }, cancellationToken).ConfigureAwait(false);

                return result;
            }
            catch (Exception ex)
            {
                exception = ex;
                AddExceptionData(ex);
                throw;
            }
            finally
            {
                var endTime = DateTimeOffset.UtcNow;
                var durationMs = (long)(endTime - startTime).TotalMilliseconds;

                _ = Task.Run(async () =>
                {
                    try
                    {
                        var logContext = new DbLogContext
                        {
                            TraceId = traceId,
                            Stage = exception == null ? LogStage.AfterExecute : LogStage.OnError,
                            Provider = "SqlServer",
                            Database = _database,
                            DataSource = _dataSource,
                            CommandText = SanitizeSql(sql),
                            CommandType = commandType,
                            Parameters = BuildParameters(parameters),
                            RowsAffected = TryGetResultCountSafe(result),
                            DurationMs = durationMs,
                            StartTimeUtc = startTime,
                            EndTimeUtc = endTime,
                            Exception = exception,
                            CorrelationId = correlationId,
                            CorrelationExtra = context?.CorrelationExtra,
                            InTransaction = inTransaction
                        };
                        await SafeLogAsync(logContext).ConfigureAwait(false);
                    }
                    catch { /* never throw from logging */ }
                });
            }
        }

        /// <summary>
        /// Executes a command with logging but without retry, using an existing open connection and transaction.
        /// Used by DbTransactionScope.
        /// </summary>
        internal async Task<TResult> ExecuteWithLogAsync<TResult>(
            string sql,
            CommandType commandType,
            SqlConnection connection,
            SqlTransaction transaction,
            Func<SqlCommand, CancellationToken, Task<TResult>> executor,
            SqlParameter[] parameters,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(sql))
                throw new ArgumentNullException(nameof(sql));
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            var context = CorrelationContextAccessor.Current;
            var startTime = DateTimeOffset.UtcNow;
            string traceId = GetTraceId();
            string correlationId = context?.CorrelationId ?? Guid.NewGuid().ToString();
            Exception exception = null;
            TResult result = default;

            // Fire-and-forget BEFORE log
            _ = Task.Run(async () =>
            {
                try
                {
                    var logContext = new DbLogContext
                    {
                        TraceId = traceId,
                        Stage = LogStage.BeforeExecute,
                        Provider = "SqlServer",
                        Database = _database,
                        DataSource = _dataSource,
                        CommandText = SanitizeSql(sql),
                        CommandType = commandType,
                        Parameters = BuildParameters(parameters),
                        StartTimeUtc = startTime,
                        CorrelationId = correlationId,
                        CorrelationExtra = context?.CorrelationExtra,
                        InTransaction = true // always true because this is called from a transaction scope
                    };
                    await SafeLogAsync(logContext).ConfigureAwait(false);
                }
                catch { /* never throw from logging */ }
            });

            try
            {
                using (var cmd = connection.CreateCommand())
                {
                    cmd.Transaction = transaction;
                    cmd.CommandTimeout = _config.TimeoutSecond;
                    cmd.CommandText = sql;
                    cmd.CommandType = commandType;
                    if (parameters != null && parameters.Length > 0)
                        cmd.Parameters.AddRange(CloneParameters(parameters));

                    result = await executor(cmd, cancellationToken).ConfigureAwait(false);
                }
                return result;
            }
            catch (Exception ex)
            {
                exception = ex;
                AddExceptionData(ex);
                throw;
            }
            finally
            {
                var endTime = DateTimeOffset.UtcNow;
                var durationMs = (long)(endTime - startTime).TotalMilliseconds;

                _ = Task.Run(async () =>
                {
                    try
                    {
                        var logContext = new DbLogContext
                        {
                            TraceId = traceId,
                            Stage = exception == null ? LogStage.AfterExecute : LogStage.OnError,
                            Provider = "SqlServer",
                            Database = _database,
                            DataSource = _dataSource,
                            CommandText = SanitizeSql(sql),
                            CommandType = commandType,
                            Parameters = BuildParameters(parameters),
                            RowsAffected = TryGetResultCountSafe(result),
                            DurationMs = durationMs,
                            StartTimeUtc = startTime,
                            EndTimeUtc = endTime,
                            Exception = exception,
                            CorrelationId = correlationId,
                            CorrelationExtra = context?.CorrelationExtra,
                            InTransaction = true
                        };
                        await SafeLogAsync(logContext).ConfigureAwait(false);
                    }
                    catch { /* never throw from logging */ }
                });
            }
        }

        #region Private Helpers

        private void AddExceptionData(Exception ex)
        {
            if (_retryResults.Any())
                ex.Data[EXCEPTION_DATA_RETRY_ATTEMPTS] = _retryResults.ToList();
        }

        private static string GetTraceId()
        {
            return System.Diagnostics.Activity.Current?.Id ?? Guid.NewGuid().ToString();
        }

        private static string SanitizeSql(string sql) => sql; // override if needed

        private static IDictionary<string, object> BuildParameters(SqlParameter[] parameters)
        {
            if (parameters == null) return null;
            return parameters.ToDictionary(
                p => p.ParameterName,
                p => IsSensitive(p.ParameterName) ? "***" : p.Value
            );
        }

        private static bool IsSensitive(string name)
        {
            if (string.IsNullOrEmpty(name)) return false;
            name = name.ToLowerInvariant();
            return name.Contains("password") || name.Contains("token") || name.Contains("secret");
        }

        private static int? TryGetResultCountSafe<TResult>(TResult result)
        {
            switch (result)
            {
                case DataTable dt: return dt.Rows.Count;
                case DataSet ds: return ds.Tables.Count > 0 ? ds.Tables[0].Rows.Count : 0;
                case IList list: return list.Count;
                case int i: return i;
                default: return null;
            }
        }

        private async Task SafeLogAsync(DbLogContext context)
        {
            if (_dbLogDelegate == null) return;
            try
            {
                await _dbLogDelegate.Invoke(context).ConfigureAwait(false);
            }
            catch { /* never throw from logging */ }
        }

        #endregion
    }
}