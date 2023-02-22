using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.MicrosoftGraphService.Model
{
    /// <summary>
    /// Config information use for application perform graph api request
    /// </summary>
    public class MsGraphApiConfig
    {
        public string Proxy_Url { get; set; }
        public string Proxy_Username { get; set; }
        public string Proxy_Password { get; set; }

        public string Token { get; set; }

        /// <summary>
        /// determine request timeout second, default is 100 seconds, https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient.timeout?view=net-7.0
        /// </summary>
        public double TimeoutSecond { get; set; }
    }
}
