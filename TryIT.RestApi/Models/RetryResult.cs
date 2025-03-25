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
        /// timestamp of the retry
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// response message for each retry
        /// </summary>
        public ResultMessage Result { get; set; }

        /// <summary>
        /// exception for each retry
        /// </summary>
        public Exception Exception { get; set; }
    }

    /// <summary>
    /// retry attempt result
    /// </summary>
    public class ResultMessage
    {
        /// <summary>
        /// the status code of the response
        /// </summary>
        public string StatusCode { get; set; }

        /// <summary>
        /// the reason phrase of the response
        /// </summary>
        public string ReasonPhrase { get; set; }

        /// <summary>
        /// the uri of the request
        /// </summary>
        public Uri RequestUri { get; set; }
    }
}
