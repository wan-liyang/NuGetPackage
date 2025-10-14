using System;
using System.Collections.Generic;
using System.Text;
using TryIT.RestApi.Models;

namespace TryIT.MicrosoftGraphApi.Model
{
    /// <summary>
    /// base api config property
    /// </summary>
    public class BaseApiConfig
    {
        /// <summary>
        /// determine request timeout second, default is 100 seconds, https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient.timeout?view=net-7.0
        /// </summary>
        public double TimeoutSecond { get; set; }

        /// <summary>
        /// proxy for the request
        /// </summary>
        public Utility.ProxyModel Proxy { get; set; }

        /// <summary>
        /// retry mechanism for the request
        /// </summary>
        public RetryProperty RetryProperty { get; set; }

        /// <summary>
        /// logging delegate for http request and response
        /// </summary>
        public HttpLogDelegate HttpLogDelegate { get; set; }
    }
}
