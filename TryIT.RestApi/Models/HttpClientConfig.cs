using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace TryIT.RestApi.Models
{
    /// <summary>
    /// Delegate for logging HTTP request and response details.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public delegate Task HttpLogDelegate(HttpLogContext context);

    /// <summary>
    /// Http log context for logging request and response details.
    /// </summary>
    public class HttpLogContext
    {
        /// <summary>
        /// Unique identifier to correlate logs across a request chain.
        /// </summary>
        public string CorrelationId { get; set; }

        /// <summary>
        /// Indicates which stage of the HTTP lifecycle this log represents.
        /// </summary>
        public LogStage Stage { get; set; }

        /// <summary>
        /// The HTTP method (GET, POST, DELETE, etc.)
        /// </summary>
        public HttpMethod Method { get; set; }

        /// <summary>
        /// The request URL
        /// </summary>
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// The HTTP response (if available)
        /// </summary>
        public HttpResponseMessage Response { get; set; }

        /// <summary>
        /// The exception that occurred (if any)
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// The UTC time when the request started
        /// </summary>
        public DateTimeOffset StartTimeUtc { get; set; }

        /// <summary>
        /// The UTC time when the request completed (success or failure)
        /// </summary>
        public DateTimeOffset EndTimeUtc { get; set; }
    }

    /// <summary>
    /// Represents the stage of the HTTP request lifecycle for logging purposes.
    /// </summary>
    public enum LogStage
    {
        /// <summary>
        /// Occurs before the HTTP request is sent.
        /// Useful for logging request URL, headers, and payload.
        /// </summary>
        BeforeRequest,

        /// <summary>
        /// Occurs after a successful HTTP response is received.
        /// Useful for logging response status code, headers, and content.
        /// </summary>
        AfterResponse,

        /// <summary>
        /// Occurs when an exception is thrown during the request or response handling.
        /// Useful for logging errors, stack traces, and diagnostic context.
        /// </summary>
        OnError
    }


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

            RetryProperty = new RetryProperty
            {
                // defalt number of retry times to 3
                RetryCount = 3,
                // default delay between each retry to 1 second
                RetryDelay = TimeSpan.FromSeconds(1),
            };
        }

        /// <summary>
        /// httpClient to use for the request, if provided will use this httpClient and <see cref="Proxy"/> will be ignored
        /// </summary>
        public HttpClient HttpClient { get; set; }

        /// <summary>
        /// retry property for the request when meet the condition
        /// </summary>
        public RetryProperty RetryProperty { get; set; }

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

        /// <summary>
        /// Optional async logger delegate, allows external caller to log request/response.
        /// </summary>
        public HttpLogDelegate HttpLogDelegate { get; set; }
    }
}