using Microsoft.Data.SqlClient;

namespace TryIT.SqlAdo.MicrosoftSqlClient.Models
{
    /// <summary>
    /// database connector configuration
    /// </summary>
    public class ConnectorConfig
    {
        /// <summary>
        /// the connection string to database
        /// </summary>
        public string ConnectionString { get; set; }

        private int? second;
        /// <summary>
        /// the timeout second for connection, default 10 minutes
        /// </summary>
        public int TimeoutSecond
        {
            get
            {
                if (second.HasValue)
                {
                    return second.Value;
                }
                return 10 * 60;
            }
            set
            {
                second = value;
            }
        }

        /// <summary>
        /// indicator to enable retry for timeout exception
        /// </summary>
        public bool EnableRetry { get; set; }
    }
}
