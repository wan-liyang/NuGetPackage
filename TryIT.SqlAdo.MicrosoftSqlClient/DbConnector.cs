using Microsoft.Data.SqlClient;
using Microsoft.Data.SqlClient.AlwaysEncrypted.AzureKeyVaultProvider;
using Microsoft.IdentityModel.Tokens;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using TryIT.SqlAdo.MicrosoftSqlClient.CopyMode;
using TryIT.SqlAdo.MicrosoftSqlClient.Helper;
using TryIT.SqlAdo.MicrosoftSqlClient.Models;

namespace TryIT.SqlAdo.MicrosoftSqlClient
{
    /// <summary>
    /// database connector
    /// </summary>
    public class DbConnector
    {
        private readonly ResiliencePipeline _pipeline;
        private ConnectorConfig _config;

        /// <summary>
        /// retry results
        /// </summary>
        public List<RetryResult> RetryResults = new List<RetryResult>();

        /// <summary>
        /// initial database connector
        /// </summary>
        /// <param name="config"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public DbConnector(ConnectorConfig config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            if (string.IsNullOrEmpty(config.ConnectionString))
            {
                throw new ArgumentNullException(nameof(config.ConnectionString));
            }
            _config = config;

            if (config.EnableRetry)
            {
                var builder = new PredicateBuilder();
                if (config.RetryOn.SqlTimeout)
                {
                    builder.Handle<SqlException>(result => result.Number == -2);
                }
                if (config.RetryOn.EstablishConnection)
                {
                    builder.Handle<SqlException>(result => result.Message.StartsWith(config.RetryOn.EstablishConnnectionErrorMessage));
                }
                if (config.RetryOn.Deadlock)
                {
                    builder.Handle<SqlException>(result => result.Message.Contains(config.RetryOn.DeadlockErrorMessage));
                }

                _pipeline = new ResiliencePipelineBuilder()
                           .AddRetry(new RetryStrategyOptions
                           {
                               ShouldHandle = builder,

                               Delay = TimeSpan.FromSeconds(1),
                               MaxRetryAttempts = 3,
                               BackoffType = DelayBackoffType.Constant,
                               OnRetry = args =>
                               {
                                   RetryResults.Add(new RetryResult
                                   {
                                       AttemptNumber = args.AttemptNumber,
                                       Exception = args.Outcome.Exception
                                   });

                                   return default;
                               }
                           })
                           .Build();
            }
            else
            {
                _pipeline = new ResiliencePipelineBuilder().Build();
            }
        }

        /// <summary>
        /// register Azure Key Vault column encryption key store provider
        /// <para>IMPORTANT: please call this method once only during program starting, otherwise may encounter 'key store providers cannot be set more than once.' error</para>
        /// </summary>
        /// <param name="azureKeyVaultProvider"></param>
        public static void RegisterColumnEncryptionKeyStore_AKV(AzureKeyVaultProvider azureKeyVaultProvider)
        {
            var credential = AzureHelper.GetClientSecretCredential(azureKeyVaultProvider);
            SqlColumnEncryptionAzureKeyVaultProvider akvProvider = new SqlColumnEncryptionAzureKeyVaultProvider(credential);

            // Register AKV provider
            SqlConnection.RegisterColumnEncryptionKeyStoreProviders(customProviders: new Dictionary<string, SqlColumnEncryptionKeyStoreProvider>(capacity: 1, comparer: StringComparer.OrdinalIgnoreCase)
                {
                    { SqlColumnEncryptionAzureKeyVaultProvider.ProviderName, akvProvider}
                });
        }

