using Npgsql;

namespace TryIT.PostgreSQL
{
    /// <summary>
    /// Helper class for PostgreSQL database operations
    /// </summary>
    public class PostgreSqlHelper
    {
        private readonly string _connStr;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlHelper"/> class.
        /// </summary>
        /// <param name="connectionString"></param>
        public PostgreSqlHelper(string connectionString)
        {
            _connStr = connectionString;
        }

        private async Task<TResult> ExecuteAsync<TResult>(
            string sql,
            Func<NpgsqlCommand, CancellationToken, Task<TResult>> executor,
            NpgsqlParameter[]? parameters = null,
            CancellationToken cancellationToken = default)
        {
            await using var conn = new NpgsqlConnection(_connStr);
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
            var conn = new NpgsqlConnection(_connStr);
            var cmd = new NpgsqlCommand(sql, conn);

            if (parameters is { Length: > 0 })
                cmd.Parameters.AddRange(parameters);

            await conn.OpenAsync(cancellationToken);

            // CommandBehavior.CloseConnection ensures conn closes when reader is disposed
            return await cmd.ExecuteReaderAsync(System.Data.CommandBehavior.CloseConnection, cancellationToken);
        }
    }
}
