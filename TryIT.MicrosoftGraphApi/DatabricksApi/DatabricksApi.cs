using System;
using System.Threading.Tasks;
using TryIT.MicrosoftGraphApi.HttpClientHelper;
using TryIT.MicrosoftGraphApi.Model;
using TryIT.MicrosoftGraphApi.Model.Databricks;
using TryIT.MicrosoftGraphApi.MsGraphApi;

namespace TryIT.MicrosoftGraphApi.DatabricksApi
{
    /// <summary>
    /// Provides methods for interacting with Databricks jobs, including triggering jobs and retrieving job run details
    /// asynchronously.
    /// </summary>
    /// <remarks>This class manages authentication tokens and handles token refresh automatically before
    /// making API calls. It is intended to be used as a client for Databricks job-related operations. Instances of this
    /// class are not thread-safe.</remarks>
    public class DatabricksApi
    {
        private DatabricksHelper _helper;
        private DateTime TokenExpireOn;

        private readonly ApiConfig _apiConfig;

        /// <summary>
        /// Initializes a new instance of the DatabricksApi class using the specified API configuration.
        /// </summary>
        /// <remarks>This constructor obtains an access token using the provided configuration and sets
        /// the token expiration time. The access token is required for authenticating subsequent API
        /// requests.</remarks>
        /// <param name="config">The configuration settings used to initialize the API client. Cannot be null.</param>
        public DatabricksApi(ApiConfig config)
        {
            _apiConfig = config;

            TokenApi tokenApi = new TokenApi(new MsGraphApiConfig
            {
                Proxy = config.Proxy,
                HttpLogDelegate = config.HttpLogDelegate,
            });

            var issueAt = DateTime.Now;
            var tokenResponse = tokenApi.GetTokenAsync(config.TokenRequestInfo).GetAwaiter().GetResult();
            this.TokenExpireOn = issueAt.AddSeconds(tokenResponse.expires_in);

            InitialHelper(tokenResponse.access_token);
        }

        private void InitialHelper(string access_token)
        {
            var config = new MsGraphApiConfig
            {
                Proxy = _apiConfig.Proxy,
                Token = access_token,
                TimeoutSecond = _apiConfig.TimeoutSecond,
                RetryProperty = _apiConfig.RetryProperty,
                HttpLogDelegate = _apiConfig.HttpLogDelegate
            };
            _helper = new DatabricksHelper(config);
        }

        /// <summary>
        /// refresh the token and helper if token expired
        /// </summary>
        private async Task RefreshToken()
        {
            if (this.TokenExpireOn < DateTime.Now.AddSeconds(-10))
            {
                TokenApi tokenApi = new TokenApi(new MsGraphApiConfig
                {
                    Proxy = _apiConfig.Proxy
                });

                var issueAt = DateTime.Now;
                var tokenResponse = await tokenApi.GetTokenAsync(_apiConfig.TokenRequestInfo);
                this.TokenExpireOn = issueAt.AddSeconds(tokenResponse.expires_in);

                InitialHelper(tokenResponse.access_token);
            }
        }

        /// <summary>
        /// Initiates the execution of a job asynchronously based on the specified request parameters.
        /// </summary>
        /// <param name="request">The request containing the details required to trigger the job. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see
        /// cref="TriggerJobResponse"/> with information about the triggered job.</returns>
        public async Task<TriggerJobResponse> TriggerJobAsync(TriggerJobRequest request)
        {
            await RefreshToken();
            return await _helper.TriggerJobAsync(request);
        }

        /// <summary>
        /// Retrieves detailed information about a specific job run asynchronously.
        /// </summary>
        /// <param name="request">The request object containing the parameters required to identify and retrieve the job run details.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see
        /// cref="GetJobRunResponse"/> with the details of the requested job run.</returns>
        public async Task<GetJobRunResponse> GetJobRunAsync(GetJobRunRequest request)
        {
            await RefreshToken();
            return await _helper.GetJobRunAsync(request);
        }
    }
}
