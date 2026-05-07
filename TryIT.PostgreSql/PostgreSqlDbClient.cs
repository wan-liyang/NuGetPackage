using Azure.Core;
using Npgsql;

namespace TryIT.PostgreSql
{
    /// <summary>
    /// Helper class for PostgreSQL database operations
    /// </summary>
    public class PostgreSqlDbClient
    {
        private readonly string _connStr;
        private readonly TokenCredential _credential;
        private static readonly string[] _scopes =
        {
            "https://ossrdbms-aad.database.windows.net/.default"
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlDbClient"/> class
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="credential"></param>
        public PostgreSqlDbClient(string connectionString, TokenCredential credential)
        {
            _connStr = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _credential = credential ?? throw new ArgumentNullException(nameof(credential));
        }

        private async Task<NpgsqlConnection> CreateConnectionAsync(CancellationToken cancellationToken)
        {
            var token = await _credential.GetTokenAsync(
                new TokenRequestContext(_scopes),
                cancellationToken);

            // IMPORTANT: inject token via connection string BEFORE open
            var builder = new NpgsqlConnectionStringBuilder(_connStr)
            {
                Password = token.Token
            };

            return new NpgsqlConnection(builder.ConnectionString);
        }

        private async Task<TResult> ExecuteAsync<TResult>(
            string sql,
            Func<NpgsqlCommand, CancellationToken, Task<TResult>> executor,
            NpgsqlParameter[]? parameters = null,
            CancellationToken cancellationToken = default)
        {
            await using var conn = await CreateConnectionAsync(cancellationToken);
            await using var cmd = new NpgsqlCommand(sql, conn);

            if (parameters is { Length: > 0 })
                cmd.Parameters.AddRange(parameters);

            await conn.OpenAsync(cancellationToken);
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
            // ⚠️ We can’t use ExecuteAsync here because connection must remain open for reader lifetime
            var conn = await CreateConnectionAsync(cancellationToken);
            var cmd = new NpgsqlCommand(sql, conn);

            if (parameters is { Length: > 0 })
                cmd.Parameters.AddRange(parameters);

            await conn.OpenAsync(cancellationToken);

            // CommandBehavior.CloseConnection ensures conn closes when reader is disposed
            return await cmd.ExecuteReaderAsync(System.Data.CommandBehavior.CloseConnection, cancellationToken);
        }
    }
}
