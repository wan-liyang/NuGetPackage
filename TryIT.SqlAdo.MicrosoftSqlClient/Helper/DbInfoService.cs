using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using TryIT.SqlAdo.MicrosoftSqlClient.Core;
using TryIT.SqlAdo.MicrosoftSqlClient.Models;

namespace TryIT.SqlAdo.MicrosoftSqlClient.Helper
{
    /// <summary>
    /// Service that provides database metadata (identity columns, table structure, encrypted columns).
    /// </summary>
    internal class DbInfoService
    {
        private readonly SqlCommandExecutor _executor;
        private readonly int _timeoutSeconds;

        public DbInfoService(SqlCommandExecutor executor, int timeoutSeconds)
        {
            _executor = executor ?? throw new ArgumentNullException(nameof(executor));
            _timeoutSeconds = timeoutSeconds;
        }

        /// <summary>
        /// Gets the identity column name for a given table.
        /// </summary>
        public string GetIdentityColumnName(string fullTableName)
        {
            string sql = @"
                SELECT COLUMN_NAME 
                FROM INFORMATION_SCHEMA.COLUMNS 
                WHERE TABLE_SCHEMA + '.' + TABLE_NAME = @tableName 
                  AND COLUMNPROPERTY(OBJECT_ID(TABLE_SCHEMA + '.' + TABLE_NAME), COLUMN_NAME, 'IsIdentity') = 1";

            var parameters = new[] { new SqlParameter("@tableName", fullTableName) };

            return _executor.ExecuteWithRetryAndLogAsync<string>(
                sql,
                CommandType.Text,
                async (conn, trans, token) =>
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandTimeout = _timeoutSeconds;
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddRange(SqlCommandExecutor.CloneParameters(parameters));
                        var result = await cmd.ExecuteScalarAsync(token).ConfigureAwait(false);
                        return result?.ToString();
                    }
                },
                parameters,
                false,
                default
            ).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets the structure of a user table.
        /// </summary>
        public List<DbTableStructure> GetDbTableStructure(string fullTableName)
        {
            string sql = @"
                SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH 
                FROM INFORMATION_SCHEMA.COLUMNS 
                WHERE TABLE_SCHEMA + '.' + TABLE_NAME = @tableName";

            var parameters = new[] { new SqlParameter("@tableName", fullTableName) };

            var dt = _executor.ExecuteWithRetryAndLogAsync<DataTable>(
                sql,
                CommandType.Text,
                async (conn, trans, token) =>
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandTimeout = _timeoutSeconds;
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddRange(SqlCommandExecutor.CloneParameters(parameters));
                        using (var adapter = new SqlDataAdapter(cmd))
                        {
                            var result = new DataTable();
                            await Task.Run(() => adapter.Fill(result), token).ConfigureAwait(false);
                            return result;
                        }
                    }
                },
                parameters,
                false,
                default
            ).GetAwaiter().GetResult();

            if (dt == null || dt.Rows.Count == 0)
                throw new InvalidOperationException($"Table '{fullTableName}' not found or no access.");

            return dt.Rows.OfType<DataRow>().Select(k =>
                  new DbTableStructure
                  {
                      TABLE_NAME = fullTableName,
                      COLUMN_NAME = k[0].ToString(),
                      DATA_TYPE = k[1].ToString(),
                      CHARACTER_MAXIMUM_LENGTH = k[2].ToString()
                  }).ToList();
        }

        /// <summary>
        /// Gets the structure of a temp table (in tempdb).
        /// </summary>
        public List<DbTableStructure> GetDbTableStructure_TempDb(string fullTableName)
        {
            string sql = @"
                SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH 
                FROM Tempdb.INFORMATION_SCHEMA.COLUMNS 
                WHERE TABLE_SCHEMA + '.' + TABLE_NAME = @tableName";

            var parameters = new[] { new SqlParameter("@tableName", fullTableName) };

            var dt = _executor.ExecuteWithRetryAndLogAsync<DataTable>(
                sql,
                CommandType.Text,
                async (conn, trans, token) =>
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandTimeout = _timeoutSeconds;
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddRange(SqlCommandExecutor.CloneParameters(parameters));
                        using (var adapter = new SqlDataAdapter(cmd))
                        {
                            var result = new DataTable();
                            await Task.Run(() => adapter.Fill(result), token).ConfigureAwait(false);
                            return result;
                        }
                    }
                },
                parameters,
                false,
                default
            ).GetAwaiter().GetResult();

            if (dt == null || dt.Rows.Count == 0)
                throw new InvalidOperationException($"Temp table '{fullTableName}' not found or no access.");

            return dt.Rows.OfType<DataRow>().Select(k =>
                  new DbTableStructure
                  {
                      TABLE_NAME = fullTableName,
                      COLUMN_NAME = k[0].ToString(),
                      DATA_TYPE = k[1].ToString(),
                      CHARACTER_MAXIMUM_LENGTH = k[2].ToString()
                  }).ToList();
        }

        /// <summary>
        /// Gets all Always Encrypted columns (optionally for a specific table).
        /// </summary>
        public List<AlwaysEncryptedColumn> GetAlwaysEncryptedColumns(string tableName = "")
        {
            string sql = @"
                SELECT 
                    sch.name + '.' + tbl.name AS TableName,
                    col.name AS ColumnName,
                    typ.name AS ColumnType,
                    CASE WHEN typ.name = 'nvarchar' THEN col.max_length / 2 ELSE col.max_length END AS ColumnMaxLength,
                    col.Precision,
                    col.Scale,
                    col.encryption_type_desc AS EncryptionType,
                    cek.name AS ColumnEncryptKeyName,
                    cmk.name AS MasterKeyName,
                    cmk.key_store_provider_name AS KeyStoreProviderName,
                    cmk.key_path AS KeyPath
                FROM sys.tables tbl
                JOIN sys.columns col ON col.object_id = tbl.object_id
                JOIN sys.column_encryption_keys cek ON cek.column_encryption_key_id = col.column_encryption_key_id
                JOIN sys.column_encryption_key_values cek_val ON cek_val.column_encryption_key_id = cek.column_encryption_key_id
                JOIN sys.column_master_keys cmk ON cmk.column_master_key_id = cek_val.column_master_key_id
                JOIN sys.schemas sch ON sch.schema_id = tbl.schema_id 
                JOIN sys.types typ ON typ.system_type_id = col.system_type_id 
                    AND typ.user_type_id = col.user_type_id
                WHERE col.[encryption_type] IS NOT NULL
                  AND (sch.name + '.' + tbl.name = @tableName OR @tableName = '')";

            var parameters = new[] { new SqlParameter("@tableName", tableName ?? "") };

            var dt = _executor.ExecuteWithRetryAndLogAsync<DataTable>(
                sql,
                CommandType.Text,
                async (conn, trans, token) =>
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandTimeout = _timeoutSeconds;
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddRange(SqlCommandExecutor.CloneParameters(parameters));
                        using (var adapter = new SqlDataAdapter(cmd))
                        {
                            var result = new DataTable();
                            await Task.Run(() => adapter.Fill(result), token).ConfigureAwait(false);
                            return result;
                        }
                    }
                },
                parameters,
                false,
                default
            ).GetAwaiter().GetResult();

            return dt.Rows.OfType<DataRow>().Select(k =>
                  new AlwaysEncryptedColumn
                  {
                      TableName = k[0].ToString(),
                      ColumnName = k[1].ToString(),
                      ColumnType = k[2].ToString(),
                      ColumnMaxLength = int.Parse(k[3].ToString()),
                      Precision = int.Parse(k[4].ToString()),
                      Scale = int.Parse(k[5].ToString()),
                      EncryptionType = k[6].ToString(),
                      ColumnEncryptKeyName = k[7].ToString(),
                      MasterKeyName = k[8].ToString(),
                      KeyStoreProviderName = k[9].ToString(),
                      KeyPath = k[10].ToString()
                  }).ToList();
        }
    }
}