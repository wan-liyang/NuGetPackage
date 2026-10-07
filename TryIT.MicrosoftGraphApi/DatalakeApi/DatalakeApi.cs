using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TryIT.MicrosoftGraphApi.HttpClientHelper;
using TryIT.MicrosoftGraphApi.Model;
using TryIT.MicrosoftGraphApi.Model.Datalake;
using TryIT.MicrosoftGraphApi.MsGraphApi;

namespace TryIT.MicrosoftGraphApi.DatalakeApi
{
    /// <summary>
    /// Provides methods for interacting with a data lake, including file retrieval and upload operations.
    /// </summary>
    /// <remarks>The DatalakeApi class manages authentication tokens and handles communication with the
    /// underlying data lake service. Instances of this class are not thread-safe. For each operation, the
    /// authentication token is refreshed automatically if it has expired.</remarks>
    public class DatalakeApi
    {
        private DatalakeHelper _helper;
        private DateTime TokenExpireOn;

        private readonly ApiConfig _apiConfig;

        /// <summary>
        /// Initializes a new instance of the DatalakeApi class using the specified API configuration.
        /// </summary>
        /// <remarks>This constructor obtains an access token using the provided configuration and sets
        /// the token expiration time. The API instance is ready for authenticated requests after
        /// construction.</remarks>
        /// <param name="config">The configuration settings used to initialize the API, including authentication and proxy information.
        /// Cannot be null.</param>
        public DatalakeApi(ApiConfig config)
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
            _helper = new DatalakeHelper(config);
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
        /// Asynchronously retrieves a list of files based on the specified request parameters.
        /// </summary>
        /// <param name="request">An object containing the criteria for listing files, such as filters or pagination options. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="ListFileResponse"/>
        /// object with the list of files and related metadata.</returns>
        public async Task<ListFileResponse> ListFilesAsync(ListFileRequest request)
        {
            await RefreshToken();
            return await _helper.ListFilesAsync(request);
        }

        /// <summary>
        /// Asynchronously retrieves the file specified by the request.
        /// </summary>
        /// <param name="request">An object that specifies the parameters for the file retrieval operation. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a stream for reading the file
        /// contents.</returns>
        public async Task<Stream> GetFileAsync(GetFileRequest request)
        {
            await RefreshToken();
            return await _helper.GetFileAsync(request);
        }

        /// <summary>
        /// Asynchronously uploads a file using the specified upload request.
        /// </summary>
        /// <param name="request">An object containing the details of the file to upload. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous upload operation.</returns>
        public async Task UploadFileAsync(UploadFileRequest request)
        {
            await RefreshToken();
            await _helper.UploadFileAsync(request);
        }

        /// <summary>
        /// Asynchronously deletes the file specified in the request.
        /// </summary>
        /// <param name="request">An object containing the details of the file to delete. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous delete operation.</returns>
        public async Task DeleteFileAsync(DeleteFileRequest request)
        {
            await RefreshToken();
            await _helper.DeleteFileAsync(request);
        }
    }
}
