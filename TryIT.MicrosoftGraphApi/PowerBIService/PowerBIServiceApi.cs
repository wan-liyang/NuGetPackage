using System.Collections.Generic;
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

            var tokenResponse = tokenApi.GetToken(config.TokenRequestInfo);

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
        public GetGroupsResponse.Group GetGroup(string name)
        {
            return _helper.GetGroup(name);
        }

        /// <summary>
        /// Returns a list of datasets from the specified workspace.
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public List<GetDatasetsInGroupResponse.Dataset> GetDatasetsInGroup(string groupName)
        {
            return _helper.GetDatasetsInGroup(groupName);
        }

        /// <summary>
        /// Triggers a refresh for the specified dataset from the specified workspace, return RequestId of the refresh (can use this RequestId to query Get Refresh History In Group get status of the refresh)
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="datasetName"></param>
        public string RefreshDatasetInGroup(string groupName, string datasetName)
        {
            return _helper.RefreshDatasetInGroup(groupName, datasetName);
        }


        /// <summary>
        /// Returns the refresh history for the specified dataset from the specified workspace
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="datasetName"></param>
        /// <returns></returns>
        public List<GetRefreshHistoryInGroupResponse.RefreshHistory> GetRefreshHistoryInGroup(string groupName, string datasetName)
        {
            return _helper.GetRefreshHistoryInGroup(groupName, datasetName);
        }
    }
}
