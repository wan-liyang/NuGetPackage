using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace TryIT.RestApi.Models
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
        /// response message for each retry
        /// </summary>
        public ResultMessage Result { get; set; }
        /// <summary>
        /// exception for each retry
        /// </summary>
        public Exception Exception { get; set; }
    }

    public class ResultMessage
    {
        public string StatusCode { get; set; }
        public string ReasonPhrase { get; set; }
        public Uri RequestUri { get; set; }
    }
}