        /// <summary>
        /// fetch DataTable from <paramref name="commandText"/>
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public DataTable FetchDataTable(string commandText, CommandType commandType = CommandType.Text, SqlParameter[] parameters = null)
        {

            if (string.IsNullOrEmpty(commandText))
            {
                throw new ArgumentNullException(nameof(commandText));
            }

            return _pipeline.Execute(exec =>
            {
                using (SqlConnection sqlConnection = new SqlConnection(_config.ConnectionString))
                {
                    sqlConnection.Open();
                    using (SqlCommand cmd = sqlConnection.CreateCommand())
                    {
                        cmd.CommandTimeout = _config.TimeoutSecond;
                        cmd.CommandText = commandText;
                        cmd.CommandType = commandType;
                        if (parameters != null && parameters.Count() > 0)
                        {
                            cmd.Parameters.AddRange(ClondParameters(parameters));
                        }

                        using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dataTable = new DataTable();
                            sqlDataAdapter.Fill(dataTable);
                            return dataTable;
                        }
                    }
                }
            });
        }

        /// <summary>
        /// fetch DataSet from <paramref name="commandText"/>
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public DataSet FetchDataSet(string commandText, CommandType commandType, params SqlParameter[] parameters)
        {
            if (string.IsNullOrEmpty(commandText))
            {
                throw new ArgumentNullException(nameof(commandText));
            }

            return _pipeline.Execute(exec =>
            {
                using (SqlConnection sqlConnection = new SqlConnection(_config.ConnectionString))
                {
                    sqlConnection.Open();
                    using (SqlCommand cmd = sqlConnection.CreateCommand())
                    {
                        cmd.CommandTimeout = _config.TimeoutSecond;
                        cmd.CommandText = commandText;
                        cmd.CommandType = commandType;
                        if (null != parameters && parameters.Count() > 0)
                        {
                            cmd.Parameters.AddRange(ClondParameters(parameters));
                        }
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataSet ds = new DataSet();
                            adapter.Fill(ds);
                            return ds;
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Call Scalar Value Function, return the value, <paramref name="function"/> must include schema, e.g. schema.function
        /// <para>this method will call database with sql: SELECT schema.function(@parameter1, @parameter2, @parameter3, ...)</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="function"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public T FetchScalarFunction<T>(string function, params SqlParameter[] parameters)
        {
            if (string.IsNullOrEmpty(function))
            {
                throw new ArgumentNullException(nameof(function));
            }
            if (function.Split('.').Length != 2)
            {
                throw new InvalidOperationException($"Function '{function}' must contains schema and fucntionName, e.g schema.fucntionName or [schema].[fucntionName]");
            }

            return _pipeline.Execute(exec =>
            {
                using (SqlConnection sqlConnection = new SqlConnection(_config.ConnectionString))
                {
                    sqlConnection.Open();
                    using (SqlCommand cmd = sqlConnection.CreateCommand())
                    {
                        cmd.CommandTimeout = _config.TimeoutSecond;
                        cmd.CommandType = CommandType.Text;

                        StringBuilder strBuilder = new StringBuilder("SELECT ");
                        strBuilder.Append(function);
                        if (null != parameters && parameters.Count() > 0)
                        {
                            cmd.Parameters.AddRange(ClondParameters(parameters));

                            strBuilder.Append("(");
                            for (int i = 0; i < parameters.Count(); i++)
                            {
                                if (i > 0)
                                {
                                    strBuilder.Append(",");
                                }
                                strBuilder.Append(parameters[i].ParameterName);
                            }
                            strBuilder.Append(")");
                        }
                        else
                        {
                            strBuilder.Append("()");
                        }

                        cmd.CommandText = strBuilder.ToString();

                        object result = cmd.ExecuteScalar();

                        return UtilityHelper.ConvertValue<T>(result);
                    }
                }
            });
        }

        /// <summary>
        /// fetch table data from function, <paramref name="function"/> must include schema, e.g. schema.function
        /// <para>this method will call database with sql: SELECT * FROM schema.function(@parameter1, @parameter2, @parameter3, ...)</para>
        /// </summary>
        /// <param name="function"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public DataTable FetchDataTableFunction(string function, params SqlParameter[] parameters)
        {
            if (string.IsNullOrEmpty(function))
            {
                throw new ArgumentNullException(nameof(function));
            }
            if (function.Split('.').Length != 2)
            {
                throw new InvalidOperationException($"Function '{function}' must contains schema and fucntionName, e.g schema.fucntionName or [schema].[fucntionName]");
            }

            return _pipeline.Execute(exec =>
            {
                using (SqlConnection sqlConnection = new SqlConnection(_config.ConnectionString))
                {
                    sqlConnection.Open();
                    using (SqlCommand cmd = sqlConnection.CreateCommand())
                    {
                        cmd.CommandTimeout = _config.TimeoutSecond;
                        cmd.CommandType = CommandType.Text;

                        StringBuilder strBuilder = new StringBuilder("SELECT * FROM ");
                        strBuilder.Append(function);
                        strBuilder.Append("(");
                        if (null != parameters && parameters.Count() > 0)
                        {
                            cmd.Parameters.AddRange(ClondParameters(parameters));
                            var parameterNames = parameters.Select(p => p.ParameterName);
                            strBuilder.Append(string.Join(",", parameterNames));
                        }
                        strBuilder.Append(")");
                        cmd.CommandText = strBuilder.ToString();
                        DataTable dt = new DataTable();
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                        return dt;
                    }
                }
            });
        }

        /// <summary>
        /// Executes a Transact-SQL statement and returns the number of rows affected.
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        public int ExecuteNonQuery(string commandText, CommandType commandType, params SqlParameter[] parameters)
        {
            if (string.IsNullOrEmpty(commandText))
            {
                throw new ArgumentNullException(nameof(commandText));
            }

            return _pipeline.Execute(exec =>
            {
                using (SqlConnection sqlConnection = new SqlConnection(_config.ConnectionString))
                {
                    sqlConnection.Open();
                    using (SqlCommand cmd = sqlConnection.CreateCommand())
                    {
                        cmd.CommandTimeout = _config.TimeoutSecond;

                        cmd.CommandText = commandText;
                        cmd.CommandType = commandType;
                        if (null != parameters && parameters.Count() > 0)
                        {
                            cmd.Parameters.AddRange(ClondParameters(parameters));
                        }
                        return cmd.ExecuteNonQuery();
                    }
                }
            });
        }

        /// <summary>
        /// clond sql parameter, avoid "The SqlParameter is already contained by another SqlParameterCollection." error when retry execution
        /// </summary>
        /// <param name="originalParameters"></param>
        /// <returns></returns>
        private SqlParameter[] ClondParameters(SqlParameter[] originalParameters)
        {
            SqlParameter[] clonedParameters = new SqlParameter[originalParameters.Length];

            for (int i = 0, j = originalParameters.Length; i < j; i++)
            { 
                clonedParameters[i] = (SqlParameter)((ICloneable)originalParameters[i]).Clone(); 
            }

            return clonedParameters;
        }


        /// <summary>
        /// Executes the query, and returns the first column of the first row in the result set returned by the query. Additional columns or rows are ignored.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public T ExecuteScalar<T>(string commandText, CommandType commandType, params SqlParameter[] parameters)
        {
            return _pipeline.Execute(exec =>
            {
                using (SqlConnection sqlConnection = new SqlConnection(_config.ConnectionString))
                {
                    sqlConnection.Open();
                    using (SqlCommand cmd = sqlConnection.CreateCommand())
                    {
                        cmd.CommandTimeout = _config.TimeoutSecond;
                        cmd.CommandText = commandText;
                        cmd.CommandType = commandType;
                        if (null != parameters && parameters.Count() > 0)
                        {
                            cmd.Parameters.AddRange(ClondParameters(parameters));
                        }
                        object result = cmd.ExecuteScalar();

                        return UtilityHelper.ConvertValue<T>(result);
                    }
                }
            });
        }

        /// <summary>
        /// Sends the System.Data.SqlClient.SqlCommand.CommandText to the System.Data.SqlClient.SqlCommand.Connection, and builds a System.Data.SqlClient.SqlDataReader using one of the System.Data.CommandBehavior values.
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public SqlDataReader ExecuteReader(string commandText, params SqlParameter[] parameters)
        {
            SqlConnection sqlConnection = new SqlConnection(_config.ConnectionString);
            SqlCommand cmd = new SqlCommand(commandText, sqlConnection);
            cmd.CommandType = CommandType.Text;
            if (null != parameters && parameters.Count() > 0)
            {
                cmd.Parameters.AddRange(ClondParameters(parameters));
            }

            return cmd.ExecuteReader(CommandBehavior.CloseConnection);
        }

        /// <summary>
        /// copy data from <see cref="CopyModeBase.SourceData"/> into <see cref="CopyModeBase.TargetTable"/>
        /// </summary>
        /// <param name="iCopyMode"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public void CopyData(ICopyMode iCopyMode)
        {
            CopyModeBase _copyMode = iCopyMode as CopyModeBase;

            if (_copyMode == null)
            {
                throw new ArgumentNullException(nameof(_copyMode));
            }
            if (string.IsNullOrEmpty(_copyMode.TargetTable))
            {
                throw new ArgumentNullException(nameof(_copyMode.TargetTable));
            }
            if (_copyMode.TargetTable.Split('.').Length != 2)
            {
                throw new InvalidOperationException($"TargetTable '{_copyMode.TargetTable}' must contains schema and table, e.g schema.table or [schema].[table]");
            }

            using (SqlConnection sqlConnection = new SqlConnection(_config.ConnectionString))
            {
                sqlConnection.Open();

                SqlTransaction transaction = null;
                try
                {
                    string transName = GetGuid();

                    // get target table structure first, outside transaction, to avoid other script locked table
                    var targetTableStructure = GetDbTableStructure(_copyMode.TargetTable);

                    /*
                     reset column map info
                    1. if provided: will use provided map to validate
                    2. if not provided: will reset to source table column, then do validate
                     
                    this is to ensure source data are valid against target table, also source data map to correct target column, 
                    e.g. source data column sequence may different with target table column sequence, if not provide column map, then program will use source table column as default map to ensure source column map to correct target column

                     */
                    _copyMode.ColumnMappings = ResetColumnMap(_copyMode.SourceData, _copyMode.ColumnMappings);
                    ValidateColumnMap(_copyMode.SourceData, targetTableStructure, _copyMode.ColumnMappings);

                    // put unique transaction name to avoid any conflict
                    transaction = sqlConnection.BeginTransaction(transName);

                    if (!string.IsNullOrEmpty(_copyMode.PreScript))
                    {
                        using (SqlCommand cmd = new SqlCommand(_copyMode.PreScript, sqlConnection, transaction))
                        {
                            cmd.CommandTimeout = _config.TimeoutSecond;
                            cmd.ExecuteNonQuery();
                        }
                    }

                    if (iCopyMode is CopyMode_InsertUpdate)
                    {
                        var mode = iCopyMode as CopyMode_InsertUpdate;
                        if (mode.SourceData.Rows.Count > 0)
                        {
                            mode.ColumnMappings = ResetColumnMap(mode.SourceData, mode.ColumnMappings);

                            Upsert(mode, sqlConnection, transaction, targetTableStructure);
                        }
                    }
                    else
                    {
                        if (iCopyMode is CopyMode_TruncateInsert)
                        {
                            var mode = (CopyMode_TruncateInsert)iCopyMode;
                            // truncate table before load
                            string cmdText = $"TRUNCATE TABLE {mode.TargetTable};";
                            using (SqlCommand cmd = new SqlCommand(cmdText, sqlConnection, transaction))
                            {
                                cmd.CommandTimeout = _config.TimeoutSecond;
                                cmd.ExecuteNonQuery();
                            }
                        }
                        else if (iCopyMode is CopyMode_DeleteInsert)
                        {
                            var mode = (CopyMode_DeleteInsert)iCopyMode;

                            // PreScript is mandatory for DeleteInsert
                            if (string.IsNullOrEmpty(mode.PreScript))
                            {
                                throw new ArgumentNullException(nameof(mode.PreScript), $"The {mode.PreScript} cannot be empty when Copy Mode is {nameof(CopyMode_DeleteInsert)}");
                            }
                        }

                        BulkCopy(_copyMode.SourceData, _copyMode.TargetTable, targetTableStructure, _copyMode.ColumnMappings, sqlConnection, transaction);
                    }

                    if (!string.IsNullOrEmpty(_copyMode.PostScript))
                    {
                        using (SqlCommand cmd = new SqlCommand(_copyMode.PostScript, sqlConnection, transaction))
                        {
                            cmd.CommandTimeout = _config.TimeoutSecond;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    if (transaction != null)
                    {
                        transaction.Rollback();
                    }

                    throw ex;
                }
            }
        }

        /// <summary>
        /// get actual column name present in database, avoid case-sensitive issue
        /// </summary>
        /// <param name="tableStructures"></param>
        /// <param name="mapValue"></param>
        /// <returns></returns>
        private string GetTargetColumn(List<DbTableStructure> tableStructures, string mapValue)
        {
            var table = tableStructures.FirstOrDefault(p => p.COLUMN_NAME.Equals(mapValue, StringComparison.CurrentCultureIgnoreCase));
            if (table == null)
            {
                throw new Exception($"column '{mapValue}' not found in target table");
            }
            return table.COLUMN_NAME;
        }

        /// <summary>
        /// reset ColumnMap if not provide, default to source table column
        /// </summary>
        /// <param name="sourceData"></param>
        /// <param name="columnMap"></param>
        /// <returns></returns>
        private Dictionary<string, string> ResetColumnMap(DataTable sourceData, Dictionary<string, string> columnMap)
        {
            if (columnMap != null && columnMap.Count > 0)
            {
                return columnMap.Where(p => !p.Key.IsNullOrEmpty() && !p.Value.IsNullOrEmpty()).ToDictionary(x => x.Key, x => x.Value);
            }

            Dictionary<string, string> map = new Dictionary<string, string>();

            foreach (DataColumn col in sourceData.Columns)
            {
                map[col.ColumnName] = col.ColumnName;
            }

            return map;
        }

        /// <summary>
        /// validate column map against source DataTable and target Database Table Strucutre
        /// </summary>
        /// <param name="sourceTable"></param>
        /// <param name="targetTableStructure"></param>
        /// <param name="columnMap">expected source to target column map, if empty then will skip this validation</param>
        /// <exception cref="Exception"></exception>
        private void ValidateColumnMap(DataTable sourceTable, List<DbTableStructure> targetTableStructure, Dictionary<string, string> columnMap)
        {
            if (columnMap != null && columnMap.Count > 0)
            {
                // validate column mapping appear in source table
                {
                    List<string> sourceColumns = new List<string>();
                    foreach (DataColumn item in sourceTable.Columns)
                    {
                        sourceColumns.Add(item.ColumnName);
                    }
                    var notExists = columnMap.Where(map => !sourceColumns.Exists(s => s.Equals(map.Key, StringComparison.OrdinalIgnoreCase))).Select(p => p.Key).ToList();
                    if (notExists != null && notExists.Count > 0)
                    {
                        throw new Exception($"column map not found in source data table: {string.Join(", ", notExists)}");
                    }
                }

                // validate column mapping appear in target table
                {
                    string targetTable = targetTableStructure.First().TABLE_NAME;

                    var notExists = columnMap.Where(map => !targetTableStructure.Exists(t => t.COLUMN_NAME.Equals(map.Value, StringComparison.OrdinalIgnoreCase))).Select(p => p.Value).ToList();
                    if (notExists != null && notExists.Count > 0)
                    {
                        throw new Exception($"column map not found in target database, table: {targetTable}, column: {string.Join(", ", notExists)}");
                    }
                }
            }
        }

        /// <summary>
        /// perform insert or update action, 1) write data into temp table, 2) insert or update to target table join with temp table, 3) delete temp table
        /// </summary>
        /// <param name="copyMode"></param>
        /// <param name="sqlConnection"></param>
        /// <param name="transaction"></param>
        /// <param name="targetTableStructure"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        private void Upsert(CopyMode_InsertUpdate copyMode, SqlConnection sqlConnection, SqlTransaction transaction, List<DbTableStructure> targetTableStructure)
        {
            if (copyMode.PrimaryKeys == null || copyMode.PrimaryKeys.Count == 0)
            {
                throw new ArgumentNullException(nameof(copyMode.PrimaryKeys), "Primary Key is mandatory for UpdateInsert copy mode");
            }

            // write data into temp table
            string tempTable = WriteDataIntoTempTable(copyMode, sqlConnection, transaction, targetTableStructure);

            // do update & insert to target table, and drop temp table
            // build primary key sql
            string sql_key = string.Empty;
            int keys_count = copyMode.PrimaryKeys.Count;
            for (int i = 0; i < keys_count; i++)
            {
                string s_col = copyMode.PrimaryKeys[i];
                string t_col = copyMode.ColumnMappings.Values.Where(p => p.Equals(s_col, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

                if (string.IsNullOrEmpty(t_col))
                {
                    throw new Exception($"Configured primay key column [{s_col}] has no corresponding column in target table {copyMode.TargetTable}");
                }

                sql_key += $"S.{FormatColumn(s_col)} = T.{FormatColumn(t_col)}";

                if (i != keys_count - 1)
                {
                    sql_key += " AND ";
                }
            }

            // build update sql
            string sql_update = string.Empty;
            var tobeUpdateColumns = copyMode.ColumnMappings.Where(p => !copyMode.PrimaryKeys.Any(k => k.Equals(p.Value, StringComparison.CurrentCultureIgnoreCase))).ToDictionary(x => x.Key, x => x.Value);

            foreach (var item in tobeUpdateColumns)
            {
                string s_col = item.Key;
                string t_col = item.Value;

                sql_update += $"T.{FormatColumn(t_col)} = S.{FormatColumn(s_col)},";
            }
            sql_update = sql_update.TrimEnd(',');

            if (!string.IsNullOrEmpty(copyMode.TimestampColumn))
            {
                sql_update += $", {FormatColumn(copyMode.TimestampColumn)} = GETDATE()";
            }

            // build select & insert columns sql
            string sql_source_col = string.Join(", ", FormatColumn(copyMode.ColumnMappings.Keys));
            string sql_target_col = string.Join(", ", FormatColumn(copyMode.ColumnMappings.Values));

            // build update and insert sql
            string sql_upsert = $@"
                            SET IDENTITY_INSERT {copyMode.TargetTable} ON;

                            UPDATE T 
                            SET {sql_update}
                            FROM {copyMode.TargetTable} T
                            INNER JOIN {tempTable} S ON {sql_key};

                            DELETE S 
                            FROM {tempTable} S
                            INNER JOIN {copyMode.TargetTable} T ON {sql_key};

                            INSERT INTO {copyMode.TargetTable} ({sql_target_col})
                            SELECT {sql_source_col}
                            FROM {tempTable} S;

                            DROP TABLE {tempTable};

                            SET IDENTITY_INSERT {copyMode.TargetTable} OFF;
                            ";

            using (SqlCommand cmd = new SqlCommand(sql_upsert, sqlConnection, transaction))
            {
                cmd.CommandTimeout = _config.TimeoutSecond;
                cmd.ExecuteNonQuery();
            }
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
            string tempTable = $"#temp_{GetGuid()}";

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
                    case "date":
                    case "bit":
                    case "int":
                        dataType = structure.DATA_TYPE;
                        break;
                    default:
                        break;
                }
                string column = $"{FormatColumn(s_col)} {dataType}";
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

        /// <summary>
        /// do bulk copy
        /// </summary>
        /// <param name="sourceTable"></param>
        /// <param name="targetTable"></param>
        /// <param name="targetTableStructure"></param>
        /// <param name="columnMappings"></param>
        /// <param name="sqlConnection"></param>
        /// <param name="transaction"></param>
        private void BulkCopy(DataTable sourceTable, string targetTable, List<DbTableStructure> targetTableStructure, Dictionary<string, string> columnMappings, SqlConnection sqlConnection, SqlTransaction transaction)
        {
            if (sourceTable.Rows.Count > 0)
            {
                var bulkOptions = SqlBulkCopyOptions.TableLock | SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.KeepIdentity | SqlBulkCopyOptions.KeepNulls;
                using (var sqlBulkCopy = new SqlBulkCopy(sqlConnection, bulkOptions, transaction))
                {
                    sqlBulkCopy.BulkCopyTimeout = _config.TimeoutSecond;
                    sqlBulkCopy.DestinationTableName = targetTable;

                    if (columnMappings != null && columnMappings.Count > 0)
                    {
                        foreach (var item in columnMappings)
                        {
                            string actualColumn = item.Value;

                            if (targetTableStructure != null && targetTableStructure.Count > 0)
                            {
                                actualColumn = GetTargetColumn(targetTableStructure, item.Value);
                            }

                            sqlBulkCopy.ColumnMappings.Add(item.Key, actualColumn);
                        }
                    }

                    sqlBulkCopy.WriteToServer(sourceTable);
                }
            }
        }

        private class DbTableStructure
        {
            public string TABLE_NAME { get; set; }
            public string COLUMN_NAME { get; set; }
            public string DATA_TYPE { get; set; }
            public string CHARACTER_MAXIMUM_LENGTH { get; set; }
        }

        private List<DbTableStructure> GetDbTableStructure(string tableName)
        {
            string sql = $"SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA + '.' + TABLE_NAME = @tableName";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@tableName", tableName)
            };

            DataTable table = this.FetchDataTable(sql, CommandType.Text, parameters);

            if (table == null || table.Rows.Count == 0)
            {
                throw new Exception($"get target table structure failed, please ensure target table '{tableName}' exists and account has permission");
            }

            return table.Rows.OfType<DataRow>().Select(k =>
                  new DbTableStructure
                  {
                      TABLE_NAME = tableName,
                      COLUMN_NAME = k[0].ToString(),
                      DATA_TYPE = k[1].ToString(),
                      CHARACTER_MAXIMUM_LENGTH = k[2].ToString()
                  }).ToList();
        }

        private List<DbTableStructure> GetDbTableStructure_TempDb(string tableName)
        {
            string sql = $"SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH FROM Tempdb.INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA + '.' + TABLE_NAME = @tableName";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@tableName", tableName)
            };

            DataTable table = this.FetchDataTable(sql, CommandType.Text, parameters);

            if (table == null || table.Rows.Count == 0)
            {
                throw new Exception($"get target table structure failed, please ensure target table '{tableName}' exists and account has permission");
            }

            return table.Rows.OfType<DataRow>().Select(k =>
                  new DbTableStructure
                  {
                      TABLE_NAME = tableName,
                      COLUMN_NAME = k[0].ToString(),
                      DATA_TYPE = k[1].ToString(),
                      CHARACTER_MAXIMUM_LENGTH = k[2].ToString()
                  }).ToList();
        }

        /// <summary>
        /// get a Guid that doesn't contain any numbers and dash
        /// </summary>
        /// <returns></returns>
        private string GetGuid()
        {
            // before: 51e3aaa4-6ff6-475a-8f0f-78cac597b6c3
            // after: FBVDRRREGWWGEHFRIWAWHITRTFJHSGTD
            return string.Concat(Guid.NewGuid().ToString("N").Select(c => (char)(c + 17))).ToUpper();
        }

        /// <summary>
        /// format column, Column1 to [Column1]
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        private string FormatColumn(string column)
        {
            return column.StartsWith("[") ? column : $"[{column}]";
        }
        private List<string> FormatColumn(IEnumerable<string> columns)
        {
            List<string> strings = new List<string>();
            foreach (var column in columns)
            {
                strings.Add(column.StartsWith("[") ? column : $"[{column}]");
            }
            return strings;
        }
    }
}
