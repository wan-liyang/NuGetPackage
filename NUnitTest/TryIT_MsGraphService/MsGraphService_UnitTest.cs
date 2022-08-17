using TryIT.MicrosoftGraphService;
using TryIT.MicrosoftGraphService.Config;
using TryIT.MicrosoftGraphService.Helper;
using NUnit.Framework;

namespace NUnitTest.Liyang_MsGraphService
{
    class MsGraphService_UnitTest
    {
        [Test]
        public void GraphService_Test()
        {
            MsGraphGetTokenConfig tokenConfig = new MsGraphGetTokenConfig
            {
                OAuth_AuthorizeUrl = "",
                OAuth_TenantId = "",
                OAuth_ClientId = "",
                OAuth_RedirectUrl = "",
                OAuth_Scope = "",
                OAuth_GetTokenUrl = "",
                OAuth_ClientSecret = "",
                OAuth_IsProxyRequired = true,

                Proxy_Url = "",
                Proxy_Username = "",
                Proxy_Password = ""
            };

            // check token availability
            /*
             * 1. get exists token (StateGuid, ExpiresOn, Access_token, Refresh_token)
             * 2. if ExpiresOn < Now, need refresh token to get new token
             * 3. save new token
             */
            TokenHelper tokenHelper = new TokenHelper(tokenConfig);
            var newtoken = tokenHelper.RefreshToken("Refresh_token", "StateGuid");

            MsGraphApiConfig apiConfig = new MsGraphApiConfig
            {
                Token = newtoken.access_token
            };

            MsGraphService.Outlook MsOutlook = new MsGraphService.Outlook(apiConfig);
            MsOutlook.GetMessages("Me", "Inbox");
        }

        [Test]
        public void TokeHelper_Test()
        {
            MsGraphGetTokenConfig tokenConfig = new MsGraphGetTokenConfig
            {
                OAuth_AuthorizeUrl = "",
                OAuth_TenantId = "",
                OAuth_ClientId = "",
                OAuth_RedirectUrl = "",
                OAuth_Scope = "",
                OAuth_GetTokenUrl = "",
                OAuth_ClientSecret = "",
                OAuth_IsProxyRequired = true,

                Proxy_Url = "",
                Proxy_Username = "",
                Proxy_Password = ""
                //Token = tokenResponse.access_token
            };

            TokenHelper tokenHelper = new TokenHelper(tokenConfig);

            // 1. validate token config
            if (tokenHelper.IsOAuthParameterValid)
            {
                // if token parameter is valid, then use browser redirect to the tokenHelper.AuthorizeUrl
                string authorizeURL = tokenHelper.AuthorizeUrl;
            }

            // 2. after authentication with above tokenHelper.AuthorizeUrl, will will redirect back to tokenConfig.OAuth_RedirectUrl
            // 3. tokenConfig.OAuth_RedirectUrl to get "code" & "state" value from query parameter
            // 4. use "code" & "state" value, to get token

            //string code = RequestUtility.GetQueryValue<string>(QueryStringKey.OAuth_code);
            //string state = RequestUtility.GetQueryValue<string>(QueryStringKey.OAuth_state);
            string code = string.Empty;
            string state = string.Empty;
            GetTokenResponse tokenResponse = tokenHelper.GetToken(code, state);

            // 5. GetTokenResponse will provide "access_token" & "refresh_token"
            // 6. use "access_token" to call MsGraphService (via Graph API)
            MsGraphApiConfig apiConfig = new MsGraphApiConfig
            {
                Token = tokenResponse.access_token
            };

            MsGraphService.Outlook MsOutlook = new MsGraphService.Outlook(apiConfig);
            MsOutlook.GetMessages("Me", "Inbox");

            // 7. "access_token" will expiry after "expires_on"
            // 8. use "refresh_token" to get new "access_token" if "access_token" expired
            string refresh_token = tokenResponse.refresh_token;
            tokenHelper.RefreshToken(refresh_token, state);
        }
    }
}
