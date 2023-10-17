using System;
using System.Collections.Generic;
using System.Net;

namespace TryIT.RestApi.Models
{
    public class HttpClientConfig
    {
        public HttpClientConfig()
        {
            securityProtocolType = SecurityProtocolType.Tls12;
        }

        /// <summary>
        /// indicator whether enable retry (max 3 times) if respons status code is 
        /// <see cref="HttpStatusCode.GatewayTimeout"/>
        /// <see cref="HttpStatusCode.BadGateway"/>
        /// <see cref="HttpStatusCode.BadRequest"/>
        /// or timeout exception happen
        /// </summary>
        public bool EnableRetry { get; set; }

        public BasicAuth BasicAuth { get; set; }
        public ProxyConfig Proxy { get; set; }
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