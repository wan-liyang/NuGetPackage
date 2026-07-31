using Microsoft.Data.SqlClient;
using Microsoft.Data.SqlClient.AlwaysEncrypted.AzureKeyVaultProvider;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TryIT.SqlAdo.MicrosoftSqlClient.BulkOperations;
using TryIT.SqlAdo.MicrosoftSqlClient.Core;
using TryIT.SqlAdo.MicrosoftSqlClient.CopyMode;
using TryIT.SqlAdo.MicrosoftSqlClient.Helper;
using TryIT.SqlAdo.MicrosoftSqlClient.Models;

namespace TryIT.SqlAdo.MicrosoftSqlClient
{
    /// <summary>
    /// Database connector facade that provides simplified data access methods.
    /// </summary>
    public class DbConnector
    {
        private readonly ConnectorConfig _config;
        private readonly SqlCommandExecutor _executor;
        private readonly DbInfoService _dbInfoService;
        private readonly SqlBulkCopyService _bulkCopyService;
        private readonly List<RetryResult> _retryResults = new List<RetryResult>();


        /// <summary>
        /// Retry results collected during execution.
        /// </summary>
        public List<RetryResult> RetryResults => _retryResults;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbConnector"/> class.
        /// </summary>
        public DbConnector(ConnectorConfig config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));
            if (string.IsNullOrEmpty(config.ConnectionString))
                throw new ArgumentException($"{nameof(config.ConnectionString)} is null or empty");

            _config = config;

            var pipeline = BuildResiliencePipeline(config.RetryProperty);
            _executor = new SqlCommandExecutor(config, pipeline, _retryResults, config.DbLogDelegate);

