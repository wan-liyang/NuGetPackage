using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using TryIT.MicrosoftGraphService.ApiModel;
using TryIT.MicrosoftGraphService.ExtensionHelper;
using static TryIT.MicrosoftGraphService.ApiModel.AppliationResponse;

namespace TryIT.MicrosoftGraphService.HttpClientHelper
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
        public GetAppliationResponse GetApplication(string appDisplayName)
        {
            if (string.IsNullOrEmpty(appDisplayName))
            {
                throw new ArgumentNullException(nameof(appDisplayName));
            }

            string url = $"https://graph.microsoft.com/v1.0/applications?$filter=displayName eq '{appDisplayName}'";

            try
            {
                _httpClient.DefaultRequestHeaders.Add("ConsistencyLevel", "eventual");

                var response = _httpClient.GetAsync(url).GetAwaiter().GetResult();

                string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                CheckStatusCode(response);

                return content.JsonToObject<GetAppliationResponse>();
            }
            catch
            {
                throw;
            }
        }
    }
}
