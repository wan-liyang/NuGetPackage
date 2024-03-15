using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using TryIT.MicrosoftGraphApi.Helper;
using TryIT.MicrosoftGraphApi.Response.PowerBIService;

namespace TryIT.MicrosoftGraphApi.HttpClientHelper
{
    internal class PowerBIServiceHelper : BaseHelper
    {
        private HttpClient _httpClient;

        public PowerBIServiceHelper(HttpClient httpClient)
        {
            if (null == httpClient)
                throw new ArgumentNullException(nameof(httpClient));

            _httpClient = httpClient;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">workspace name</param>
        /// <returns></returns>
        public GetGroupsResponse.Group GetGroup(string name)
        {
            string url = $"https://api.powerbi.com/v1.0/myorg/groups?$filter=name eq '{name}'";

            try
            {
                var response = _httpClient.GetAsync(url).GetAwaiter().GetResult();
                CheckStatusCode(response);

                string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                return content.JsonToObject<GetGroupsResponse.Response>().value.FirstOrDefault();
            }
            catch
            {
                throw;
            }
        }

        public List<GetDatasetsInGroupResponse.Dataset> GetDatasetsInGroup(string groupName)
        {
            var group = GetGroup(groupName);

            string url = $"https://api.powerbi.com/v1.0/myorg/groups/{group.id}/datasets";

            try
            {
                var response = _httpClient.GetAsync(url).GetAwaiter().GetResult();
                CheckStatusCode(response);

                string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                return content.JsonToObject<GetDatasetsInGroupResponse.Response>().value;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Triggers a refresh for the specified dataset from the specified workspace, return RequestId of the refresh (can use this RequestId to query Get Refresh History In Group get status of the refresh)
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="datasetName"></param>
        /// <returns></returns>
        public string RefreshDatasetInGroup(string groupName, string datasetName)
        {
            var group = GetGroup(groupName);
            var datasets = GetDatasetsInGroup(groupName);

            var dataset = datasets.First(p => p.name.Equals(datasetName, StringComparison.OrdinalIgnoreCase));

            string url = $"https://api.powerbi.com/v1.0/myorg/groups/{group.id}/datasets/{dataset.id}/refreshes";

            try
            {
                HttpContent httpContent = new StringContent("");

                var response = _httpClient.PostAsync(url, httpContent).GetAwaiter().GetResult();
                CheckStatusCode(response);

                if (response.Headers.Contains("RequestId"))
                {
                    return response.Headers.GetValues("RequestId").First();
                }
            }
            catch
            {
                throw;
            }

            return null;
        }

        /// <summary>
        /// Returns the refresh history for the specified dataset from the specified workspace
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="datasetName"></param>
        /// <returns></returns>
        public List<GetRefreshHistoryInGroupResponse.RefreshHistory> GetRefreshHistoryInGroup(string groupName, string datasetName)
        {
            var group = GetGroup(groupName);
            var datasets = GetDatasetsInGroup(groupName);

            var dataset = datasets.First(p => p.name.Equals(datasetName, StringComparison.OrdinalIgnoreCase));

            string url = $"https://api.powerbi.com/v1.0/myorg/groups/{group.id}/datasets/{dataset.id}/refreshes";

            try
            {
                var response = _httpClient.GetAsync(url).GetAwaiter().GetResult();
                CheckStatusCode(response);

                string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                return content.JsonToObject<GetRefreshHistoryInGroupResponse.Response>().value;
            }
            catch
            {
                throw;
            }
        }
    }
}
