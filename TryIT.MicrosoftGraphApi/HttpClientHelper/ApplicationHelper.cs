using System;
using System.Linq;
using System.Net.Http;
using TryIT.MicrosoftGraphApi.Helper;
using TryIT.MicrosoftGraphApi.Response.Application;

namespace TryIT.MicrosoftGraphApi.HttpClientHelper
{
    internal class ApplicationHelper : BaseHelper
    {
        private HttpClient _httpClient;

        public ApplicationHelper(HttpClient httpClient)
        {
            if (null == httpClient)
                throw new ArgumentNullException(nameof(httpClient));

            _httpClient = httpClient;
        }

        /// <summary>
        /// get application detail
        /// </summary>
        /// <param name="appDisplayName">application display name</param>
        /// <returns></returns>
        public GetAppliationResponse.Appliation GetApplication(string appDisplayName)
        {
            if (string.IsNullOrEmpty(appDisplayName))
            {
                throw new ArgumentNullException(nameof(appDisplayName));
            }

            string url = $"{GraphApiRootUrl}/applications?$filter=displayName eq '{appDisplayName}'";

            try
            {
                _httpClient.DefaultRequestHeaders.Add("ConsistencyLevel", "eventual");
                var response = _httpClient.GetAsync(url).GetAwaiter().GetResult();
                CheckStatusCode(response);

                string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                return content.JsonToObject<GetAppliationResponse.Response>().value.FirstOrDefault();
            }
            catch
            {
                throw;
            }
        }
    }
}
