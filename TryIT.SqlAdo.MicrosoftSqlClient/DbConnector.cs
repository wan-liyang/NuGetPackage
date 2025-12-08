using Microsoft.Data.SqlClient;
using Microsoft.Data.SqlClient.AlwaysEncrypted.AzureKeyVaultProvider;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
        private const string EXCEPTION_DATA_RETRY_ATTEMPTS = "RetryAttempts";
        private readonly ResiliencePipeline _pipeline;
        private readonly ConnectorConfig _config;
        
        private readonly List<RetryResult> _retryResults = new List<RetryResult>();
        
        /// <summary>
        /// retry results
        /// </summary>
        public List<RetryResult> RetryResults
        {
            get
            {
                return _retryResults;
            }
        }

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
                throw new ArgumentException($"{nameof(config.ConnectionString)} is null or empty");
            }
            _config = config;

            var Buider = GetBuilder(config.RetryProperty);

            if (Buider.EnableRetry)
            {
                _pipeline = new ResiliencePipelineBuilder()
                           .AddRetry(new RetryStrategyOptions
                           {
                               ShouldHandle = Buider.RetryBuilder,

                               Delay = config.RetryProperty.RetryDelay,
                               MaxRetryAttempts = config.RetryProperty.RetryCount,
                               BackoffType = DelayBackoffType.Constant,
                               OnRetry = args =>
                               {
                                   UpdateRetryResult(args);
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
        /// get retry builder based on retry property
        /// </summary>
        /// <param name="retryProperty"></param>
        /// <returns></returns>
        private static (bool EnableRetry, PredicateBuilder RetryBuilder) GetBuilder(RetryProperty retryProperty)
        {
            if (retryProperty == null)
            {
                return (false, null);
            }

            bool _isRetryEnabled = false;

            var builder = new PredicateBuilder();

            if (retryProperty.RetryExceptions != null && retryProperty.RetryExceptions.Any())
            {
                _isRetryEnabled = true;

                builder.Handle<Exception>((Func<Exception, bool>)(ex =>
                {
                    if (retryProperty.RetryExceptions.Any(
                            retryEx => retryEx.ExceptionType.IsInstanceOfType(ex)
                            && (
                                string.IsNullOrEmpty(retryEx.MessageKeyword)
                                || ex.Message.ToUpper().Contains(retryEx.MessageKeyword.ToUpper())
                                ))
                        )
                    {
                        return true;
                    }

                    return false;
                }));
            }

            return (_isRetryEnabled, builder);
        }

        private void UpdateRetryResult(OnRetryArguments<object> args)
        {
            _retryResults.Add(new RetryResult
            {
                AttemptNumber = args.AttemptNumber,
                Timestamp = DateTime.Now,
                Exception = args.Outcome.Exception
            });
        }

        /// <summary>
        /// add extra data into exception
        /// <para>Uri</para>
        /// <para>Method</para>
        /// <para>RetryResults</para>
        /// </summary>
        /// <param name="ex"></param>
        private void AddExcetionData(Exception ex)
        {
            if (RetryResults.Any())
            {
                ex.Data[EXCEPTION_DATA_RETRY_ATTEMPTS] = RetryResults;
            }
        }

        /// <summary>
        /// register Azure Key Vault column encryption key store provider
        /// <para>IMPORTANT: please call this method once only during program starting, otherwise may encounter 'key store providers cannot be set more than once.' error</para>
        /// </summary>
        /// <param name="azureKeyVaultProvider"></param>
        public static void RegisterColumnEncryptionKeyStore_AKV(AzureServicePrincipal azureKeyVaultProvider)
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
        /// open new SqlConnection
        /// </summary>
        /// <returns></returns>
        private SqlConnection OpenConection()
        {
            return OpenConectionAsync().GetAwaiter().GetResult();
        }

        private async Task<SqlConnection> OpenConectionAsync(CancellationToken cancellationToken = default)
        {
            var conn = new SqlConnection(_config.ConnectionString);

            if (!string.IsNullOrEmpty(_config.AccessToken))
            {
                conn.AccessToken = _config.AccessToken;
            }
            await conn.OpenAsync(cancellationToken);

            return conn;
        }

        /// <summary>
        /// fetch DataTable from <paramref name="sql"/>
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public DataTable FetchDataTable(string sql, CommandType commandType = CommandType.Text, SqlParameter[] parameters = null)
        {
            return FetchDataTableAsync(sql, commandType, parameters).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Asynchronously fetches a DataTable from the specified SQL command.
        /// </summary>
        /// <param name="sql">The SQL command text.</param>
        /// <param name="commandType">The type of the command.</param>
        /// <param name="parameters">The parameters for the command.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task representing the asynchronous operation, with a DataTable result.</returns>
        /// <returns></returns>
        public async Task<DataTable> FetchDataTableAsync(
            string sql,
            CommandType commandType,
            SqlParameter[] parameters = null,
            CancellationToken cancellationToken = default)
        {
            return await ExecuteAsync(sql, commandType, async (cmd, token) =>
            {
                return await Task.Run(() =>
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                });
            }, parameters, cancellationToken);
        }

        /// <summary>
        /// fetch DataSet from <paramref name="sql"/>
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public DataSet FetchDataSet(string sql, CommandType commandType, params SqlParameter[] parameters)
        {
            return FetchDataSetAsync(sql, commandType, parameters).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Asynchronously fetches a DataSet from the specified SQL command.
        /// </summary>
        /// <param name="sql">The SQL command text.</param>
        /// <param name="commandType">The type of the command.</param>
        /// <param name="parameters">The parameters for the command.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task representing the asynchronous operation, with a DataSet result.</returns>
        public async Task<DataSet> FetchDataSetAsync(
            string sql,
            CommandType commandType,
            SqlParameter[] parameters = null,
            CancellationToken cancellationToken = default)
        {
            return await ExecuteAsync(sql, commandType, async (cmd, token) =>
            {
                return await Task.Run(() =>
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataSet ds = new DataSet();
                        adapter.Fill(ds);
                        return ds;
                    }
                });
            }, parameters, cancellationToken);
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

            try
            {
                return _pipeline.Execute(exec =>
                    {
                        using (SqlConnection sqlConnection = OpenConection())
                        {
                            using (SqlCommand cmd = sqlConnection.CreateCommand())
                            {
                                cmd.CommandTimeout = _config.TimeoutSecond;
                                cmd.CommandType = CommandType.Text;

                                StringBuilder strBuilder = new StringBuilder("SELECT ");
                                strBuilder.Append(function);
                                if (null != parameters && parameters.Any())
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

                                return SqlHelper.ConvertValue<T>(result);
                            }
                        }
                    });
            }
            catch (Exception ex)
            {
                AddExcetionData(ex);
                throw;
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

            try
            {
                return _pipeline.Execute(exec =>
                    {
                        using (SqlConnection sqlConnection = OpenConection())
                        {
                            using (SqlCommand cmd = sqlConnection.CreateCommand())
                            {
                                cmd.CommandTimeout = _config.TimeoutSecond;
                                cmd.CommandType = CommandType.Text;

                                StringBuilder strBuilder = new StringBuilder("SELECT * FROM ");
                                strBuilder.Append(function);
                                strBuilder.Append("(");
                                if (null != parameters && parameters.Any())
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
            catch (Exception ex)
            {
                AddExcetionData(ex);
                throw;
            }
        }

        private async Task<TResult> ExecuteAsync<TResult>(
            string sql,
            CommandType commandType,
            Func<SqlCommand, CancellationToken, Task<TResult>> executor,
            SqlParameter[] parameters = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(sql))
            {
                throw new ArgumentNullException(nameof(sql));
            }

            try
            {
                return await _pipeline.ExecuteAsync(async exec =>
                {
                    using (SqlConnection sqlConnection = await OpenConectionAsync(cancellationToken))
                    {
                        using (SqlCommand cmd = sqlConnection.CreateCommand())
                        {
                            cmd.CommandTimeout = _config.TimeoutSecond;

                            cmd.CommandText = sql;
                            cmd.CommandType = commandType;
                            if (null != parameters && parameters.Any())
                            {
                                cmd.Parameters.AddRange(ClondParameters(parameters));
                            }

                            return await executor(cmd, cancellationToken);
                        }
                    }
                }, cancellationToken);
            }
            catch (Exception ex)
            {
                AddExcetionData(ex);
                throw;
            }
        }

        /// <summary>
        /// Executes a Transact-SQL statement and returns the number of rows affected.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        public int ExecuteNonQuery(string sql, CommandType commandType, params SqlParameter[] parameters)
        {
            return ExecuteAsync(sql, commandType, (cmd, token) => cmd.ExecuteNonQueryAsync(token), parameters).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Executes a Transact-SQL statement asynchronously and returns the number of rows affected.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<int> ExecuteNonQueryAsync(
            string sql, 
            CommandType commandType, 
            SqlParameter[] parameters = null,
            CancellationToken cancellationToken = default)
        {
            return await ExecuteAsync(sql, commandType, (cmd, token) => cmd.ExecuteNonQueryAsync(token), parameters, cancellationToken);
        }

        /// <summary>
        /// clond sql parameter, avoid "The SqlParameter is already contained by another SqlParameterCollection." error when retry execution
        /// </summary>
        /// <param name="originalParameters"></param>
        /// <returns></returns>
        private static SqlParameter[] ClondParameters(SqlParameter[] originalParameters)
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
        /// <param name="sql"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public T ExecuteScalar<T>(string sql, CommandType commandType, params SqlParameter[] parameters)
        {
            object result = ExecuteAsync(sql, commandType, (cmd, token) => cmd.ExecuteScalarAsync(token), parameters).GetAwaiter().GetResult();
            return SqlHelper.ConvertValue<T>(result);
        }

        /// <summary>
        /// Executes the query asynchronously and returns the first column of the first row in the result set returned by the query. Additional columns or rows are ignored.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<T> ExecuteScalarAsync<T>(
            string sql,
            CommandType commandType,
            SqlParameter[] parameters = null,
            CancellationToken cancellationToken = default)
        {
            object result = await ExecuteAsync(sql, commandType, (cmd, token) => cmd.ExecuteScalarAsync(token), parameters, cancellationToken);
            return SqlHelper.ConvertValue<T>(result);
        }

        /// <summary>
        /// Sends the System.Data.SqlClient.SqlCommand.CommandText to the System.Data.SqlClient.SqlCommand.Connection, and builds a System.Data.SqlClient.SqlDataReader using one of the System.Data.CommandBehavior values.
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public SqlDataReader ExecuteReader(string commandText, params SqlParameter[] parameters)
        {
            SqlConnection sqlConnection = OpenConection();
            SqlCommand cmd = new SqlCommand(commandText, sqlConnection);
            cmd.CommandType = CommandType.Text;
            if (null != parameters && parameters.Any())
            {
                cmd.Parameters.AddRange(ClondParameters(parameters));
            }

            return cmd.ExecuteReader(CommandBehavior.CloseConnection);
        }

        /// <summary>
        /// copy data from one source to multiple target table (within same target db)
        /// <para>the retry is not applicable for this, because this may invoke external action <see cref="CopyModeBase.PostAction"/></para>
        /// </summary>
        /// <param name="copyModes"></param>
        public void CopyData(List<ICopyMode> copyModes)
        {
            DoDataCopy(copyModes);
        }

        /// <summary>
        /// copy data from <see cref="CopyModeBase.SourceData"/> into <see cref="CopyModeBase.TargetTable"/>
        /// <para>the retry is not applicable for this, because this may invoke external action <see cref="CopyModeBase.PostAction"/></para>
        /// </summary>
        /// <param name="iCopyMode"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public void CopyData(ICopyMode iCopyMode)
        {
            DoDataCopy(new List<ICopyMode> { iCopyMode });
        }

        internal class DataCopyInfo
        {
            public ICopyMode CopyMode { get; set; }
            public List<DbTableStructure> TableStructures { get; set; }
        }

        private void DoDataCopy(List<ICopyMode> copyModes)
        {
            List<DataCopyInfo> tobeCopyModes = new List<DataCopyInfo>();

            // validate information
            foreach (var item in copyModes)
            {
                CopyModeBase _copyMode = item as CopyModeBase;

                if (_copyMode == null)
                {
                    throw new ArgumentException($"{nameof(_copyMode)} should not be null");
                }
                if (string.IsNullOrEmpty(_copyMode.TargetTable))
                {
                    throw new ArgumentException($"{nameof(_copyMode.TargetTable)} should not be null or empty");
                }
                if (_copyMode.TargetTable.Split('.').Length != 2)
                {
                    throw new ArgumentException($"TargetTable '{_copyMode.TargetTable}' must contains schema and table, e.g schema.table or [schema].[table]");
                }

                // get target table structure first, outside transaction, to avoid other script locked table
                var targetTableStructure = this.GetDbTableStructure(_copyMode.TargetTable);

                /*
                    reset column map info
                1. if provided: will use provided map to validate
                2. if not provided: will reset to source table column, then do validate

                this is to ensure source data are valid against target table, also source data map to correct target column, 
                e.g. source data column sequence may different with target table column sequence, if not provide column map, then program will use source table column as default map to ensure source column map to correct target column

                    */
                _copyMode.ColumnMappings = ResetColumnMap(_copyMode.SourceData, _copyMode.ColumnMappings);
                ValidateColumnMap(_copyMode.SourceData, targetTableStructure, _copyMode.ColumnMappings);

                tobeCopyModes.Add(new DataCopyInfo
                {
                    CopyMode = item,
                    TableStructures = targetTableStructure
                });
            }

            using (SqlConnection sqlConnection = OpenConection())
            {
                SqlTransaction transaction = null;
                try
                {
                    // put unique transaction name to avoid any conflict
                    string transName = SqlHelper.GetGuid();
                    transaction = sqlConnection.BeginTransaction(transName);

                    foreach (var item in tobeCopyModes)
                    {
                        ICopyMode iCopyMode = item.CopyMode;
                        List<DbTableStructure> targetTableStructure = item.TableStructures;

                        CopyModeBase _copyMode = iCopyMode as CopyModeBase;

                        if (!string.IsNullOrEmpty(_copyMode.PreScript))
                        {
                            try
                            {
                                using (SqlCommand cmd = new SqlCommand(_copyMode.PreScript, sqlConnection, transaction))
                                {
                                    cmd.CommandTimeout = _config.TimeoutSecond;
                                    cmd.ExecuteNonQuery();
                                }
                            }
                            catch (Exception ex)
                            {
                                throw new InvalidOperationException($"PreScript failed - {ex.Message}, {_copyMode.PreScript}", ex);
                            }
                        }

                        if (iCopyMode is CopyMode_InsertUpdate insertUpdateMode)
                        {
                            if (insertUpdateMode.SourceData.Rows.Count > 0)
                            {
                                insertUpdateMode.ColumnMappings = ResetColumnMap(insertUpdateMode.SourceData, insertUpdateMode.ColumnMappings);

                                var alwaysEncryptedColumns = this.GetAlwaysEncryptedColumns(insertUpdateMode.TargetTable);

                                if (alwaysEncryptedColumns != null && alwaysEncryptedColumns.Count > 0)
                                {
                                    try
                                    {
                                        Upsert_Encrypted(insertUpdateMode, sqlConnection, transaction, alwaysEncryptedColumns);
                                    }
                                    catch (Exception ex)
                                    {
                                        throw new InvalidOperationException($"Upsert_Encrypted failed - {ex.Message}", ex);
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        UpsertToDestination(insertUpdateMode, sqlConnection, transaction, targetTableStructure);
                                    }
                                    catch (Exception ex)
                                    {
                                        throw new InvalidOperationException($"Upsert failed - {ex.Message}", ex);
                                    }
                                }
                            }
                        }
                        else if (iCopyMode is UpdateCopyMode updateMode)
                        {
                            if (updateMode.SourceData.Rows.Count > 0)
                            {
                                updateMode.ColumnMappings = ResetColumnMap(updateMode.SourceData, updateMode.ColumnMappings);

                                try
                                {
                                    UpdateToDestination(updateMode, sqlConnection, transaction, targetTableStructure);
                                }
                                catch (Exception ex)
                                {
                                    throw new InvalidOperationException($"Update failed - {ex.Message}", ex);
                                }
                            }                            
                        }
                        else
                        {
                            if (iCopyMode is CopyMode_TruncateInsert truncateInsertMode)
                            {
                                // truncate table before load
                                string cmdText = $"TRUNCATE TABLE {SqlHelper.SqlWarpTable(truncateInsertMode.TargetTable)};";
                                using (SqlCommand cmd = new SqlCommand(cmdText, sqlConnection, transaction))
                                {
                                    cmd.CommandTimeout = _config.TimeoutSecond;
                                    cmd.ExecuteNonQuery();
                                }
                            }
                            else if (iCopyMode is CopyMode_DeleteInsert deleteInsertMode 
                                && string.IsNullOrEmpty(deleteInsertMode.PreScript))
                            {
                                // PreScript is mandatory for DeleteInsert
                                throw new ArgumentException($"The {deleteInsertMode.PreScript} cannot be empty when Copy Mode is {nameof(CopyMode_DeleteInsert)}");
                            }

                            try
                            {
                                BulkCopy(_copyMode.SourceData, _copyMode.TargetTable, targetTableStructure, _copyMode.ColumnMappings, sqlConnection, transaction);
                            }
                            catch (Exception ex)
                            {
                                throw new InvalidOperationException($"BulkCopy failed - {ex.Message}", ex);
                            }
                        }

                        if (!string.IsNullOrEmpty(_copyMode.PostScript))
                        {
                            try
                            {
                                using (SqlCommand cmd = new SqlCommand(_copyMode.PostScript, sqlConnection, transaction))
                                {
                                    cmd.CommandTimeout = _config.TimeoutSecond;
                                    cmd.ExecuteNonQuery();
                                }
                            }
                            catch (Exception ex)
                            {
                                throw new InvalidOperationException($"PostScript failed - {ex.Message}, {_copyMode.PostScript} ", ex);
                            }
                        }

                        // if PostAction is not null, execute the action before commit transaction
                        if (_copyMode.PostAction != null)
                        {
                            try
                            {
                                _copyMode.PostAction(sqlConnection, transaction);
                            }
                            catch (Exception ex)
                            {
                                throw new InvalidOperationException($"PostAction failed - {ex.Message}, {_copyMode.PostAction}", ex);
                            }
                        }
                    }

                    transaction.Commit();
                }
                catch(Exception ex)
                {
                    try
                    {
                        // check connection state to avoid error "This SqlTransaction has completed; it is no longer usable."
                        if (transaction != null && transaction.Connection != null && transaction.Connection.State == ConnectionState.Open)
                        {
                            transaction.Rollback();
                        }
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

        /// <summary>
        /// get actual column name present in database, avoid case-sensitive issue
        /// </summary>
        /// <param name="tableStructures"></param>
        /// <param name="mapValue"></param>
        /// <returns></returns>
        private static string GetTargetColumn(List<DbTableStructure> tableStructures, string mapValue)
        {
            var table = tableStructures.FirstOrDefault(p => p.COLUMN_NAME.Equals(mapValue, StringComparison.CurrentCultureIgnoreCase));
            if (table == null)
            {
                throw new ArgumentException($"column '{mapValue}' not found in target table");
            }
            return table.COLUMN_NAME;
        }

        /// <summary>
        /// reset ColumnMap if not provide, default to source table column
        /// </summary>
        /// <param name="sourceData"></param>
        /// <param name="columnMap"></param>
        /// <returns></returns>
        private static Dictionary<string, string> ResetColumnMap(DataTable sourceData, Dictionary<string, string> columnMap)
        {
            if (columnMap != null && columnMap.Count > 0)
            {
                return columnMap
                    .Where(p => !string.IsNullOrEmpty(p.Key) && !string.IsNullOrEmpty(p.Value))
                    .ToDictionary(x => x.Key, x => x.Value);
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
        private static void ValidateColumnMap(DataTable sourceTable, List<DbTableStructure> targetTableStructure, Dictionary<string, string> columnMap)
        {
            if (columnMap != null && columnMap.Count > 0)
            {
                // validate column mapping appear in source table
                ValidateColumnMap_Source(sourceTable, columnMap);

                // validate column mapping appear in target table
                ValidateColumnMap_Target(targetTableStructure, columnMap);
            }
        }

        private static void ValidateColumnMap_Source(DataTable sourceTable, Dictionary<string, string> columnMap)
        {
            List<string> sourceColumns = new List<string>();
            foreach (DataColumn item in sourceTable.Columns)
            {
                sourceColumns.Add(item.ColumnName);
            }
            var notExists = columnMap.Where(map => !sourceColumns.Exists(s => s.Equals(map.Key, StringComparison.OrdinalIgnoreCase))).Select(p => p.Key).ToList();
            if (notExists != null && notExists.Count > 0)
            {
                throw new ArgumentException($"column map not found in source data table: {string.Join(", ", notExists)}");
            }
        }

        private static void ValidateColumnMap_Target(List<DbTableStructure> targetTableStructure, Dictionary<string, string> columnMap)
        {
            string targetTable = targetTableStructure[0].TABLE_NAME;

            var notExists = columnMap.Where(map => !targetTableStructure.Exists(t => t.COLUMN_NAME.Equals(map.Value, StringComparison.OrdinalIgnoreCase))).Select(p => p.Value).ToList();
            if (notExists != null && notExists.Count > 0)
            {
                throw new ArgumentException($"column map not found in target database, table: {targetTable}, column: {string.Join(", ", notExists)}");
            }
        }


        #region do Update
        /// <summary>
        /// perform insert or update action, 1) write data into temp table, 2) insert or update to target table join with temp table, 3) delete temp table
        /// </summary>
        /// <param name="copyMode"></param>
        /// <param name="sqlConnection"></param>
        /// <param name="transaction"></param>
        /// <param name="targetTableStructure"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        private void UpdateToDestination(UpdateCopyMode copyMode, SqlConnection sqlConnection, SqlTransaction transaction, List<DbTableStructure> targetTableStructure)
        {
            if (copyMode.PrimaryKeys == null || copyMode.PrimaryKeys.Count == 0)
            {
                throw new ArgumentException("Primary Key is mandatory for UpdateInsert copy mode");
            }

            // write data into temp table
            string tempTable = WriteDataIntoTempTable(copyMode, sqlConnection, transaction, targetTableStructure);

            // do update & insert to target table, and drop temp table
            // build primary key sql
            string sql_key = string.Empty;
            int keys_count = copyMode.PrimaryKeys.Count;
            for (int i = 0; i < keys_count; i++)
            {
                string t_col = copyMode.PrimaryKeys[i];
                string s_col = copyMode.ColumnMappings.First(p => p.Value.Equals(t_col, StringComparison.CurrentCultureIgnoreCase)).Key;

                if (string.IsNullOrEmpty(t_col))
                {
                    throw new ArgumentException($"Configured primay key column [{s_col}] has no corresponding column in target table {copyMode.TargetTable}");
                }

                sql_key += $"S.{SqlHelper.SqlWarpColumn(s_col)} = T.{SqlHelper.SqlWarpColumn(t_col)}";

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

                sql_update += $"T.{SqlHelper.SqlWarpColumn(t_col)} = S.{SqlHelper.SqlWarpColumn(s_col)},";
            }
            sql_update = sql_update.TrimEnd(',');

            if (!string.IsNullOrEmpty(copyMode.TimestampColumn))
            {
                sql_update += $", {SqlHelper.SqlWarpColumn(copyMode.TimestampColumn)} = GETDATE()";
            }

            // build update sql
            string warppedTable = SqlHelper.SqlWarpTable(copyMode.TargetTable);

            string sql_upsert = $@"
                            UPDATE T 
                            SET {sql_update}
                            FROM {warppedTable} T
                            INNER JOIN {tempTable} S ON {sql_key};

                            DROP TABLE {tempTable};
                            ";

            // if target table has identity column, then check the column map has that column or not, if has, then warp with IdentityInsert, otherwise not need warp
            string identityColumn = this.GetIdentityColumnName(copyMode.TargetTable);
            if (!string.IsNullOrEmpty(identityColumn))
            {
                var map = copyMode.ColumnMappings.Where(p => p.Value.Equals(identityColumn, StringComparison.CurrentCultureIgnoreCase)).ToList();
                if (map != null && map.Count > 0)
                {
                    sql_upsert = SqlHelper.SqlWarpIdentityInsert(warppedTable, sql_upsert);
                }
            }

            using (SqlCommand cmd = new SqlCommand(sql_upsert, sqlConnection, transaction))
            {
                cmd.CommandTimeout = _config.TimeoutSecond;
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region do Update & Insert
        /// <summary>
        /// perform insert or update action, 1) write data into temp table, 2) insert or update to target table join with temp table, 3) delete temp table
        /// </summary>
        /// <param name="copyMode"></param>
        /// <param name="sqlConnection"></param>
        /// <param name="transaction"></param>
        /// <param name="targetTableStructure"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        private void UpsertToDestination(CopyMode_InsertUpdate copyMode, SqlConnection sqlConnection, SqlTransaction transaction, List<DbTableStructure> targetTableStructure)
        {
            if (copyMode.PrimaryKeys == null || copyMode.PrimaryKeys.Count == 0)
            {
                throw new ArgumentException("Primary Key is mandatory for UpdateInsert copy mode");
            }

            // write data into temp table
            string tempTable = WriteDataIntoTempTable(copyMode, sqlConnection, transaction, targetTableStructure);

            // do update & insert to target table, and drop temp table
            // build primary key sql
            string sql_key = string.Empty;
            int keys_count = copyMode.PrimaryKeys.Count;
            for (int i = 0; i < keys_count; i++)
            {
                string t_col = copyMode.PrimaryKeys[i];
                string s_col = copyMode.ColumnMappings.First(p => p.Value.Equals(t_col, StringComparison.CurrentCultureIgnoreCase)).Key;

                if (string.IsNullOrEmpty(t_col))
                {
                    throw new ArgumentException($"Configured primay key column [{s_col}] has no corresponding column in target table {copyMode.TargetTable}");
                }

                sql_key += $"S.{SqlHelper.SqlWarpColumn(s_col)} = T.{SqlHelper.SqlWarpColumn(t_col)}";

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

                sql_update += $"T.{SqlHelper.SqlWarpColumn(t_col)} = S.{SqlHelper.SqlWarpColumn(s_col)},";
            }
            sql_update = sql_update.TrimEnd(',');

            if (!string.IsNullOrEmpty(copyMode.TimestampColumn))
            {
                sql_update += $", {SqlHelper.SqlWarpColumn(copyMode.TimestampColumn)} = GETDATE()";
            }

            // build select & insert columns sql
            string sql_source_col = string.Join(", ", SqlHelper.SqlWarpColumn(copyMode.ColumnMappings.Keys));
            string sql_target_col = string.Join(", ", SqlHelper.SqlWarpColumn(copyMode.ColumnMappings.Values));

            // build update and insert sql
            string warppedTable = SqlHelper.SqlWarpTable(copyMode.TargetTable);

            string sql_upsert = $@"
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

                            DROP TABLE {tempTable};
                            ";

            // if target table has identity column, then check the column map has that column or not, if has, then warp with IdentityInsert, otherwise not need warp
            string identityColumn = this.GetIdentityColumnName(copyMode.TargetTable);
            if (!string.IsNullOrEmpty(identityColumn))
            {
                var map = copyMode.ColumnMappings.Where(p => p.Value.Equals(identityColumn, StringComparison.CurrentCultureIgnoreCase)).ToList();
                if (map != null && map.Count > 0)
                {
                    sql_upsert = SqlHelper.SqlWarpIdentityInsert(warppedTable, sql_upsert);
                }                
            }

            using (SqlCommand cmd = new SqlCommand(sql_upsert, sqlConnection, transaction))
            {
                cmd.CommandTimeout = _config.TimeoutSecond;
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

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

        #region do Update & Insert for tables contains AlwaysEncrypted enabled
        /// <summary>
        /// do update and insert for table has AlwaysEncrypted enabled
        /// </summary>
        /// <param name="copyMode"></param>
        /// <param name="sqlConnection"></param>
        /// <param name="transaction"></param>
        /// <param name="alwaysEncryptedColumns"></param>
        private void Upsert_Encrypted(CopyMode_InsertUpdate copyMode, SqlConnection sqlConnection, SqlTransaction transaction, List<AlwaysEncryptedColumn> alwaysEncryptedColumns)
        {
            if (copyMode.PrimaryKeys == null || copyMode.PrimaryKeys.Count == 0)
            {
                throw new ArgumentException("Primary Key is mandatory for UpdateInsert copy mode");
            }

            ConsoleLog("upsert with always encrypted started");

            string sql_where = "";
            string sql_set = "";
            string sql_insert_columns = "";
            string sql_insert_params = "";

            for (int i = 0; i < copyMode.PrimaryKeys.Count; i++)
            {
                string t_col = copyMode.PrimaryKeys[i];

                if (i > 0)
                {
                    sql_where += " AND ";
                }
                sql_where += $"{SqlHelper.SqlWarpColumn(t_col)} = @{SqlHelper.SqlParamName(t_col)}";

                // add primary key column into insert statement
                string col = SqlHelper.SqlWarpColumn(t_col);
                string param = SqlHelper.SqlParamName(t_col);
                sql_insert_columns += $"{col},";
                sql_insert_params += $"@{param},";
            }

            var tobeUpdateColumns = copyMode.ColumnMappings.Where(p => !copyMode.PrimaryKeys.Any(k => k.Equals(p.Value, StringComparison.CurrentCultureIgnoreCase))).ToDictionary(x => x.Key, x => x.Value);

            foreach (var item in tobeUpdateColumns)
            {
                string col = SqlHelper.SqlWarpColumn(item.Value);
                string param = SqlHelper.SqlParamName(item.Value);

                // add update column into update statement
                sql_set += $"{col} = @{SqlHelper.SqlParamName(item.Value)},";

                // add update column into insert statement
                sql_insert_columns += $"{col},";
                sql_insert_params += $"@{param},";
            }
            sql_set = sql_set.TrimEnd(',');
            sql_insert_columns = sql_insert_columns.TrimEnd(',');
            sql_insert_params = sql_insert_params.TrimEnd(',');

            string warppedTable = SqlHelper.SqlWarpTable(copyMode.TargetTable);
            string sql = $"IF EXISTS (SELECT 1 FROM {warppedTable} WHERE {sql_where}) BEGIN UPDATE {warppedTable} SET {sql_set} WHERE {sql_where} END ELSE BEGIN INSERT INTO {warppedTable}({sql_insert_columns}) VALUES ({sql_insert_params}) END;";

            // if target table has identity column, then check the column map has that column or not, if has, then warp with IdentityInsert, otherwise not need warp
            string identityColumn = this.GetIdentityColumnName(copyMode.TargetTable);
            if (!string.IsNullOrEmpty(identityColumn))
            {
                var map = copyMode.ColumnMappings.Where(p => p.Value.Equals(identityColumn, StringComparison.CurrentCultureIgnoreCase)).ToList();

                if (map != null && map.Count > 0)
                {
                    sql = SqlHelper.SqlWarpIdentityInsert(warppedTable, sql);
                }
            }

            /*
             * below assign value may cause deadlock issue
             * sqlCommand.CommandText = sql;
             * sqlCommand.Connection = sqlConnection; 
             * sqlCommand.Transaction = transaction;             
             */
            using (SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection, transaction))
            {
                sqlCommand.CommandType = CommandType.Text;

                int row = 0;

                for (int i = 0; i < copyMode.SourceData.Rows.Count; i++)
                {
                    for (int k = 0; k < copyMode.PrimaryKeys.Count; k++)
                    {
                        string t_col = copyMode.PrimaryKeys[k];
                        string s_col = copyMode.ColumnMappings.First(p => p.Value.Equals(t_col, StringComparison.CurrentCultureIgnoreCase)).Key;
                        AlwaysEncryptedColumn colEncrypt = alwaysEncryptedColumns.FirstOrDefault(p => p.ColumnName.Equals(t_col, StringComparison.CurrentCultureIgnoreCase));

                        var param = SqlHelper.GetParameter(t_col, copyMode.SourceData.Rows[i][s_col], colEncrypt);

                        sqlCommand.Parameters.Add(param);
                    }

                    foreach (var item in tobeUpdateColumns)
                    {
                        string t_col = item.Value;
                        string s_col = item.Key;
                        AlwaysEncryptedColumn colEncrypt = alwaysEncryptedColumns.FirstOrDefault(p => p.ColumnName.Equals(t_col, StringComparison.CurrentCultureIgnoreCase));

                        var param = SqlHelper.GetParameter(item.Value, copyMode.SourceData.Rows[i][s_col], colEncrypt);

                        sqlCommand.Parameters.Add(param);
                    }

                    sqlCommand.ExecuteNonQuery();

                    sqlCommand.Parameters.Clear();

                    row++;
                    if (row % 2000 == 0)
                    {
                        ConsoleLog($"{row} exec posted");
                    }
                }

                ConsoleLog($"{copyMode.SourceData.Rows.Count} exec posted");
            }
            ConsoleLog("upsert with always encrypted completed");
        }

        /// <summary>
        /// get always encrypted column information, return null if not exists
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="alwaysEncryptedColumns"></param>
        /// <returns></returns>
        private static AlwaysEncryptedColumn GetAlwaysEncryptedColumn(string columnName, List<AlwaysEncryptedColumn> alwaysEncryptedColumns)
        {
            return alwaysEncryptedColumns.FirstOrDefault(p => p.ColumnName.Equals(columnName));
        }

        private static void PostIntoDb(SqlConnection sqlConnection, SqlTransaction sqlTransaction, string sql, SqlParameter[] sqlParameters)
        {
            using (SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection, sqlTransaction))
            {
                sqlCommand.CommandTimeout = 10 * 60;
                sqlCommand.Parameters.Clear();
                sqlCommand.Parameters.AddRange(sqlParameters);
                sqlCommand.ExecuteNonQuery();
            }
        }
        #endregion

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

        private static void ConsoleLog(string message)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff");
            Console.WriteLine($"[{timestamp}] {message}");
        }
    }
}