            _dbInfoService = new DbInfoService(_executor, _config.TimeoutSecond);
            _bulkCopyService = new SqlBulkCopyService(config, _executor);
        }

        public List<AlwaysEncryptedColumn> GetAlwaysEncryptedColumns(string tableName) => _dbInfoService.GetAlwaysEncryptedColumns(tableName);

        /// <summary>
        /// Registers Azure Key Vault column encryption key store provider.
        /// <para>IMPORTANT: call this once during program startup.</para>
        /// </summary>
        public static void RegisterColumnEncryptionKeyStore_AKV(AzureServicePrincipal azureKeyVaultProvider)
        {
            var credential = AzureHelper.GetClientSecretCredential(azureKeyVaultProvider);
            var akvProvider = new SqlColumnEncryptionAzureKeyVaultProvider(credential);
            SqlConnection.RegisterColumnEncryptionKeyStoreProviders(
                new Dictionary<string, SqlColumnEncryptionKeyStoreProvider>(StringComparer.OrdinalIgnoreCase)
                {
                    { SqlColumnEncryptionAzureKeyVaultProvider.ProviderName, akvProvider }
                });
        }

        // ---------------------- Fetch DataTable ----------------------

        public DataTable FetchDataTable(string sql, CommandType commandType = CommandType.Text, SqlParameter[] parameters = null)
            => FetchDataTableAsync(sql, commandType, parameters).GetAwaiter().GetResult();

        public async Task<DataTable> FetchDataTableAsync(
            string sql,
            CommandType commandType,
            SqlParameter[] parameters = null,
            CancellationToken cancellationToken = default)
        {
            return await _executor.ExecuteWithRetryAndLogAsync(
                sql,
                commandType,
                async (conn, trans, token) =>
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandTimeout = _config.TimeoutSecond;
                        cmd.CommandText = sql;
                        cmd.CommandType = commandType;
                        if (parameters?.Length > 0)
                            cmd.Parameters.AddRange(SqlCommandExecutor.CloneParameters(parameters));

                        using (var adapter = new SqlDataAdapter(cmd))
                        {
                            var dt = new DataTable();
                            await Task.Run(() => adapter.Fill(dt), token).ConfigureAwait(false);
                            return dt;
                        }
                    }
                },
                parameters,
                false,
                cancellationToken
            ).ConfigureAwait(false);
        }

        // ---------------------- Fetch DataSet ----------------------

        public DataSet FetchDataSet(string sql, CommandType commandType, params SqlParameter[] parameters)
            => FetchDataSetAsync(sql, commandType, parameters).GetAwaiter().GetResult();

        public async Task<DataSet> FetchDataSetAsync(
            string sql,
            CommandType commandType,
            SqlParameter[] parameters = null,
            CancellationToken cancellationToken = default)
        {
            return await _executor.ExecuteWithRetryAndLogAsync(
                sql,
                commandType,
                async (conn, trans, token) =>
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandTimeout = _config.TimeoutSecond;
                        cmd.CommandText = sql;
                        cmd.CommandType = commandType;
                        if (parameters?.Length > 0)
                            cmd.Parameters.AddRange(SqlCommandExecutor.CloneParameters(parameters));

                        using (var adapter = new SqlDataAdapter(cmd))
                        {
                            var ds = new DataSet();
                            await Task.Run(() => adapter.Fill(ds), token).ConfigureAwait(false);
                            return ds;
                        }
                    }
                },
                parameters,
                false,
                cancellationToken
            ).ConfigureAwait(false);
        }

        // ---------------------- ExecuteNonQuery ----------------------

        public int ExecuteNonQuery(string sql, CommandType commandType, params SqlParameter[] parameters)
            => ExecuteNonQueryAsync(sql, commandType, parameters).GetAwaiter().GetResult();

        public async Task<int> ExecuteNonQueryAsync(
            string sql,
            CommandType commandType,
            SqlParameter[] parameters = null,
            CancellationToken cancellationToken = default)
        {
            return await _executor.ExecuteWithRetryAndLogAsync(
                sql,
                commandType,
                async (conn, trans, token) =>
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandTimeout = _config.TimeoutSecond;
                        cmd.CommandText = sql;
                        cmd.CommandType = commandType;
                        if (parameters?.Length > 0)
                            cmd.Parameters.AddRange(SqlCommandExecutor.CloneParameters(parameters));
                        return await cmd.ExecuteNonQueryAsync(token).ConfigureAwait(false);
                    }
                },
                parameters,
                false,
                cancellationToken
            ).ConfigureAwait(false);
        }

        // ---------------------- ExecuteScalar ----------------------

        public T ExecuteScalar<T>(string sql, CommandType commandType, params SqlParameter[] parameters)
            => ExecuteScalarAsync<T>(sql, commandType, parameters).GetAwaiter().GetResult();

        public async Task<T> ExecuteScalarAsync<T>(
            string sql,
            CommandType commandType,
            SqlParameter[] parameters = null,
            CancellationToken cancellationToken = default)
        {
            object result = await _executor.ExecuteWithRetryAndLogAsync(
                sql,
                commandType,
                async (conn, trans, token) =>
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandTimeout = _config.TimeoutSecond;
                        cmd.CommandText = sql;
                        cmd.CommandType = commandType;
                        if (parameters?.Length > 0)
                            cmd.Parameters.AddRange(SqlCommandExecutor.CloneParameters(parameters));
                        return await cmd.ExecuteScalarAsync(token).ConfigureAwait(false);
                    }
                },
                parameters,
                false,
                cancellationToken
            ).ConfigureAwait(false);
            return SqlHelper.ConvertValue<T>(result);
        }

        // ---------------------- Scalar Function ----------------------

        public T FetchScalarFunction<T>(string function, params SqlParameter[] parameters)
        {
            if (string.IsNullOrEmpty(function))
                throw new ArgumentNullException(nameof(function));
            if (function.Split('.').Length != 2)
                throw new InvalidOperationException($"Function '{function}' must contain schema and function name, e.g. schema.function");

            string sql = new StringBuilder("SELECT ")
                .Append(function)
                .Append("(")
                .Append(parameters?.Length > 0 ? string.Join(",", parameters.Select(p => p.ParameterName)) : "")
                .Append(")")
                .ToString();

            return ExecuteScalar<T>(sql, CommandType.Text, parameters);
        }

        // ---------------------- Table-Valued Function ----------------------

        public DataTable FetchDataTableFunction(string function, params SqlParameter[] parameters)
        {
            if (string.IsNullOrEmpty(function))
                throw new ArgumentNullException(nameof(function));
            if (function.Split('.').Length != 2)
                throw new InvalidOperationException($"Function '{function}' must contain schema and function name, e.g. schema.function");

            string sql = new StringBuilder("SELECT * FROM ")
                .Append(function)
                .Append("(")
                .Append(parameters?.Length > 0 ? string.Join(",", parameters.Select(p => p.ParameterName)) : "")
                .Append(")")
                .ToString();

            return FetchDataTable(sql, CommandType.Text, parameters);
        }

        // ---------------------- ExecuteReader ----------------------

        /// <summary>
        /// Executes the command and returns a SqlDataReader. The connection is closed when the reader is disposed.
        /// NOTE: This method does NOT use retry or logging (kept for backward compatibility).
        /// </summary>
        public SqlDataReader ExecuteReader(string commandText, params SqlParameter[] parameters)
        {
            var connection = _executor.OpenConnection();
            var cmd = new SqlCommand(commandText, connection);
            cmd.CommandTimeout = _config.TimeoutSecond;
            cmd.CommandType = CommandType.Text;
            if (parameters?.Length > 0)
                cmd.Parameters.AddRange(SqlCommandExecutor.CloneParameters(parameters));
            return cmd.ExecuteReader(CommandBehavior.CloseConnection);
        }

        // ---------------------- Bulk Copy ----------------------

        public void CopyData(List<ICopyMode> copyModes) => _bulkCopyService.CopyData(copyModes);
        public void CopyData(ICopyMode copyMode) => _bulkCopyService.CopyData(new List<ICopyMode> { copyMode });

        // ---------------------- Transaction ----------------------

        /// <summary>
        /// Begins a new transaction scope. All commands executed within this scope will share the same transaction.
        /// </summary>
        public DbTransactionScope BeginTransaction()
        {
            return new DbTransactionScope(_executor);
        }

        // ---------------------- Private Helpers ----------------------

        private ResiliencePipeline BuildResiliencePipeline(RetryProperty retryProperty)
        {
            if (retryProperty == null || retryProperty.RetryExceptions == null || !retryProperty.RetryExceptions.Any())
                return new ResiliencePipelineBuilder().Build();

            var builder = new PredicateBuilder();
            builder.Handle<Exception>(ex =>
                retryProperty.RetryExceptions.Any(retryEx =>
                    retryEx.ExceptionType.IsInstanceOfType(ex) &&
                    (string.IsNullOrEmpty(retryEx.MessageKeyword) ||
                     ex.Message.ToUpperInvariant().Contains(retryEx.MessageKeyword.ToUpperInvariant()))
                )
            );

            return new ResiliencePipelineBuilder()
                .AddRetry(new RetryStrategyOptions
                {
                    ShouldHandle = builder,
                    Delay = retryProperty.RetryDelay,
                    MaxRetryAttempts = retryProperty.RetryCount,
                    BackoffType = DelayBackoffType.Constant,
                    OnRetry = args =>
                    {
                        _retryResults.Add(new RetryResult
                        {
                            AttemptNumber = args.AttemptNumber,
                            Timestamp = DateTime.Now,
                            Exception = args.Outcome.Exception
                        });
                        return default;
                    }
                })
                .Build();
        }
    }
}