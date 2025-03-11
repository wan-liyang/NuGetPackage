using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace TryIT.RestApi.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class HttpClientConfig
    {
        /// <summary>
        /// initialize the HttpClientConfig with defualt values
        /// </summary>
        public HttpClientConfig()
        {
            securityProtocolType = SecurityProtocolType.Tls12;

            // defalt number of retry times to 3
            RetryCount = 3;

            // default delay between each retry to 1 second
            RetryDelay = TimeSpan.FromSeconds(1);
        }

        /// <summary>
        /// httpClient to use for the request, if provided will use this httpClient and <see cref="Proxy"/> will be ignored
        /// </summary>
        public HttpClient HttpClient { get; set; }

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

        /// <summary>
        /// basic auth for the request
        /// </summary>
        public BasicAuth BasicAuth { get; set; }

        /// <summary>
        /// proxy for the request, this will not be use if the <see cref="HttpClient"/> is set
        /// </summary>
        public ProxyConfig Proxy { get; set; }

        /// <summary>
        /// headers to add to the request
        /// </summary>
        public Dictionary<string, string> Headers { get; set; }

        /// <summary>
        /// determine request timeout second, default is 100 seconds, https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient.timeout?view=net-7.0
        /// </summary>
        public double TimeoutSecond { get; set; }

        /// <summary>
        /// default use <see cref="SecurityProtocolType.Tls12"/> for more secure
        /// </summary>
        public SecurityProtocolType securityProtocolType { get; set; }
    }
}