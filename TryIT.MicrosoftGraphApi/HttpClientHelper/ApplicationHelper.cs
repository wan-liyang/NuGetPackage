using System;
using System.Collections.Generic;
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

            HttpClient.DefaultRequestHeaders.Add("ConsistencyLevel", "eventual");
            var response = RestApi.GetAsync(url).GetAwaiter().GetResult();
            CheckStatusCode(response);

            string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return content.JsonToObject<GetAppliationResponse.Response>().value.FirstOrDefault();
        }

        /// <summary>
        /// get owned applications by user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public List<GetAppliationResponse.Appliation> GetOwnedApplications(string userId)
        {

            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }

            string url = $"{GraphApiRootUrl}/users/{userId}/ownedObjects/microsoft.graph.application";

            var response = RestApi.GetAsync(url).GetAwaiter().GetResult();
            CheckStatusCode(response);


            string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var responseObj = content.JsonToObject<GetAppliationResponse.Response>();

            List<GetAppliationResponse.Appliation> list = new List<GetAppliationResponse.Appliation>();
            list.AddRange(responseObj.value);

            if (!string.IsNullOrEmpty(responseObj.odatanextLink))
            {
                _getnextlink(responseObj.odatanextLink, list);
            }

            return list;
        }

        private void _getnextlink(string nextLink, List<GetAppliationResponse.Appliation> list)
        {
            var response = RestApi.GetAsync(nextLink).GetAwaiter().GetResult();
            CheckStatusCode(response, RestApi.RetryResults);

            string content = response.Content.ReadAsStringAsync().Result;
            var responseObj = content.JsonToObject<GetAppliationResponse.Response>();

            list.AddRange(responseObj.value);

            if (!string.IsNullOrEmpty(responseObj.odatanextLink))
            {
                _getnextlink(responseObj.odatanextLink, list);
            }
        }
    }
}
