using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace TryIT.RestApi.Models
{
    public class ProxyConfig
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
