namespace TryIT.SqlAdoService
{
    /// <summary>
    /// required configuration for SQL ADO
    /// </summary>
    public class AdoConfig
    {
        /// <summary>
        /// the connection string to database
        /// </summary>
        public string ConnectionString { get; set; }
        /// <summary>
        /// the timeout second for connection
        /// </summary>
        public int TimeoutSecond { get; set; }
    }
}
