using Azure.Core;
using Npgsql;

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

        private readonly NpgsqlDataSource dataSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlDbClient"/> class
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="credential"></param>
        public PostgreSqlDbClient(string connectionString, TokenCredential credential)
        {
            if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentNullException(nameof(connectionString));
            if (credential == null) throw new ArgumentNullException(nameof(credential));

            dataSource = new NpgsqlDataSourceBuilder(connectionString)
                .UsePeriodicPasswordProvider(async (builder, cancellationToken) =>
                {
                    var token = await credential.GetTokenAsync(
                        new TokenRequestContext(_scopes),
                        cancellationToken);
                    return token.Token;
                }, TimeSpan.FromMinutes(25), TimeSpan.FromSeconds(30))
                .Build();
        }

        private async Task<TResult> ExecuteAsync<TResult>(
            string sql,
            Func<NpgsqlCommand, CancellationToken, Task<TResult>> executor,
            NpgsqlParameter[]? parameters = null,
            CancellationToken cancellationToken = default)
        {
            await using var cmd = dataSource.CreateCommand(sql);

            if (parameters is { Length: > 0 })
                cmd.Parameters.AddRange(parameters);

            return await executor(cmd, cancellationToken);
        }

        /// <summary>
        /// Executes a non-query SQL command asynchronously.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<int> ExecuteNonQueryAsync(
            string sql,
            NpgsqlParameter[]? parameters = null,
            CancellationToken cancellationToken = default)
            => ExecuteAsync(sql, (cmd, token) => cmd.ExecuteNonQueryAsync(token), parameters, cancellationToken);

        /// <summary>
        /// Executes a SQL command that returns a single value asynchronously.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<object?> ExecuteScalarAsync(
            string sql,
            NpgsqlParameter[]? parameters = null,
            CancellationToken cancellationToken = default)
            => ExecuteAsync(sql, (cmd, token) => cmd.ExecuteScalarAsync(token), parameters, cancellationToken);

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
            var cmd = dataSource.CreateCommand(sql);

            if (parameters is { Length: > 0 })
                cmd.Parameters.AddRange(parameters);

            // CommandBehavior.CloseConnection ensures conn closes when reader is disposed
            return await cmd.ExecuteReaderAsync(System.Data.CommandBehavior.CloseConnection, cancellationToken);
        }
    }
}
