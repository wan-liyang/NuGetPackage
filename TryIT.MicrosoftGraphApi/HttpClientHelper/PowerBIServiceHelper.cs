using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TryIT.MicrosoftGraphApi.Helper;
using TryIT.MicrosoftGraphApi.Model;
using TryIT.MicrosoftGraphApi.Response.PowerBIService;

namespace TryIT.MicrosoftGraphApi.HttpClientHelper
{
    internal class PowerBIServiceHelper : BaseHelper
    {
        public PowerBIServiceHelper(MsGraphApiConfig config) : base(config) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">workspace name</param>
        /// <returns></returns>
        public async Task<GetGroupsResponse.Group> GetGroupAsync(string name)
        {
            string url = $"https://api.powerbi.com/v1.0/myorg/groups?$filter={EscapeExpression($"name eq '{name}'")}";

            var response = await RestApi.GetAsync(url);
            CheckStatusCode(response);

            string content = await response.Content.ReadAsStringAsync();
            return content.JsonToObject<GetGroupsResponse.Response>().value.FirstOrDefault();
        }

        public async Task<List<GetDatasetsInGroupResponse.Dataset>> GetDatasetsInGroupAsync(string groupName)
        {
            var group = await GetGroupAsync(groupName);

            string url = $"https://api.powerbi.com/v1.0/myorg/groups/{group.id}/datasets";

            var response = await RestApi.GetAsync(url);
            CheckStatusCode(response);

            string content = await response.Content.ReadAsStringAsync();
            return content.JsonToObject<GetDatasetsInGroupResponse.Response>().value;
        }

        /// <summary>
        /// Triggers a refresh for the specified dataset from the specified workspace, return RequestId of the refresh (can use this RequestId to query Get Refresh History In Group get status of the refresh)
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="datasetName"></param>
        /// <returns></returns>
        public async Task<string> RefreshDatasetInGroupAsync(string groupName, string datasetName)
        {
            var group = await GetGroupAsync(groupName);
            var datasets = await GetDatasetsInGroupAsync(groupName);

            var dataset = datasets.First(p => p.name.Equals(datasetName, StringComparison.OrdinalIgnoreCase));

            string url = $"https://api.powerbi.com/v1.0/myorg/groups/{group.id}/datasets/{dataset.id}/refreshes";

            HttpContent httpContent = new StringContent("");

            var response = await RestApi.PostAsync(url, httpContent);
            CheckStatusCode(response);

            if (response.Headers.Contains("RequestId"))
            {
                return response.Headers.GetValues("RequestId").First();
            }

            return null;
        }

        /// <summary>
        /// Returns the refresh history for the specified dataset from the specified workspace
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="datasetName"></param>
        /// <returns></returns>
        public async Task<List<GetRefreshHistoryInGroupResponse.RefreshHistory>> GetRefreshHistoryInGroupAsync(string groupName, string datasetName)
        {
            var group = await GetGroupAsync(groupName);
            var datasets = await GetDatasetsInGroupAsync(groupName);

            var dataset = datasets.First(p => p.name.Equals(datasetName, StringComparison.OrdinalIgnoreCase));

            string url = $"https://api.powerbi.com/v1.0/myorg/groups/{group.id}/datasets/{dataset.id}/refreshes";

            var response = await RestApi.GetAsync(url);
            CheckStatusCode(response);

            string content = await response.Content.ReadAsStringAsync();
            return content.JsonToObject<GetRefreshHistoryInGroupResponse.Response>().value;
        }
    }
}
