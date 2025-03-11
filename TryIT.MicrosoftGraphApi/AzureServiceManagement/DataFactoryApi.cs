using System;
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
        private DataFactoryHelper _helper;
        private DateTime TokenIssueOn;
        private DateTime TokenExpireOn;

        private readonly ApiConfig _apiConfig;
        /// <summary>
        /// init application api with configuration
        /// </summary>
        /// <param name="config"></param>
        public DataFactoryApi(ApiConfig config)
        {
            _apiConfig = config;

            TokenApi tokenApi = new TokenApi(new MsGraphApiConfig
            {
                Proxy = config.Proxy
            });

            this.TokenIssueOn = DateTime.Now;
            var tokenResponse = tokenApi.GetToken(config.TokenRequestInfo);
            this.TokenExpireOn = this.TokenIssueOn.AddSeconds(tokenResponse.expires_in);

            InitialGraphHelper(tokenResponse.access_token);
        }

        /// <summary>
        /// refresh the token and helper if token expired
        /// </summary>
        private void RefreshToken()
        {
            if (this.TokenExpireOn < DateTime.Now.AddSeconds(-10))
            {
                TokenApi tokenApi = new TokenApi(new MsGraphApiConfig
                {
                    Proxy = _apiConfig.Proxy
                });

                var tokenResponse = tokenApi.GetToken(_apiConfig.TokenRequestInfo);
                this.TokenExpireOn = this.TokenIssueOn.AddSeconds(tokenResponse.expires_in);

                InitialGraphHelper(tokenResponse.access_token);
            }
        }

        private void InitialGraphHelper(string access_token)
        {
            var config = new MsGraphApiConfig
            {
                Proxy = _apiConfig.Proxy,
                Token = access_token,
                TimeoutSecond = _apiConfig.TimeoutSecond,
                RetryProperty = _apiConfig.RetryProperty
            };
            _helper = new DataFactoryHelper(config);
        }

        /// <summary>
        /// Creates a run of a pipeline
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public CreateRunResponse.Response CreateRun(CreateRunRequest request)
        {
            RefreshToken();
            return _helper.CreateRun(request);
        }

        /// <summary>
        /// Get a pipeline run by its run ID
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public GetPipelineRunsResponse.Response GetPipelineRun(GetPipelineRunRequest request)
        {
            RefreshToken();
            return _helper.GetPipelineRun(request);
        }
    }
}
