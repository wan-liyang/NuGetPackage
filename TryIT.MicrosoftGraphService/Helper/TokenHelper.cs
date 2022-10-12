using TryIT.MicrosoftGraphService.Config;
using TryIT.MicrosoftGraphService.ExtensionHelper;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace TryIT.MicrosoftGraphService.Helper
{
    /// <summary>
    /// this token helper use approach from https://docs.microsoft.com/en-us/graph/auth-v2-user
    /// </summary>
    public class TokenHelper
    {
        private MsGraphGetTokenConfig _config;

        public TokenHelper(MsGraphGetTokenConfig config)
        {
            if(config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }
            
            if(string.IsNullOrEmpty(config.OAuth_TenantId))
            {
                throw new ArgumentNullException(nameof(config.OAuth_TenantId));
            }
            if(string.IsNullOrEmpty(config.OAuth_ClientId))
            {
                throw new ArgumentNullException(nameof(config.OAuth_ClientId));
            }
            if(string.IsNullOrEmpty(config.OAuth_ClientSecret))
            {
                throw new ArgumentNullException(nameof(config.OAuth_ClientSecret));
            }
            if(string.IsNullOrEmpty(config.OAuth_GetTokenUrl))
            {
                throw new ArgumentNullException(nameof(config.OAuth_GetTokenUrl));
            }

            if (config.OAuth_IsProxyRequired)
            {
                if (string.IsNullOrEmpty(config.Proxy_Url))
                {
                    throw new ArgumentNullException(nameof(config.Proxy_Url));
                }
            }

            _config = config;
        }

        /// <summary>
        /// get authorize url for user to perform manual sign in, system should redirect to this url for user to sign in
        /// </summary>
        public string AuthorizeUrl
        {
            get
            {
                string tenantId = _config.OAuth_TenantId;
                string clientId = _config.OAuth_ClientId;

                if(string.IsNullOrEmpty(_config.OAuth_AuthorizeUrl))
                {
                    throw new ArgumentNullException(nameof(_config.OAuth_AuthorizeUrl));
                }
                if(string.IsNullOrEmpty(_config.OAuth_RedirectUrl))
                {
                    throw new ArgumentNullException(nameof(_config.OAuth_RedirectUrl));
                }
                if(string.IsNullOrEmpty(_config.OAuth_Scope))
                {
                    throw new ArgumentNullException(nameof(_config.OAuth_Scope));
                }

                string authorizeURL = _config.OAuth_AuthorizeUrl.Replace("{tenant_id}", tenantId)
                    .Replace("{client_id}", clientId)
                    .Replace("{redirect_uri}", _config.OAuth_RedirectUrl)
                    .Replace("{scope}", _config.OAuth_Scope)
                    + "&state=" + Guid.NewGuid().ToString() + "&prompt=select_account";

                return authorizeURL;
            }
        }

        public GetTokenResponse GetToken(string code, string state)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "code", code }
            };

            GetTokenResponse tokenResponse = GetTokenResponse(state, dic);

            return tokenResponse;
        }

        public GetTokenResponse RefreshToken(string refresh_token, string state)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>
            {
                { "grant_type", "refresh_token" },
                { "refresh_token", refresh_token }
            };

            GetTokenResponse tokenResponse = GetTokenResponse(state, dic);

            return tokenResponse;
        }

        private GetTokenResponse GetTokenResponse(string state, Dictionary<string, string> requestHeaders)
        {
            string oauth_GetTokenUrl = _config.OAuth_GetTokenUrl;
            string tenantId = _config.OAuth_TenantId;
            oauth_GetTokenUrl = oauth_GetTokenUrl.Replace("{tenant_id}", tenantId);

            string clientId = _config.OAuth_ClientId;
            string clientSecret = _config.OAuth_ClientSecret;
            string redirectUrl = _config.OAuth_RedirectUrl;
            string scope = _config.OAuth_Scope;

            Dictionary<string, string> dic = new Dictionary<string, string>
            {
                { "client_id", clientId },
                { "client_secret", clientSecret },
                { "redirect_uri", redirectUrl },
                { "scope", scope }
            };

            if (requestHeaders != null && requestHeaders.Count > 0)
            {
                foreach (var item in requestHeaders.Keys)
                {
                    dic[item] = requestHeaders[item];
                }
            }

            var response = RequestToken(oauth_GetTokenUrl, dic);
            DateTime dtTokenIssueOn = DateTime.Now;

            bool isOk = response.Item1.ToUpper().Equals("OK");
            if (isOk && !string.IsNullOrEmpty(response.Item2))
            {
                GetTokenResponse tokenResponse = response.Item2.JsonToObject<GetTokenResponse>();
                tokenResponse.state = state;
                tokenResponse.issue_on = dtTokenIssueOn;
                tokenResponse.expires_on = tokenResponse.issue_on.AddSeconds(tokenResponse.expires_in);
                return tokenResponse;
            }
            else
            {
                throw new Exception("Invalid Token Request");
            }
        }

        private Tuple<string, string> RequestToken(string url, Dictionary<string, string> parameter)
        {
            string statusCode = "NotOk";
            string content = string.Empty;

            try
            {
                HttpClientHandler clientHandler = new HttpClientHandler()
                {
                    AllowAutoRedirect = true,
                    AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
                };

                WebProxy proxy = WebProxyHelper.GetProxy(_config.Proxy_Url, _config.Proxy_Username, _config.Proxy_Password);
                if (proxy != null)
                {
                    clientHandler.Proxy = proxy;
                }

                HttpClient client = new HttpClient(clientHandler);

                HttpContent httpContent = new FormUrlEncodedContent(parameter);
                var response = client.PostAsync(url, httpContent).Result;
                statusCode = response.StatusCode.ToString();
                content = response.Content.ReadAsStringAsync().Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return new Tuple<string, string>(statusCode, content);
        }
    }

    /// <summary>
    /// refer to https://docs.microsoft.com/en-us/graph/auth-v2-user for detail explanation for each response parameter
    /// </summary>
    public class GetTokenResponse
    {
        public string state { get; set; }
        /// <summary>
        /// Indicates the token type value. The only type that Azure AD supports is Bearer.
        /// </summary>
        public string token_type { get; set; }
        /// <summary>
        /// A space separated list of the Microsoft Graph permissions that the access_token is valid for.
        /// </summary>
        public string scope { get; set; }
        /// <summary>
        /// How long the access token is valid (in seconds).
        /// </summary>
        public int expires_in { get; set; }
        /// <summary>
        /// The requested access token. Your app can use this token to call Microsoft Graph.
        /// </summary>
        public string access_token { get; set; }
        /// <summary>
        /// An OAuth 2.0 refresh token. Your app can use this token to acquire additional access tokens after the current access token expires. Refresh tokens are long-lived, and can be used to retain access to resources for extended periods of time. For more detail, refer to the v2.0 token reference (https://docs.microsoft.com/en-us/azure/active-directory/develop/active-directory-v2-tokens).
        /// </summary>
        public string refresh_token { get; set; }



        // below are customized property to store token info

        /// <summary>
        /// store first time access_token or refresh_token issue time, 
        /// can use to identify whether refresh_token still available (up to 90 days)
        /// https://docs.microsoft.com/en-us/azure/active-directory/develop/refresh-tokens
        /// </summary>
        public DateTime issue_on { get; set; }
        /// <summary>
        /// store current access_token expire time (normally is one hour from current request)
        /// </summary>
        public DateTime expires_on { get; set; }
    }
}
