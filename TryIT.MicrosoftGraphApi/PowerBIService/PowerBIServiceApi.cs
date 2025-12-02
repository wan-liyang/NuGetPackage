using System.Collections.Generic;
using System.Threading.Tasks;
using TryIT.MicrosoftGraphApi.HttpClientHelper;
using TryIT.MicrosoftGraphApi.Model;
using TryIT.MicrosoftGraphApi.MsGraphApi;
using TryIT.MicrosoftGraphApi.Response.PowerBIService;

namespace TryIT.MicrosoftGraphApi.PowerBIService
{
    /// <summary>
    /// https://learn.microsoft.com/en-us/rest/api/power-bi/datasets/get-refresh-history-in-group
    /// </summary>
    public class PowerBIServiceApi
    {
        private readonly PowerBIServiceHelper _helper;
        /// <summary>
        /// init application api with configuration
        /// </summary>
        /// <param name="config"></param>
        public PowerBIServiceApi(ApiConfig config)
        {
            TokenApi tokenApi = new TokenApi(new MsGraphApiConfig
            {
                Proxy = config.Proxy
            });

            var tokenResponse = tokenApi.GetTokenAsync(config.TokenRequestInfo).GetAwaiter().GetResult();

            var _msApiConfig = new MsGraphApiConfig
            {
                Proxy = config.Proxy,
                Token = tokenResponse.access_token,
                TimeoutSecond = config.TimeoutSecond,
                RetryProperty = config.RetryProperty
            };

            _helper = new PowerBIServiceHelper(_msApiConfig);
        }

        /// <summary>
        /// get the workspaces the user has access to.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<GetGroupsResponse.Group> GetGroupAsync(string name)
        {
            return await _helper.GetGroupAsync(name);
        }

        /// <summary>
        /// Returns a list of datasets from the specified workspace.
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public async Task<List<GetDatasetsInGroupResponse.Dataset>> GetDatasetsInGroupAsync(string groupName)
        {
            return await _helper.GetDatasetsInGroupAsync(groupName);
        }

        /// <summary>
        /// Triggers a refresh for the specified dataset from the specified workspace, return RequestId of the refresh (can use this RequestId to query Get Refresh History In Group get status of the refresh)
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="datasetName"></param>
        public async Task<string> RefreshDatasetInGroupAsync(string groupName, string datasetName)
        {
            return await _helper.RefreshDatasetInGroupAsync(groupName, datasetName);
        }


        /// <summary>
        /// Returns the refresh history for the specified dataset from the specified workspace
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="datasetName"></param>
        /// <returns></returns>
        public async Task<List<GetRefreshHistoryInGroupResponse.RefreshHistory>> GetRefreshHistoryInGroupAsync(string groupName, string datasetName)
        {
            return await _helper.GetRefreshHistoryInGroupAsync(groupName, datasetName);
        }
    }
}
