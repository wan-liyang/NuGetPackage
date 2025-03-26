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
        /// retry property for the action when meet the condition
        /// </summary>
        public RetryProperty RetryProperty { get; set; }
    }
}
