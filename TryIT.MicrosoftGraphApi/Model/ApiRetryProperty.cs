using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace TryIT.MicrosoftGraphApi.Model
{
    /// <summary>
    /// retry property for the api
    /// </summary>
    public class ApiRetryProperty
    {

        /// <summary>
        /// the status codes that will be retried
        /// </summary>
        public HashSet<HttpStatusCode> RetryStatusCodes { get; set; }

        /// <summary>
        /// number of retry times, default 3 times
        /// </summary>
        public int RetryCount { get; set; }

        /// <summary>
        /// delay between each retry, default 1 second
        /// </summary>
        public TimeSpan RetryDelay { get; set; }
    }
}
