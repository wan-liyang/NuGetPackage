using System;
using System.Net;

namespace TryIT.RestApi
{
	public class HttpClientConfig
	{
		public HttpClientConfig()
        {
            securityProtocolType = SecurityProtocolType.Tls12;
        }

        public WebProxyInfo WebProxy { get; set; }

        /// <summary>
        /// determine request timeout second, default is 100 seconds, https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient.timeout?view=net-7.0
        /// </summary>
        public double TimeoutSecond { get; set; }

        /// <summary>
        /// default use <see cref="SecurityProtocolType.Tls12"/> for more secure
        /// </summary>
        public SecurityProtocolType securityProtocolType { get; set; }


        public class WebProxyInfo
        {
            /// <summary>
            /// Url for proxy server
            /// </summary>
            public string Url { get; set; }

            /// <summary>
            /// Username for connect to proxy, if empty will set <see cref="WebProxy.UseDefaultCredentials"/> to true
            /// </summary>
            public string Username { get; set; }
            /// <summary>
            /// Password for connect to proxy
            /// </summary>
            public string Password { get; set; }
        }
    }
}

