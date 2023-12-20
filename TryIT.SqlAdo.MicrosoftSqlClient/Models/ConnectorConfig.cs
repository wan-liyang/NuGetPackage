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
        /// indicator to enable retry
        /// </summary>
        internal bool EnableRetry
        {
            get
            {
                return RetryOn.SqlTimeout || RetryOn.EstablishConnection || RetryOn.Deadlock;
            }
        }

        /// <summary>
        /// indicate need retry on what error
        /// </summary>
        public RetryOn RetryOn { get; set; }
    }

    /// <summary>
    /// define scenario need retry
    /// </summary>
    public class RetryOn
    {
        /// <summary>
        /// indicate retry on Sql Timeout exception, <see cref="SqlException.Number"/> == -2
        /// </summary>
        public bool SqlTimeout { get; set; }

        /// <summary>
        /// indicate retry on network error cannot establish connection
        /// </summary>
        public bool EstablishConnection { get; set; }

        /// <summary>
        /// message for network error
        /// </summary>
        internal string EstablishConnnectionErrorMessage { get; } = "A network-related or instance-specific error occurred while establishing a connection to SQL Server";

        /// <summary>
        /// indicate retry on deadlock error
        /// </summary>
        public bool Deadlock { get; set; }

        /// <summary>
        /// deadlock error message
        /// </summary>
        internal string DeadlockErrorMessage { get; } = "communication buffer resources with another process and has been chosen as the deadlock victim. Rerun the transaction.";
    }
}
