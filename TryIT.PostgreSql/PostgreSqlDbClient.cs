using Azure.Core;
using Npgsql;
using Polly;
using Polly.Retry;
using TryIT.PostgreSql.Core;
using TryIT.PostgreSql.Helper;
using TryIT.PostgreSql.Models;

namespace TryIT.PostgreSql
{
    /// <summary>
    /// Helper class for PostgreSQL database operations
    /// </summary>
    public class PostgreSqlDbClient
    {
        private static readonly string[] _scopes =
        {
            "https://ossrdbms-aad.database.windows.net/.default"
        };

        private readonly List<RetryResult> _retryResults = new List<RetryResult>();
        private readonly NpgsqlDataSource _dataSource;

        private readonly CommandExecutor _executor;

        /// <summary>
        /// Retry results from the operation, including attempt number, timestamp, and exception details.
        /// </summary>
        public List<RetryResult> RetryResults => _retryResults;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlDbClient"/> class
        /// </summary>
        /// <param name="config"></param>
        public PostgreSqlDbClient(ConnectorConfig config)
        {
            if (string.IsNullOrWhiteSpace(config.ConnectionString)) throw new ArgumentNullException(nameof(config), $"{nameof(config.ConnectionString)} is mandatory");
            if (config.TokenCredential == null) throw new ArgumentNullException(nameof(config), $"{nameof(config.TokenCredential)} is mandatory");

            _dataSource = new NpgsqlDataSourceBuilder(config.ConnectionString)
                .UsePeriodicPasswordProvider(async (builder, cancellationToken) =>
                {
                    var token = await config.TokenCredential.GetTokenAsync(
                        new TokenRequestContext(_scopes),
                        cancellationToken);
                    return token.Token;
                }, TimeSpan.FromMinutes(25), TimeSpan.FromSeconds(30))
                .Build();

            var pipeline = BuildResiliencePipeline(config.RetryProperty);
            _executor = new CommandExecutor(config, pipeline, _retryResults, config.DbLogDelegate);
        }

        private ResiliencePipeline BuildResiliencePipeline(RetryProperty? retryProperty)
        {
            if (retryProperty == null 
                || retryProperty.RetryExceptions == null 
                || !retryProperty.RetryExceptions.Any())
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

        /// <summary>
        /// Executes a SQL command that returns the number of rows affected
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<int> ExecuteNonQueryAsync(
            string sql,
            NpgsqlParameter[]? parameters = null,
            CancellationToken cancellationToken = default)
        {
            return await _executor.ExecuteWithRetryAndLogAsync(
                sql,
                _dataSource,
                async (dataSource, trans, token) =>
                {
                    await using var cmd = dataSource.CreateCommand(sql);

                    if (parameters is { Length: > 0 })
                        cmd.Parameters.AddRange(parameters);

                    return await cmd.ExecuteNonQueryAsync(token);
                },
                parameters,
                false,
                cancellationToken
            ).ConfigureAwait(false);
        }

        /// <summary>
        /// Executes a SQL command that returns a single value asynchronously.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<T> ExecuteScalarAsync<T>(
            string sql,
            NpgsqlParameter[]? parameters = null,
            CancellationToken cancellationToken = default)
        {
            object? result = await _executor.ExecuteWithRetryAndLogAsync(
                sql,
                _dataSource,
                async (dataSource, trans, token) =>
                {
                    await using var cmd = dataSource.CreateCommand(sql);

                    if (parameters is { Length: > 0 })
                        cmd.Parameters.AddRange(parameters);

                    return await cmd.ExecuteScalarAsync(token);
                },
                parameters,
                false,
                cancellationToken
            ).ConfigureAwait(false);

            return SqlHelper.ConvertValue<T>(result);
        }


        /// <summary>
        /// Executes a SQL command that returns a data reader asynchronously.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<NpgsqlDataReader> ExecuteReaderAsync(
            string sql,
            NpgsqlParameter[]? parameters = null,
            CancellationToken cancellationToken = default)
        {
            var cmd = _dataSource.CreateCommand(sql);

            if (parameters is { Length: > 0 })
                cmd.Parameters.AddRange(parameters);

            // CommandBehavior.CloseConnection ensures conn closes when reader is disposed
            return await cmd.ExecuteReaderAsync(System.Data.CommandBehavior.CloseConnection, cancellationToken);
        }        
    }
}
