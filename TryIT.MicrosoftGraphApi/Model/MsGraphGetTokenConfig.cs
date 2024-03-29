﻿using TryIT.MicrosoftGraphApi.Model.Utility;

namespace TryIT.MicrosoftGraphApi.Model
{
    /// <summary>
    /// Config information use for application obtain Token
    /// </summary>
    public class MsGraphGetTokenConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public string OAuth_TenantId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string OAuth_ClientId { get; set; }
        
        /// <summary>
        /// Azure url to obtain token, to config this url, https://login.microsoftonline.com/{tenant_id}/oauth2/v2.0/token
        /// </summary>
        public string OAuth_GetTokenUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string OAuth_ClientSecret { get; set; }

        /// <summary>
        /// the authorize url for user to perform manual sign in, required for first obtain token, no required for refresh token
        /// </summary>
        public string OAuth_AuthorizeUrl { get; set; }
        /// <summary>
        /// the redirect url configured in Azure Application Authentication, required for first obtain token, no required for refresh token
        /// </summary>
        public string OAuth_RedirectUrl { get; set; }        
        /// <summary>
        /// the scope defined for the account, required for first obtain token, no required for refresh token
        /// </summary>
        public string OAuth_Scope { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ProxyModel Proxy { get; set; }
    }
}
