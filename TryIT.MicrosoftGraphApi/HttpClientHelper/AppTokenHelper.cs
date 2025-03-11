using System;
using System.Collections.Generic;
using System.Net.Http;
using TryIT.MicrosoftGraphApi.Helper;
using TryIT.MicrosoftGraphApi.Model;
using TryIT.MicrosoftGraphApi.Model.Token;
using TryIT.MicrosoftGraphApi.Response.Token;

namespace TryIT.MicrosoftGraphApi.HttpClientHelper
{
    internal class AppTokenHelper : BaseHelper
    {
        public AppTokenHelper(MsGraphApiConfig config) : base(config) { }

        public GetAppTokenResponse.Response GetToken(GetTokenModel tokenModel)
        {
            string url = $"https://login.microsoftonline.com/{tokenModel.tenant_id}/oauth2/v2.0/token";

            Dictionary<string, string> dic = new Dictionary<string, string>
                {
                    { "grant_type", tokenModel.grant_type },
                    { "client_id", tokenModel.client_id },
                    { "client_secret", tokenModel.client_secret },
                    { "scope", tokenModel.scope }
                };

            HttpContent httpContent = new FormUrlEncodedContent(dic);
            var response = RestApi.PostAsync(url, httpContent).GetAwaiter().GetResult();
            CheckStatusCode(response);

            string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return content.JsonToObject<GetAppTokenResponse.Response>();
        }
    }
}
