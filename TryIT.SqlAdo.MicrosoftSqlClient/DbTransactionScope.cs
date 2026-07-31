using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using TryIT.SqlAdo.MicrosoftSqlClient.Core;
using TryIT.SqlAdo.MicrosoftSqlClient.Helper;

namespace TryIT.SqlAdo.MicrosoftSqlClient
{
    /// <summary>
    /// Represents a transaction scope that allows multiple commands to be executed within a single database transaction.
    /// All commands are automatically logged with InTransaction = true.
    /// </summary>
    public class DbTransactionScope : IDisposable
    {
        private readonly SqlCommandExecutor _executor;
        private readonly SqlConnection _connection;
        private readonly SqlTransaction _transaction;
        private bool _committed;
        private bool _disposed;

        internal DbTransactionScope(SqlCommandExecutor executor)
        {
            _executor = executor ?? throw new ArgumentNullException(nameof(executor));
            _connection = _executor.OpenConnection();
            _transaction = _connection.BeginTransaction();
        }

        /// <summary>
        /// Executes a non-query command asynchronously within the transaction.
        /// </summary>
        public async Task<int> ExecuteNonQueryAsync(
            string sql,
            CommandType commandType,
            SqlParameter[] parameters = null,
            CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();
            return await _executor.ExecuteWithLogAsync(
                sql,
                commandType,
                _connection,
                _transaction,
                (cmd, token) => cmd.ExecuteNonQueryAsync(token),
                parameters,
                cancellationToken
            ).ConfigureAwait(false);
        }

        /// <summary>
        /// Executes a scalar command asynchronously within the transaction.
        /// </summary>
        public async Task<T> ExecuteScalarAsync<T>(
            string sql,
            CommandType commandType,
            SqlParameter[] parameters = null,
            CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();
            object result = await _executor.ExecuteWithLogAsync(
                sql,
                commandType,
                _connection,
                _transaction,
                (cmd, token) => cmd.ExecuteScalarAsync(token),
                parameters,
                cancellationToken
            ).ConfigureAwait(false);
            return SqlHelper.ConvertValue<T>(result);
        }

        /// <summary>
        /// Fetches a DataTable asynchronously within the transaction.
        /// </summary>
        public async Task<DataTable> FetchDataTableAsync(
            string sql,
            CommandType commandType,
            SqlParameter[] parameters = null,
            CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();
            return await _executor.ExecuteWithLogAsync(
                sql,
                commandType,
                _connection,
                _transaction,
                async (cmd, token) =>
                {
                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        await Task.Run(() => adapter.Fill(dt), token).ConfigureAwait(false);
                        return dt;
                    }
                },
                parameters,
                cancellationToken
            ).ConfigureAwait(false);
        }

        /// <summary>
        /// Fetches a DataSet asynchronously within the transaction.
        /// </summary>
        public async Task<DataSet> FetchDataSetAsync(
            string sql,
            CommandType commandType,
            SqlParameter[] parameters = null,
            CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();
            return await _executor.ExecuteWithLogAsync(
                sql,
                commandType,
                _connection,
                _transaction,
                async (cmd, token) =>
                {
                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        var ds = new DataSet();
                        await Task.Run(() => adapter.Fill(ds), token).ConfigureAwait(false);
                        return ds;
                    }
                },
                parameters,
                cancellationToken
            ).ConfigureAwait(false);
        }

        /// <summary>
        /// Commits the transaction.
        /// </summary>
        public void Commit()
        {
            ThrowIfDisposed();
            _transaction.Commit();
            _committed = true;
        }

        /// <summary>
        /// Rolls back the transaction.
        /// </summary>
        public void Rollback()
        {
            ThrowIfDisposed();
            _transaction.Rollback();
        }

        /// <summary>
        /// Disposes the scope. If not committed, automatically rolls back.
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;
            try
            {
                if (!_committed)
                    _transaction?.Rollback();
            }
            finally
            {
                _transaction?.Dispose();
                _connection?.Dispose();
                _disposed = true;
            }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(DbTransactionScope));
        }
    }
}