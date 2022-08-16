using System;
using System.Collections.Generic;
using System.Text;

namespace MicrosoftGraphService.Config
{
    /// <summary>
    /// Config information use for application obtain Token
    /// </summary>
    public class MsGraphGetTokenConfig
    {
        public string OAuth_AuthorizeUrl { get; set; }
        public string OAuth_TenantId { get; set; }
        public string OAuth_ClientId { get; set; }
        public string OAuth_RedirectUrl { get; set; }
        public string OAuth_Scope { get; set; }
        public string OAuth_GetTokenUrl { get; set; }
        public string OAuth_ClientSecret { get; set; }
        public bool OAuth_IsProxyRequired { get; set; }

        public string Proxy_Url { get; set; }
        public string Proxy_Username { get; set; }
        public string Proxy_Password { get; set; }
    }
}
