using Microsoft.Data.SqlClient;
using Microsoft.Data.SqlClient.AlwaysEncrypted.AzureKeyVaultProvider;
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

                _pipeline = new ResiliencePipelineBuilder()
                           .AddRetry(new RetryStrategyOptions
                           {
                               ShouldHandle = new PredicateBuilder()
                                    .Handle<SqlException>(result => result.Number == -2), // handle sql timeout

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

            using (SqlConnection sqlConnection = new SqlConnection(_config.ConnectionString))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.CommandTimeout = _config.TimeoutSecond;
                    sqlCommand.CommandText = commandText;
                    sqlCommand.CommandType = commandType;
                    if (parameters != null && parameters.Count() > 0)
                    {
                        sqlCommand.Parameters.AddRange(parameters);
                    }

                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        DataTable dataTable = new DataTable();
                        sqlDataAdapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
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
                        cmd.Parameters.AddRange(parameters);
                    }
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataSet ds = new DataSet();
                        adapter.Fill(ds);
                        return ds;
                    }
                }
            }
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
                        cmd.Parameters.AddRange(parameters);

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
                        cmd.Parameters.AddRange(parameters);
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
                            cmd.Parameters.AddRange(parameters);
                        }

                        return cmd.ExecuteNonQuery();
                    }
                }
            });
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
                        cmd.Parameters.AddRange(parameters);
                    }
                    object result = cmd.ExecuteScalar();

                    return UtilityHelper.ConvertValue<T>(result);
                }
            }
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
                cmd.Parameters.AddRange(parameters);
            }

            return cmd.ExecuteReader(CommandBehavior.CloseConnection);
        }

        /// <summary>
        /// copy data from <see cref="CopyModeBase.SourceData"/> into <see cref="CopyModeBase.TargetTable"/>
        /// <para>the column map is case-sensitive on source column and destination column</para>
        /// </summary>
        /// <param name="copyMode"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public void CopyData(ICopyMode copyMode)
        {
            CopyModeBase _copyMode = copyMode as CopyModeBase;

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

                    if (copyMode is CopyMode_InsertUpdate)
                    {
                        Upsert(copyMode as CopyMode_InsertUpdate, sqlConnection, transaction);
                    }
                    else
                    {
                        if (copyMode is CopyMode_TruncateInsert)
                        {
                            var mode = (CopyMode_TruncateInsert)copyMode;
                            // truncate table before load
                            string cmdText = $"TRUNCATE TABLE {mode.TargetTable};";
                            using (SqlCommand cmd = new SqlCommand(cmdText, sqlConnection, transaction))
                            {
                                cmd.CommandTimeout = _config.TimeoutSecond;
                                cmd.ExecuteNonQuery();
                            }
                        }
                        else if (copyMode is CopyMode_DeleteInsert)
                        {
                            var mode = (CopyMode_DeleteInsert)copyMode;

                            if (string.IsNullOrEmpty(mode.DeleteCondition))
                            {
                                throw new ArgumentNullException(nameof(mode.DeleteCondition), $"The {mode.DeleteCondition} cannot be empty when Copy Mode is {nameof(CopyMode_DeleteInsert)}");
                            }

                            // delete table before load
                            string cmdText = $"DELETE FROM {mode.TargetTable} {mode.DeleteCondition};";
                            using (SqlCommand cmd = new SqlCommand(cmdText, sqlConnection, transaction))
                            {
                                cmd.CommandTimeout = _config.TimeoutSecond;
                                cmd.ExecuteNonQuery();
                            }
                        }

                        if (_copyMode.SourceData.Rows.Count > 0)
                        {
                            // load data into table
                            var bulkOptions = SqlBulkCopyOptions.TableLock | SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.KeepIdentity;
                            using (var sqlBulkCopy = new SqlBulkCopy(sqlConnection, bulkOptions, transaction))
                            {
                                // set timeout to 30 mins, in case large data
                                sqlBulkCopy.BulkCopyTimeout = _config.TimeoutSecond;
                                sqlBulkCopy.DestinationTableName = _copyMode.TargetTable;

                                if (_copyMode.ColumnMappings != null && _copyMode.ColumnMappings.Count > 0)
                                {
                                    foreach (var item in _copyMode.ColumnMappings)
                                    {
                                        sqlBulkCopy.ColumnMappings.Add(item.Key, item.Value);
                                    }
                                }

                                sqlBulkCopy.WriteToServer(_copyMode.SourceData);
                            }
                        }
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

        private void Upsert(CopyMode_InsertUpdate copyMode, SqlConnection sqlConnection, SqlTransaction transaction)
        {
            if (copyMode.PrimaryKeys == null || copyMode.PrimaryKeys.Count == 0)
            {
                throw new ArgumentNullException(nameof(copyMode.PrimaryKeys), "Primary Key is mandatory for UpdateInsert copy mode");
            }

            // create temp table
            string tempTable = $"#temp_{GetGuid()}";

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"CREATE TABLE {tempTable}(");

            var dataTable = copyMode.SourceData;
            int columnCount = dataTable.Columns.Count;

            // no action if source data has no record
            if (dataTable.Rows.Count == 0)
            {
                return;
            }

            var targetTableStructure = GetDbTableStructure(copyMode.TargetTable);

            for (int i = 0; i < columnCount; i++)
            {
                string s_col = dataTable.Columns[i].ColumnName;
                Console.WriteLine(s_col);
                string t_col = copyMode.ColumnMappings[s_col];

                var structure = targetTableStructure.First(p => p.COLUMN_NAME.Equals(t_col, StringComparison.CurrentCultureIgnoreCase));

                string dataType = "NVARCHAR(MAX)";
                switch (structure.DATA_TYPE)
                {
                    case "datetime":
                    case "date":
                        dataType = "datetime";
                        break;
                    case "bit":
                        dataType = "bit";
                        break;
                    default:
                        break;
                }

                string colum = $"{s_col} {dataType}";

                stringBuilder.Append($"{colum}");
                if (i != columnCount - 1)
                {
                    stringBuilder.Append(", ");
                }
            }
            stringBuilder.Append(");");

            string sql_create = stringBuilder.ToString();
            using (SqlCommand cmd = new SqlCommand(sql_create, sqlConnection, transaction))
            {
                cmd.CommandTimeout = _config.TimeoutSecond;
                cmd.ExecuteNonQuery();
            }

            // insert data into temp table
            var bulkOptions = SqlBulkCopyOptions.TableLock | SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.KeepIdentity;
            using (var sqlBulkCopy = new SqlBulkCopy(sqlConnection, bulkOptions, transaction))
            {
                // set timeout to 30 mins, in case large data
                sqlBulkCopy.BulkCopyTimeout = _config.TimeoutSecond;
                sqlBulkCopy.DestinationTableName = tempTable;

                if (copyMode.ColumnMappings != null && copyMode.ColumnMappings.Count > 0)
                {
                    foreach (var item in copyMode.ColumnMappings)
                    {
                        sqlBulkCopy.ColumnMappings.Add(item.Key, item.Key);
                    }
                }
                sqlBulkCopy.WriteToServer(copyMode.SourceData);
            }

            // do update & insert to target table, and drop temp table

            // build primary key sql
            string sql_key = string.Empty;
            int keys_count = copyMode.PrimaryKeys.Count;
            for (int i = 0; i < keys_count; i++)
            {
                string s_col = copyMode.PrimaryKeys[i];
                string t_col = copyMode.ColumnMappings.Values.Where(p => p.Equals(s_col)).First();
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
            string sql_upsert = $@"UPDATE T 
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
                            ";

            using (SqlCommand cmd = new SqlCommand(sql_upsert, sqlConnection, transaction))
            {
                cmd.CommandTimeout = _config.TimeoutSecond;
                cmd.ExecuteNonQuery();
            }
        }


        private class DbTableStructure
        {
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

            return table.Rows.OfType<DataRow>().Select(k =>
                  new DbTableStructure
                  {
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
