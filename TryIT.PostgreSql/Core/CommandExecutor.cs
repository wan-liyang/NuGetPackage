using Npgsql;
using Polly;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TryIT.PostgreSql.Models;

namespace TryIT.PostgreSql.Core
{
    internal class CommandExecutor
    {
        private const string EXCEPTION_DATA_RETRY_ATTEMPTS = "RetryAttempts";

        private readonly ConnectorConfig _config;
        private readonly ResiliencePipeline _pipeline;
        private readonly List<RetryResult> _retryResults;
        private readonly DbLogDelegate? _dbLogDelegate;
        private readonly string? _dataSource;
        private readonly string? _database;

        public CommandExecutor(ConnectorConfig config, 
            ResiliencePipeline pipeline,
            List<RetryResult> retryResults,
            DbLogDelegate? dbLogDelegate)
        {
            _config = config;
            _pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));
            _retryResults = retryResults ?? throw new ArgumentNullException(nameof(retryResults));
            _dbLogDelegate = dbLogDelegate;

            var builder = new NpgsqlConnectionStringBuilder(config.ConnectionString);
            _dataSource = builder.Host;
            _database = builder.Database;
        }


        /// <summary>
        /// Executes a command with retry and logging (standalone calls).
        /// </summary>
        internal async Task<TResult> ExecuteWithRetryAndLogAsync<TResult>(
            string sql,
            NpgsqlDataSource dataSource,
            Func<NpgsqlDataSource, NpgsqlTransaction, CancellationToken, Task<TResult>> executionFunc,
            NpgsqlParameter[]? parameters,
            bool inTransaction,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(sql))
                throw new ArgumentNullException(nameof(sql));

            var context = CorrelationContextAccessor.Current;
            var startTime = DateTimeOffset.UtcNow;
            string traceId = GetTraceId();
            string correlationId = context?.CorrelationId ?? Guid.NewGuid().ToString();
            Exception? exception = null;
            TResult? result = default;

            // Fire-and-forget BEFORE log
            _ = Task.Run(async () =>
            {
                try
                {
                    var logContext = new DbLogContext
                    {
                        TraceId = traceId,
                        Stage = LogStage.BeforeExecute,
                        Provider = "PostgreSql",
                        Database = _database,
                        DataSource = _dataSource,
                        CommandText = SanitizeSql(sql),
                        //CommandType = "sql",
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
                    return await executionFunc(dataSource, null, cancellationToken).ConfigureAwait(false);

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
                            Provider = "PostgreSql",
                            Database = _database,
                            DataSource = _dataSource,
                            CommandText = SanitizeSql(sql),
                            //CommandType = commandType,
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

        private static IDictionary<string, object> BuildParameters(NpgsqlParameter[] parameters)
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
    }
}
