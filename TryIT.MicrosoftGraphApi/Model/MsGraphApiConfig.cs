using System;
using System.Collections.Generic;
using System.Net;

namespace TryIT.MicrosoftGraphApi.Model
{
    /// <summary>
    /// Config information use for application perform graph api request
    /// </summary>
    public class MsGraphApiConfig : BaseApiConfig
    {
        /// <summary>
        /// bearer token for the request
        /// </summary>
        public string Token { get; set; }
    }
}
