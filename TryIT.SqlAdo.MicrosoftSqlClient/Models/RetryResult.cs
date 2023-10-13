using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.SqlAdo.MicrosoftSqlClient.Models
{
    /// <summary>
    /// retry result
    /// </summary>
    public class RetryResult
    {
        /// <summary>
        /// number of retry
        /// </summary>
        public int AttemptNumber { get; set; }

        /// <summary>
        /// exception for each retry
        /// </summary>
        public Exception Exception { get; set; }
    }
}
