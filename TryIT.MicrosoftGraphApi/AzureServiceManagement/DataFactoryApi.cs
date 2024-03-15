using TryIT.MicrosoftGraphApi.Helper;
using TryIT.MicrosoftGraphApi.HttpClientHelper;
using TryIT.MicrosoftGraphApi.Model;
using TryIT.MicrosoftGraphApi.MsGraphApi;
using TryIT.MicrosoftGraphApi.Request.DataFactory;
using TryIT.MicrosoftGraphApi.Response.DataFactory;

namespace TryIT.MicrosoftGraphApi.AzureServiceManagement
{
    /// <summary>
    /// https://learn.microsoft.com/en-us/rest/api/datafactory/pipelines/create-run
    /// </summary>
    public class DataFactoryApi
    {
        private static DataFactoryHelper _helper;

        /// <summary>
        /// init application api with configuration
        /// </summary>
        /// <param name="config"></param>
        public DataFactoryApi(ApiConfig config)
        {
            TokenApi tokenApi = new TokenApi(new MsGraphApiConfig
            {
                Proxy = config.Proxy
            });

            var tokenResponse = tokenApi.GetToken(config.TokenRequestInfo);

            MsGraphHelper graphHelper = new MsGraphHelper(new MsGraphApiConfig
            {
                Proxy = config.Proxy,
                Token = tokenResponse.access_token,
                TimeoutSecond = config.TimeoutSecond,
            });
            _helper = new DataFactoryHelper(graphHelper.GetHttpClient());
        }

        /// <summary>
        /// Creates a run of a pipeline
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public CreateRunResponse.Response CreateRun(CreateRunRequest request)
        {
            return _helper.CreateRun(request);
        }

        /// <summary>
        /// Get a pipeline run by its run ID
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public GetPipelineRunsResponse.Response GetPipelineRun(GetPipelineRunRequest request)
        {
            return _helper.GetPipelineRun(request);
        }
    }
}
