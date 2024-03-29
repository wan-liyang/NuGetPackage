﻿using System.Data.SqlClient;
using System;
using System.Data;
using System.Linq;
using System.Text;

namespace TryIT.SqlAdoService
{
    /// <summary>
    /// operate database via objective SQL ADO method
    /// </summary>
    public class SqlAdoObject
    {
        private AdoConfig _config;

        /// <summary>
        /// init SQL ADO configuration, e.g. connection, timeout second
        /// </summary>
        /// <param name="config"></param>
        public SqlAdoObject(AdoConfig config)
        {
            _config = config;
        }
        private string m_ConnectionStr
        {
            get
            {
                if (_config == null)
                {
                    throw new NullReferenceException(nameof(_config));
                }
                if (string.IsNullOrEmpty(_config.ConnectionString))
                {
                    throw new NullReferenceException(nameof(_config.ConnectionString));
                }

                return _config.ConnectionString;
            }
        }
        private int m_CmdTimeout
        {
            get
            {
                return _config.TimeoutSecond;
            }
        }

        private void OpenConnection(IDbConnection dbConnection)
        {
            if (null == dbConnection)
            {
                throw new NullReferenceException($"dbConnection not instantiated.");
            }
            if (dbConnection.State != ConnectionState.Closed)
            {
                dbConnection.Close();
            }
            dbConnection.Open();
        }

        private SqlConnection Conn
        {
            get
            {
                return new SqlConnection(m_ConnectionStr);
            }
        }

        /// <summary>
        /// Get DataTable based on provided SqlCommand script, it can be query script or store procedure
        /// </summary>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public DataTable FetchDataTable(string cmdText, CommandType cmdType, params SqlParameter[] parameters)
        {
            if (string.IsNullOrEmpty(cmdText))
            {
                throw new ArgumentNullException(nameof(cmdText));
            }

            using (var conn = Conn)
            {
                OpenConnection(conn);
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = cmdText;
                    cmd.CommandType = cmdType;
                    cmd.CommandTimeout = m_CmdTimeout;
                    if (null != parameters && parameters.Count() > 0)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        /// <summary>
        /// Get DataSet based on provided SqlCommand script, it can be query script or store procedure
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="cmdType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public DataSet FetchDataSet(string cmdText, CommandType cmdType, params SqlParameter[] parameters)
        {
            if (string.IsNullOrEmpty(cmdText))
            {
                throw new ArgumentNullException(nameof(cmdText));
            }

            using (var conn = Conn)
            {
                OpenConnection(conn);
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = cmdText;
                    cmd.CommandType = cmdType;
                    cmd.CommandTimeout = m_CmdTimeout;
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
        /// Call Scalar Value Function, return the value, Function Name, must include schema,
        /// <para>SELECT schema.FunctionName(@parameter1, @parameter2, @parameter3, ...)</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="function"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public T FetchScalarFunction<T>(string function, params SqlParameter[] parameters)
        {
            using (var conn = Conn)
            {
                OpenConnection(conn);
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = m_CmdTimeout;

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

                    return SqlAdoExtension.ConvertValue<T>(result);
                }
            }
        }

        /// <summary>
        /// fetch table data by function. function name must include schema.
        /// <para>SELECT schema.FunctionName(@parameter1, @parameter2, @parameter3, ...)</para>
        /// </summary>
        /// <param name="function"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public DataTable FetchDataTableFunction(string function, params SqlParameter[] parameters)
        {
            using (var conn = Conn)
            {
                OpenConnection(conn);
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = m_CmdTimeout;

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
        /// <param name="cmdText"></param>
        /// <param name="cmdType"></param>
        /// <param name="parameters"></param>
        public int ExecuteNonQuery(string cmdText, CommandType cmdType, params SqlParameter[] parameters)
        {
            if (string.IsNullOrEmpty(cmdText))
            {
                throw new ArgumentNullException(nameof(cmdText));
            }

            using (var conn = Conn)
            {
                OpenConnection(conn);
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = cmdText;
                    cmd.CommandType = cmdType;
                    cmd.CommandTimeout = m_CmdTimeout;
                    if (null != parameters && parameters.Count() > 0)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }

                    return cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// For other MSSQL DB, Executes a Transact-SQL statement and returns the number of rows affected.
        /// </summary>
        /// <param name="connnectionString"></param>
        /// <param name="cmdText"></param>
        /// <param name="cmdType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public int ExecuteNonQuery_withConnection(string connnectionString, string cmdText, CommandType cmdType, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(connnectionString))
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = cmdText;
                    cmd.CommandType = cmdType;
                    cmd.CommandTimeout = m_CmdTimeout;
                    if (null != parameters && parameters.Count() > 0)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }

                    return cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Executes the query, and returns the first column of the first row in the result set returned by the query. Additional columns or rows are ignored.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmdText"></param>
        /// <param name="cmdType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public T ExecuteScalar<T>(string cmdText, CommandType cmdType, params SqlParameter[] parameters)
        {
            using (var conn = Conn)
            {
                OpenConnection(conn);
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = cmdText;
                    cmd.CommandType = cmdType;
                    cmd.CommandTimeout = m_CmdTimeout;
                    if (null != parameters && parameters.Count() > 0)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    object result = cmd.ExecuteScalar();

                    return SqlAdoExtension.ConvertValue<T>(result);
                }
            }
        }

        /// <summary>
        /// Sends the System.Data.SqlClient.SqlCommand.CommandText to the System.Data.SqlClient.SqlCommand.Connection, and builds a System.Data.SqlClient.SqlDataReader using one of the System.Data.CommandBehavior values.
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public SqlDataReader ExecuteReader(string cmdText, params SqlParameter[] parameters)
        {
            SqlConnection con = new SqlConnection(m_ConnectionStr);
            SqlCommand cmd = new SqlCommand(cmdText, con);
            cmd.CommandType = CommandType.Text;
            if (null != parameters && parameters.Count() > 0)
            {
                cmd.Parameters.AddRange(parameters);
            }

            return cmd.ExecuteReader(CommandBehavior.CloseConnection);
        }
    }
}
