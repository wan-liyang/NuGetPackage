using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using TryIT.SqlAdo.MicrosoftSqlClient.CopyMode;
using TryIT.SqlAdo.MicrosoftSqlClient.Core;
using TryIT.SqlAdo.MicrosoftSqlClient.Helper;
using TryIT.SqlAdo.MicrosoftSqlClient.Models;
using TryIT.SqlAdo.MicrosoftSqlClient.Validations;

namespace TryIT.SqlAdo.MicrosoftSqlClient.BulkOperations
{
    /// <summary>
    /// Service that handles bulk copy operations (Insert/Update, TruncateInsert, DeleteInsert, etc.)
    /// including Always Encrypted support.
    /// </summary>
    internal class SqlBulkCopyService
    {
        private readonly ConnectorConfig _config;
        private readonly SqlCommandExecutor _executor;
        private readonly DbInfoService _dbInfoService;

        public SqlBulkCopyService(ConnectorConfig config, SqlCommandExecutor executor)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _executor = executor ?? throw new ArgumentNullException(nameof(executor));
            _dbInfoService = new DbInfoService(_executor, _config.TimeoutSecond);
        }

        #region Public Entry Point

        /// <summary>
        /// Copies data from multiple copy modes within a single transaction.
        /// </summary>
        public void CopyData(List<ICopyMode> copyModes)
        {
            var tobeCopyModes = new List<DataCopyInfo>();

            // Validate and gather target table structures (outside transaction to avoid locks)
            foreach (var item in copyModes)
            {
                var copyMode = item as CopyModeBase;
                if (copyMode == null)
                    throw new ArgumentException($"{nameof(copyMode)} must be of type CopyModeBase");

                if (string.IsNullOrEmpty(copyMode.TargetTable))
                    throw new ArgumentException($"{nameof(copyMode.TargetTable)} cannot be null or empty");

                if (copyMode.TargetTable.Split('.').Length != 2)
                    throw new ArgumentException($"TargetTable '{copyMode.TargetTable}' must contain schema and table name, e.g. schema.table");

                var targetTableStructure = _dbInfoService.GetDbTableStructure(copyMode.TargetTable);
                copyMode.ColumnMappings = ResetColumnMap(copyMode.SourceData, copyMode.ColumnMappings);
                ColumnMapValidation.ValidateColumnMap(copyMode.SourceData, targetTableStructure, copyMode.ColumnMappings);

                tobeCopyModes.Add(new DataCopyInfo
                {
                    CopyMode = item,
                    TableStructures = targetTableStructure
                });
            }

            using (var connection = _executor.OpenConnection())
            {
                var transaction = connection.BeginTransaction();
                try
                {
                    foreach (var info in tobeCopyModes)
                    {
                        var copyMode = info.CopyMode as CopyModeBase;
                        var targetStructure = info.TableStructures;

                        // Pre-script
                        if (!string.IsNullOrEmpty(copyMode.PreScript))
                        {
                            using (var cmd = new SqlCommand(copyMode.PreScript, connection, transaction))
                            {
                                cmd.CommandTimeout = _config.TimeoutSecond;
                                cmd.ExecuteNonQuery();
                            }
                        }

                        // Determine copy type and execute
                        if (copyMode is CopyMode_InsertUpdate insertUpdateMode)
                        {
                            if (insertUpdateMode.SourceData.Rows.Count > 0)
                            {
                                var encryptedColumns = _dbInfoService.GetAlwaysEncryptedColumns(insertUpdateMode.TargetTable);
                                if (encryptedColumns?.Count > 0)
                                {
                                    ColumnMapValidation.PrimaryKeyExistsInColumnMap(insertUpdateMode.PrimaryKeys, insertUpdateMode.ColumnMappings);
                                    UpsertEncrypted(insertUpdateMode, connection, transaction, encryptedColumns);
                                }
                                else
                                {
                                    ColumnMapValidation.PrimaryKeyExistsInColumnMap(insertUpdateMode.PrimaryKeys, insertUpdateMode.ColumnMappings);
                                    UpsertToDestination(insertUpdateMode, connection, transaction, targetStructure);
                                }
                            }
                        }
                        else if (copyMode is UpdateCopyMode updateMode)
                        {
                            if (updateMode.SourceData.Rows.Count > 0)
                            {
                                ColumnMapValidation.PrimaryKeyExistsInColumnMap(updateMode.PrimaryKeys, updateMode.ColumnMappings);
                                UpdateToDestination(updateMode, connection, transaction, targetStructure);
                            }
                        }
                        else
                        {
                            // TruncateInsert or DeleteInsert
                            if (copyMode is CopyMode_TruncateInsert truncateInsertMode)
                            {
                                string cmdText = $"TRUNCATE TABLE {SqlHelper.SqlWarpTable(truncateInsertMode.TargetTable)};";
                                using (var cmd = new SqlCommand(cmdText, connection, transaction))
                                {
                                    cmd.CommandTimeout = _config.TimeoutSecond;
                                    cmd.ExecuteNonQuery();
                                }
                            }
                            else if (copyMode is CopyMode_DeleteInsert deleteInsertMode && string.IsNullOrEmpty(deleteInsertMode.PreScript))
                            {
                                throw new ArgumentException($"PreScript cannot be empty for {nameof(CopyMode_DeleteInsert)}");
                            }

                            // Bulk copy
                            BulkCopy(copyMode.SourceData, copyMode.TargetTable, targetStructure, copyMode.ColumnMappings, connection, transaction);
                        }

                        // Post-script
                        if (!string.IsNullOrEmpty(copyMode.PostScript))
                        {
                            using (var cmd = new SqlCommand(copyMode.PostScript, connection, transaction))
                            {
                                cmd.CommandTimeout = _config.TimeoutSecond;
                                cmd.ExecuteNonQuery();
                            }
                        }

                        // Post-action
                        copyMode.PostAction?.Invoke(connection, transaction);
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    try
                    {
                        if (transaction?.Connection?.State == ConnectionState.Open)
                            transaction.Rollback();
                    }
                    catch (Exception rollbackEx)
                    {
                        ex.Data["RollbackException"] = rollbackEx;
                    }
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }
        }

        #endregion

        #region Private Helpers (Metadata)

        private class DataCopyInfo
        {
            public ICopyMode CopyMode { get; set; }
            public List<DbTableStructure> TableStructures { get; set; }
        }

        #endregion

        #region Core Copy Helpers

        private static string GetTargetColumn(List<DbTableStructure> structures, string mapValue)
        {
            var match = structures.FirstOrDefault(p => p.COLUMN_NAME.Equals(mapValue, StringComparison.CurrentCultureIgnoreCase));
            if (match == null)
                throw new ArgumentException($"Column '{mapValue}' not found in target table");
            return match.COLUMN_NAME;
        }

        private static Dictionary<string, string> ResetColumnMap(DataTable sourceData, Dictionary<string, string> columnMap)
        {
            if (columnMap != null && columnMap.Count > 0)
                return columnMap.Where(p => !string.IsNullOrEmpty(p.Key) && !string.IsNullOrEmpty(p.Value))
                                .ToDictionary(x => x.Key, x => x.Value);

            var map = new Dictionary<string, string>();
            foreach (DataColumn col in sourceData.Columns)
                map[col.ColumnName] = col.ColumnName;
            return map;
        }

        private void BulkCopy(DataTable sourceTable, string targetTable, List<DbTableStructure> targetStructure,
                              Dictionary<string, string> columnMappings, SqlConnection connection, SqlTransaction transaction)
        {
            if (sourceTable.Rows.Count == 0) return;

            var options = SqlBulkCopyOptions.TableLock | SqlBulkCopyOptions.FireTriggers |
                          SqlBulkCopyOptions.KeepIdentity | SqlBulkCopyOptions.KeepNulls;
            using (var bulk = new SqlBulkCopy(connection, options, transaction))
            {
                bulk.BulkCopyTimeout = _config.TimeoutSecond;
                bulk.DestinationTableName = targetTable;

                foreach (var map in columnMappings)
                {
                    string actual = map.Value;
                    if (targetStructure != null && targetStructure.Count > 0)
                        actual = GetTargetColumn(targetStructure, map.Value);
                    bulk.ColumnMappings.Add(map.Key, actual);
                }

                bulk.WriteToServer(sourceTable);
            }
        }

        #endregion

        #region Update / Upsert Logic

        private string BuildPrimaryKeyCondition(List<string> primaryKeys, Dictionary<string, string> mappings)
        {
            var conditions = new List<string>();
            foreach (string t_col in primaryKeys)
            {
                string s_col = mappings.First(p => p.Value.Equals(t_col, StringComparison.CurrentCultureIgnoreCase)).Key;
                conditions.Add($"S.{SqlHelper.SqlWarpColumn(s_col)} = T.{SqlHelper.SqlWarpColumn(t_col)}");
            }
            return string.Join(" AND ", conditions);
        }

        private string BuildUpdateSetClause(Dictionary<string, string> mappings, List<string> primaryKeys, string timestampColumn)
        {
            var toUpdate = mappings.Where(p => !primaryKeys.Any(k => k.Equals(p.Value, StringComparison.CurrentCultureIgnoreCase)));
            var setClauses = toUpdate.Select(p => $"T.{SqlHelper.SqlWarpColumn(p.Value)} = S.{SqlHelper.SqlWarpColumn(p.Key)}").ToList();
            if (!string.IsNullOrEmpty(timestampColumn))
                setClauses.Add($"{SqlHelper.SqlWarpColumn(timestampColumn)} = GETDATE()");
            return string.Join(", ", setClauses);
        }

        private string WrapIdentityInsertIfNeeded(string warppedTable, string targetTable, Dictionary<string, string> mappings, string sql)
        {
            string identityColumn = _dbInfoService.GetIdentityColumnName(targetTable);
            if (!string.IsNullOrEmpty(identityColumn))
            {
                if (mappings.Any(p => p.Value.Equals(identityColumn, StringComparison.CurrentCultureIgnoreCase)))
                    return SqlHelper.SqlWarpIdentityInsert(warppedTable, sql);
            }
            return sql;
        }

        /// <summary>
        /// write data into temp table
        /// </summary>
        /// <param name="copyMode"></param>
        /// <param name="sqlConnection"></param>
        /// <param name="transaction"></param>
        /// <param name="targetTableStructure"></param>
        /// <returns></returns>
        private string WriteDataIntoTempTable(ICopyMode copyMode, SqlConnection sqlConnection, SqlTransaction transaction, List<DbTableStructure> targetTableStructure)
        {
            var copyStuff = copyMode as CopyModeBase;
            // create temp table
            string tempTable = $"#temp_{SqlHelper.GetGuid()}";

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"CREATE TABLE {tempTable}(");

            foreach (var dic in copyStuff.ColumnMappings)
            {
                string s_col = dic.Key;
                string t_col = dic.Value;
                var structure = targetTableStructure.First(p => p.COLUMN_NAME.Equals(t_col, StringComparison.CurrentCultureIgnoreCase));

                string dataType = "NVARCHAR(MAX)";
                switch (structure.DATA_TYPE)
                {
                    case "datetime":
                    case "time":
                    case "date":
                    case "bit":
                    case "int":
                    case "uniqueidentifier":
                        dataType = structure.DATA_TYPE;
                        break;
                    case "varbinary":
                        dataType = "varbinary(max)";
                        break;
                    default:
                        break;
                }
                string column = $"{SqlHelper.SqlWarpColumn(s_col)} {dataType}";
                stringBuilder.Append($"{column}");

                stringBuilder.Append(",");
            }
            // remove last ,
            stringBuilder.Remove(stringBuilder.Length - 1, 1);
            stringBuilder.Append(");");

            string sql_create = stringBuilder.ToString();
            using (SqlCommand cmd = new SqlCommand(sql_create, sqlConnection, transaction))
            {
                cmd.CommandTimeout = _config.TimeoutSecond;
                cmd.ExecuteNonQuery();
            }

            // insert data into temp table, column map need be SourceColumn - SourceColumn
            Dictionary<string, string> tempTableMap = copyStuff.ColumnMappings.ToDictionary(x => x.Key, x => x.Key);
            BulkCopy(copyStuff.SourceData, tempTable, null, tempTableMap, sqlConnection, transaction);

            return tempTable;
        }

        private void UpdateToDestination(UpdateCopyMode copyMode, SqlConnection connection, SqlTransaction transaction,
                                         List<DbTableStructure> targetStructure)
        {
            string tempTable = WriteDataIntoTempTable(copyMode, connection, transaction, targetStructure);
            string warppedTable = SqlHelper.SqlWarpTable(copyMode.TargetTable);

            string sql_key = BuildPrimaryKeyCondition(copyMode.PrimaryKeys, copyMode.ColumnMappings);
            string sql_update = BuildUpdateSetClause(copyMode.ColumnMappings, copyMode.PrimaryKeys, copyMode.TimestampColumn);

            string sql = $@"
                UPDATE T 
                SET {sql_update}
                FROM {warppedTable} T
                INNER JOIN {tempTable} S ON {sql_key};
                DROP TABLE {tempTable};";

            sql = WrapIdentityInsertIfNeeded(warppedTable, copyMode.TargetTable, copyMode.ColumnMappings, sql);

            using (var cmd = new SqlCommand(sql, connection, transaction))
            {
                cmd.CommandTimeout = _config.TimeoutSecond;
                cmd.ExecuteNonQuery();
            }
        }

        private void UpsertToDestination(CopyMode_InsertUpdate copyMode, SqlConnection connection, SqlTransaction transaction,
                                         List<DbTableStructure> targetStructure)
        {
            string tempTable = WriteDataIntoTempTable(copyMode, connection, transaction, targetStructure);
            string warppedTable = SqlHelper.SqlWarpTable(copyMode.TargetTable);

            string sql_key = BuildPrimaryKeyCondition(copyMode.PrimaryKeys, copyMode.ColumnMappings);
            string sql_update = BuildUpdateSetClause(copyMode.ColumnMappings, copyMode.PrimaryKeys, copyMode.TimestampColumn);

            string sql_source_col = string.Join(", ", SqlHelper.SqlWarpColumn(copyMode.ColumnMappings.Keys));
            string sql_target_col = string.Join(", ", SqlHelper.SqlWarpColumn(copyMode.ColumnMappings.Values));

            string sql = $@"
                UPDATE T 
                SET {sql_update}
                FROM {warppedTable} T
                INNER JOIN {tempTable} S ON {sql_key};

                DELETE S 
                FROM {tempTable} S
                INNER JOIN {warppedTable} T ON {sql_key};

                INSERT INTO {warppedTable} ({sql_target_col})
                SELECT {sql_source_col}
                FROM {tempTable} S;

                DROP TABLE {tempTable};";

            sql = WrapIdentityInsertIfNeeded(warppedTable, copyMode.TargetTable, copyMode.ColumnMappings, sql);

            using (var cmd = new SqlCommand(sql, connection, transaction))
            {
                cmd.CommandTimeout = _config.TimeoutSecond;
                cmd.ExecuteNonQuery();
            }
        }

        private void UpsertEncrypted(CopyMode_InsertUpdate copyMode, SqlConnection connection, SqlTransaction transaction,
                                     List<AlwaysEncryptedColumn> encryptedColumns)
        {
            ConsoleLog("Upsert with Always Encrypted started");

            string sql_where = "";
            string sql_set = "";
            string sql_insert_columns = "";
            string sql_insert_params = "";

            foreach (string t_col in copyMode.PrimaryKeys)
            {
                if (!string.IsNullOrEmpty(sql_where))
                    sql_where += " AND ";
                sql_where += $"{SqlHelper.SqlWarpColumn(t_col)} = @{SqlHelper.SqlParamName(t_col)}";

                sql_insert_columns += $"{SqlHelper.SqlWarpColumn(t_col)},";
                sql_insert_params += $"@{SqlHelper.SqlParamName(t_col)},";
            }

            var tobeUpdateColumns = copyMode.ColumnMappings
                .Where(p => !copyMode.PrimaryKeys.Any(k => k.Equals(p.Value, StringComparison.CurrentCultureIgnoreCase)))
                .ToDictionary(x => x.Key, x => x.Value);

            foreach (var item in tobeUpdateColumns)
            {
                sql_set += $"{SqlHelper.SqlWarpColumn(item.Value)} = @{SqlHelper.SqlParamName(item.Value)},";
                sql_insert_columns += $"{SqlHelper.SqlWarpColumn(item.Value)},";
                sql_insert_params += $"@{SqlHelper.SqlParamName(item.Value)},";
            }
            sql_set = sql_set.TrimEnd(',');
            sql_insert_columns = sql_insert_columns.TrimEnd(',');
            sql_insert_params = sql_insert_params.TrimEnd(',');

            string warppedTable = SqlHelper.SqlWarpTable(copyMode.TargetTable);
            string sql = $@"
                IF EXISTS (SELECT 1 FROM {warppedTable} WHERE {sql_where})
                BEGIN
                    UPDATE {warppedTable} SET {sql_set} WHERE {sql_where}
                END
                ELSE
                BEGIN
                    INSERT INTO {warppedTable} ({sql_insert_columns}) VALUES ({sql_insert_params})
                END;";

            sql = WrapIdentityInsertIfNeeded(warppedTable, copyMode.TargetTable, copyMode.ColumnMappings, sql);

            using (var sqlCommand = new SqlCommand(sql, connection, transaction))
            {
                sqlCommand.CommandType = CommandType.Text;
                int row = 0;

                foreach (DataRow dataRow in copyMode.SourceData.Rows)
                {
                    // Primary keys
                    foreach (string t_col in copyMode.PrimaryKeys)
                    {
                        string s_col = copyMode.ColumnMappings.First(p => p.Value.Equals(t_col, StringComparison.CurrentCultureIgnoreCase)).Key;
                        var encryptedCol = encryptedColumns.FirstOrDefault(p => p.ColumnName.Equals(t_col, StringComparison.CurrentCultureIgnoreCase));
                        var param = SqlHelper.GetParameter(t_col, dataRow[s_col], encryptedCol);
                        sqlCommand.Parameters.Add(param);
                    }

                    // Update columns
                    foreach (var item in tobeUpdateColumns)
                    {
                        string t_col = item.Value;
                        string s_col = item.Key;
                        var encryptedCol = encryptedColumns.FirstOrDefault(p => p.ColumnName.Equals(t_col, StringComparison.CurrentCultureIgnoreCase));
                        var param = SqlHelper.GetParameter(t_col, dataRow[s_col], encryptedCol);
                        sqlCommand.Parameters.Add(param);
                    }

                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Parameters.Clear();

                    row++;
                    if (row % 2000 == 0)
                        ConsoleLog($"{row} upsert executed");
                }

                ConsoleLog($"{copyMode.SourceData.Rows.Count} upsert executed");
            }

            ConsoleLog("Upsert with Always Encrypted completed");
        }

        #endregion

        #region Misc Helpers

        private static void ConsoleLog(string message)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff");
            Console.WriteLine($"[{timestamp}] {message}");
        }

        #endregion
    }
}