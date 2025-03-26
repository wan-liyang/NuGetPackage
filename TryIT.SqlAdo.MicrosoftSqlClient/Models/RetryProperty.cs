using System;
using System.Collections.Generic;

namespace TryIT.SqlAdo.MicrosoftSqlClient.Models
{
    /// <summary>
    /// retry property for the api
    /// </summary>
    public class RetryProperty
    {
        /// <summary>
        /// the exceptions that will be retried, this only applicable for GET method
        /// </summary>
        public List<RetryExceptionConfig> RetryExceptions { get; set; } = new List<RetryExceptionConfig>();

        /// <summary>
        /// number of retry times, default 3 times
        /// </summary>
        public int RetryCount { get; set; } = 3;

        /// <summary>
        /// delay between each retry, default 1 second
        /// </summary>
        public TimeSpan RetryDelay { get; set; }
    }

    /// <summary>
    /// exception config for retry
    /// </summary>
    public class RetryExceptionConfig
    {
        /// <summary>
        /// E.g., typeof(HttpRequestException)
        /// </summary>
        public Type ExceptionType { get; set; }

        /// <summary>
        /// E.g., "SSL connection"
        /// </summary>
        public string MessageKeyword { get; set; }
    }
}
