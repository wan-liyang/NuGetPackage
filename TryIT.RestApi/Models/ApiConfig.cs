using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using System.Text;

namespace TryIT.RestApi.Models
{
    /// <summary>
    /// configuration
    /// </summary>
    public class ApiConfig
    {
        /// <summary>
        /// the ready HttpClient with necessary information e.g. Proxy, Auth, Header
        /// </summary>
        public HttpClient HttpClient { get; set; }

        /// <summary>
        /// indicator whether enable retry (max 3 times) if respons status code is 
        /// <see cref="HttpStatusCode.GatewayTimeout"/>
        /// <see cref="HttpStatusCode.BadGateway"/>
        /// <see cref="HttpStatusCode.BadRequest"/>
        /// or timeout exception happen
        /// </summary>
        public bool EnableRetry { get; set; }
    }
}
