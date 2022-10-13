using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace TryIT.OracleDbConnector
{
    public class DbConnector
    {
        private string _connectionString;
        public DbConnector(string connectionString)
        {
            if(string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }
            _connectionString = connectionString;
        }

        /// <summary>
        /// get data as DataTable
        /// </summary>
        /// <param name="cmdText"></param>
        /// <returns></returns>
        public DataTable FetchDataTable(string cmdText)
        {
            DataTable dt = new DataTable();
            using(OracleConnection con = new OracleConnection(_connectionString))
            {
                con.Open();
                using(OracleCommand cmd = new OracleCommand(cmdText, con))
                {
                    OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                    adapter.Fill(dt);
                }
            }
            return dt;
        }

        /// <summary>
        /// run query, return effected row number
        /// </summary>
        /// <param name="cmdText"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(string cmdText)
        {
            using (OracleConnection con = new OracleConnection(_connectionString))
            {
                con.Open();
                using (OracleCommand cmd = new OracleCommand(cmdText, con))
                {
                    return cmd.ExecuteNonQuery();
                }
            }
        }
    }
}

