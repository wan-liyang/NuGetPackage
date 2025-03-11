using System;
using System.Linq;
using System.Net.Http;
using TryIT.MicrosoftGraphApi.Helper;
using TryIT.MicrosoftGraphApi.Model;
using TryIT.MicrosoftGraphApi.Response.Application;

namespace TryIT.MicrosoftGraphApi.HttpClientHelper
{
    internal class ApplicationHelper : BaseHelper
    {
        public ApplicationHelper(MsGraphApiConfig config) : base(config) { }

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

            string url = $"{GraphApiRootUrl}/applications?$filter={EscapeExpression($"displayName eq '{appDisplayName}'")}";

            try
            {
                HttpClient.DefaultRequestHeaders.Add("ConsistencyLevel", "eventual");
                var response = RestApi.GetAsync(url).GetAwaiter().GetResult();
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
