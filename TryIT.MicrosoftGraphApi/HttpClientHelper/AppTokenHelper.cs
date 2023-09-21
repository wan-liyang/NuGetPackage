using System;
using System.Collections.Generic;
using System.Net.Http;
using TryIT.MicrosoftGraphApi.Helper;
using TryIT.MicrosoftGraphApi.Model.Token;
using TryIT.MicrosoftGraphApi.Response.Token;

namespace TryIT.MicrosoftGraphApi.HttpClientHelper
{
    internal class AppTokenHelper : BaseHelper
    {
        private HttpClient _httpClient;

        public AppTokenHelper(HttpClient httpClient)
        {
            if (null == httpClient)
                throw new ArgumentNullException(nameof(httpClient));

            _httpClient = httpClient;
        }

        public GetAppTokenResponse.Response GetToken(GetTokenModel tokenModel)
        {
            string url = $"https://login.microsoftonline.com/{tokenModel.tenant_id}/oauth2/v2.0/token";

            try
            {
                Dictionary<string, string> dic = new Dictionary<string, string>
                {
                    { "grant_type", tokenModel.grant_type },
                    { "client_id", tokenModel.client_id },
                    { "client_secret", tokenModel.client_secret },
                    { "scope", tokenModel.scope }
                };

                HttpContent httpContent = new FormUrlEncodedContent(dic);
                var response = _httpClient.PostAsync(url, httpContent).GetAwaiter().GetResult();
                CheckStatusCode(response);

                string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                return content.JsonToObject<GetAppTokenResponse.Response>();
            }
            catch
            {
                throw;
            }
        }
    }
}
