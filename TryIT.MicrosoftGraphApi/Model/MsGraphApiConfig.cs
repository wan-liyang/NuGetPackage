﻿namespace TryIT.MicrosoftGraphApi.Model
{
    /// <summary>
    /// Config information use for application perform graph api request
    /// </summary>
    public class MsGraphApiConfig
    {
        public string Token { get; set; }

        /// <summary>
        /// determine request timeout second, default is 100 seconds, https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient.timeout?view=net-7.0
        /// </summary>
        public double TimeoutSecond { get; set; }

        public Utility.ProxyModel Proxy { get; set; }
    }
}
