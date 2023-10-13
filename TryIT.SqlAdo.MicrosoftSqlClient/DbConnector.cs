using Microsoft.Data.SqlClient;
using Microsoft.Data.SqlClient.AlwaysEncrypted.AzureKeyVaultProvider;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
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
        /// indicator whether column encryption key store has been register
        /// </summary>
        private static bool IsRegistered { get; set; }

        /// <summary>
        /// register Azure Key Vault column encryption key store provider
        /// </summary>
        /// <param name="azureKeyVaultProvider"></param>
        public void RegisterColumnEncryptionKeyStore_AKV(AzureKeyVaultProvider azureKeyVaultProvider)
        {
            // [fix issue] Key store providers cannot be set more than once.
            if (IsRegistered)
            {
                return;
            }

            var credential = AzureHelper.GetClientSecretCredential(azureKeyVaultProvider);
            SqlColumnEncryptionAzureKeyVaultProvider akvProvider = new SqlColumnEncryptionAzureKeyVaultProvider(credential);

            // Register AKV provider
            SqlConnection.RegisterColumnEncryptionKeyStoreProviders(customProviders: new Dictionary<string, SqlColumnEncryptionKeyStoreProvider>(capacity: 1, comparer: StringComparer.OrdinalIgnoreCase)
                {
                    { SqlColumnEncryptionAzureKeyVaultProvider.ProviderName, akvProvider}
                });
            IsRegistered = true;
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
        /// copy data from <see cref="CopyDataModel.SourceData"/> into <see cref="CopyDataModel.TargetTable"/>
        /// <para>the column map is case-sensitive on source column and destination column</para>
        /// </summary>
        /// <param name="copyDataModel"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public void CopyData(CopyDataModel copyDataModel)
        {
            if (copyDataModel == null)
            {
                throw new ArgumentNullException(nameof(copyDataModel));
            }
            if (string.IsNullOrEmpty(copyDataModel.TargetTable))
            {
                throw new ArgumentNullException(nameof(copyDataModel.TargetTable));
            }
            if (copyDataModel.TargetTable.Split('.').Length != 2)
            {
                throw new InvalidOperationException($"TargetTable '{copyDataModel.TargetTable}' must contains schema and table, e.g schema.table or [schema].[table]");
            }

            using (SqlConnection sqlConnection = new SqlConnection(_config.ConnectionString))
            {
                sqlConnection.Open();

                SqlTransaction transaction = null;
                try
                {
                    // create a Guid that doesn't contain any numbers and dash
                    // before: 51e3aaa4-6ff6-475a-8f0f-78cac597b6c3
                    // after: FBVDRRREGWWGEHFRIWAWHITRTFJHSGTD
                    string transName = string.Concat(Guid.NewGuid().ToString("N").Select(c => (char)(c + 17))).ToUpper();

                    // put unique transaction name to avoid any conflict
                    transaction = sqlConnection.BeginTransaction(transName);

                    if (!string.IsNullOrEmpty(copyDataModel.PreScript))
                    {
                        using (SqlCommand cmd = new SqlCommand(copyDataModel.PreScript, sqlConnection, transaction))
                        {
                            cmd.CommandTimeout = _config.TimeoutSecond;
                            cmd.ExecuteNonQuery();
                        }
                    }

                    if (copyDataModel.CopyMode == CopyMode.TRUNCATE_INSERT)
                    {
                        // truncate table before load
                        string cmdText = $"TRUNCATE TABLE {copyDataModel.TargetTable};";
                        using (SqlCommand cmd = new SqlCommand(cmdText, sqlConnection, transaction))
                        {
                            cmd.CommandTimeout = _config.TimeoutSecond;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    else if (copyDataModel.CopyMode == CopyMode.DELETE_INSERT)
                    {
                        if (string.IsNullOrEmpty(copyDataModel.DeleteCondition))
                        {
                            throw new ArgumentNullException(nameof(copyDataModel.DeleteCondition), $"The {copyDataModel.DeleteCondition} cannot be empty when Copy Mode is {CopyMode.DELETE_INSERT}");
                        }

                        // delete table before load
                        string cmdText = $"DELETE FROM {copyDataModel.TargetTable} {copyDataModel.DeleteCondition};";
                        using (SqlCommand cmd = new SqlCommand(cmdText, sqlConnection, transaction))
                        {
                            cmd.CommandTimeout = _config.TimeoutSecond;
                            cmd.ExecuteNonQuery();
                        }
                    }

                    // load data into table
                    var bulkOptions = SqlBulkCopyOptions.TableLock | SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.KeepIdentity;
                    using (var sqlBulkCopy = new SqlBulkCopy(sqlConnection, bulkOptions, transaction))
                    {
                        // set timeout to 30 mins, in case large data
                        sqlBulkCopy.BulkCopyTimeout = _config.TimeoutSecond;
                        sqlBulkCopy.DestinationTableName = copyDataModel.TargetTable;

                        if (copyDataModel.ColumnMappings != null && copyDataModel.ColumnMappings.Count > 0)
                        {
                            foreach (var item in copyDataModel.ColumnMappings)
                            {
                                sqlBulkCopy.ColumnMappings.Add(item.Key, item.Value);
                            }
                        }

                        sqlBulkCopy.WriteToServer(copyDataModel.SourceData);
                    }

                    if (!string.IsNullOrEmpty(copyDataModel.PostScript))
                    {
                        using (SqlCommand cmd = new SqlCommand(copyDataModel.PostScript, sqlConnection, transaction))
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
    }
}
