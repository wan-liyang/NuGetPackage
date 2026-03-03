using System;
using System.Net.Http;
using System.Threading.Tasks;
using TryIT.RestApi.Models;
using TryIT.TableauApi.ApiResponse;
using TryIT.TableauApi.SiteModel;

namespace TryIT.TableauApi
{
    public partial class TableauConnector : IDisposable
    {
        string siteId;
        string myId;

        private RestApi.Api _api;
        RestApi.Api RestApiInstance
        {
            get
            {
                if (_api == null)
                {
                    throw new InvalidOperationException("please call SignIn method to initialize rest api instance before any API call");
                }
                return _api;
            }
        }

        private readonly ApiRequestModel _requestModel;
        private readonly HttpClientConfig _httpClientConfig;

        private static void ValidateInfo(ApiRequestModel requestModel)
        {
            if (requestModel == null)
            {
                throw new ArgumentNullException(nameof(requestModel));
            }
            if (string.IsNullOrEmpty(requestModel.HostUrl))
            {
                throw new ArgumentException(nameof(requestModel.HostUrl));
            }
            if (string.IsNullOrEmpty(requestModel.Sitename))
            {
                throw new ArgumentException(nameof(requestModel.Sitename));
            }
            if (string.IsNullOrEmpty(requestModel.ApiVersion))
            {
                throw new ArgumentException(nameof(requestModel.ApiVersion));
            }
            if (string.IsNullOrEmpty(requestModel.TokenName))
            {
                throw new ArgumentException(nameof(requestModel.TokenName));
            }
            if (string.IsNullOrEmpty(requestModel.TokenSecret))
            {
                throw new ArgumentException(nameof(requestModel.TokenSecret));
            }
        }

        

        /// <summary>
        /// Init Tableau Connector, after initializing, you can call SignIn method to get token for later API call, if the token is expired, just call SignIn method again to refresh the token
        /// </summary>
        /// <param name="httpClientConfig"></param>
        /// <param name="requestModel"></param>
        public TableauConnector(HttpClientConfig httpClientConfig, ApiRequestModel requestModel)
        {
            ValidateInfo(requestModel);

            _requestModel = requestModel;
            _httpClientConfig = httpClientConfig;
        }

        /// <summary>
        /// Perform sign in action to get siteId, userId and token, the token will be added into httpClient for later API call, if the token is expired, just call this method again to refresh the token
        /// </summary>
        public async Task SignIn()
        {
            string url = $"{_requestModel.HostUrl}/api/{_requestModel.ApiVersion}/auth/signin";
            string request = $"<tsRequest><credentials personalAccessTokenName=\"{_requestModel.TokenName}\" personalAccessTokenSecret=\"{_requestModel.TokenSecret}\"><site contentUrl=\"{_requestModel.Sitename}\"/></credentials></tsRequest>";
            StringContent requestContent = new StringContent(request, System.Text.Encoding.UTF8, "application/xml");
            var responseMessage = await new RestApi.Api(_httpClientConfig).PostAsync(url, requestContent);
            CheckResponseStatus(responseMessage);

            var content = await responseMessage.Content.ReadAsStringAsync();
            var signinResponse = content.JsonToObject<SigninResponse.Response>();
            this.siteId = signinResponse.credentials.site.id;
            this.myId = signinResponse.credentials.user.id;

            HttpClientConfig clientConfig = new HttpClientConfig
            {
                TimeoutSecond = _httpClientConfig.TimeoutSecond,
                BasicAuth = _httpClientConfig.BasicAuth,
                ClientCertificates = _httpClientConfig.ClientCertificates,
                HttpClient = _httpClientConfig.HttpClient,
                HttpLogDelegate = _httpClientConfig.HttpLogDelegate,
                Proxy = _httpClientConfig.Proxy,
                securityProtocolType = _httpClientConfig.securityProtocolType,
                Headers = _httpClientConfig.Headers,
                RetryProperty = _httpClientConfig.RetryProperty,
            };

            // add the token into httpClient
            string token = signinResponse.credentials.token;
            clientConfig.Headers["X-Tableau-Auth"] = token;

            _api = new RestApi.Api(clientConfig);
        }

        /// <summary>
        /// Dispose HttpClient and clear siteId and myId, after calling Dispose, the instance is not recommended to use anymore
        /// </summary>
        public void Dispose()
        {
            siteId = null;
            myId = null;
            _api = null;
        }

        /// <summary>
        /// check API response status, if failed, throw exception
        /// </summary>
        /// <param name="responseMessage"></param>
        /// <exception cref="Exception"></exception>
        private static void CheckResponseStatus(HttpResponseMessage responseMessage)
        {
            if (!responseMessage.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"operation failed: {responseMessage.Content.ReadAsStringAsync().Result}");
            }
        }

        /// <summary>
        /// get total pages based on PageSize and TotalAvailable items
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="totalAvailable"></param>
        private static int GetTotalPages(int pageSize, int totalAvailable)
        {
            int pages = totalAvailable / pageSize;

            if (totalAvailable % pageSize > 0)
            {
                pages++;
            }

            return pages;
        }
    }
}
