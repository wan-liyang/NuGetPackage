using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Polly;
using Polly.Retry;

namespace TryIT.RestApi
{
    /// <summary>
    /// initial API request
    /// </summary>
    public class ApiRequest
    {
        private readonly AsyncRetryPolicy _retryPolicy;
        private HttpClient _HttpClient;
        public List<string> RetryLog = new List<string>();

        /// <summary>
        /// initial ApiRequest with defualt <see cref="HttpClientConfig"/>
        /// </summary>
        public ApiRequest()
        {
            _HttpClient = InitHttpClient(new HttpClientConfig());

            _retryPolicy = Policy.Handle<HttpRequestException>().RetryAsync(3, onRetry: (exception, retryCount) =>
            {
                // Add logic to be executed before each retry, such as logging
                RetryLog.Add($"retry {retryCount}, previous exception: {exception.Message}");
            });
        }

        /// <summary>
        /// initial ApiRequest with <see cref="HttpClientConfig"/>
        /// </summary>
        /// <param name="config"></param>
        public ApiRequest(HttpClientConfig config)
        {
            _HttpClient = InitHttpClient(config);

            _retryPolicy = Policy.Handle<HttpRequestException>().RetryAsync(3, onRetry: (exception, retryCount) =>
            {
                // Add logic to be executed before each retry, such as logging
                RetryLog.Add($"retry {retryCount}, exception: {exception.Message}");
            });
        }

        public async Task<ResponseModel> GetAsync(RequestModel request)
        {
            HttpClient client = SetupHttpClient(request);
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                var clientResult = await client.GetAsync(request.Url);

                return new ResponseModel
                {
                    StatusCode = clientResult.StatusCode,
                    Content = clientResult.Content
                };
            });
        }

        public async Task<ResponseModel> PostAsync(RequestModel request)
        {
            HttpClient client = SetupHttpClient(request);
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                if (request.HttpContent == null)
                {
                    request.HttpContent = new StringContent("", System.Text.Encoding.UTF8, "application/json");
                }

                var clientResult = await client.PostAsync(request.Url, request.HttpContent);

                return new ResponseModel
                {
                    StatusCode = clientResult.StatusCode,
                    Content = clientResult.Content
                };
            });
        }

        /// <summary>
        /// initial HttpClient
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        private HttpClient InitHttpClient(HttpClientConfig config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            HttpClientHandler clientHandler = new HttpClientHandler()
            {
                AllowAutoRedirect = true,
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
            };

            if (config.WebProxy != null)
            {
                var proxy = GetWebProxy(config.WebProxy);
                if (proxy != null)
                {
                    clientHandler.Proxy = proxy;
                }
            }
            HttpClient client = new HttpClient(clientHandler);

            if (config.TimeoutSecond > 0)
            {
                client.Timeout = TimeSpan.FromSeconds(config.TimeoutSecond);
            }

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            ServicePointManager.SecurityProtocol = config.securityProtocolType;

            return client;
        }

        /// <summary>
        /// setup HttpClient for individual request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        private HttpClient SetupHttpClient(RequestModel request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            if (string.IsNullOrEmpty(request.Url))
            {
                throw new ArgumentNullException(nameof(request.Url));
            }

            _HttpClient.BaseAddress = new Uri(request.Url);

            if (request.BasicAuth != null)
            {
                if (!string.IsNullOrEmpty(request.BasicAuth.Username) || !string.IsNullOrEmpty(request.BasicAuth.Password))
                {
                    string basicToken = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes($"{request.BasicAuth.Username}:{request.BasicAuth.Password}"));
                    _HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", basicToken);
                }
            }

            if (request.Headers != null && request.Headers.Count > 0)
            {
                foreach (var item in request.Headers)
                {
                    _HttpClient.DefaultRequestHeaders.Remove(item.Key);
                    _HttpClient.DefaultRequestHeaders.Add(item.Key, item.Value);
                }
            }

            return _HttpClient;
        }

        private WebProxy GetWebProxy(HttpClientConfig.WebProxyInfo webProxyInfo)
        {
            if (!string.IsNullOrEmpty(webProxyInfo.Url))
            {
                WebProxy proxy = new WebProxy(webProxyInfo.Url);
                proxy.UseDefaultCredentials = true;

                if (!string.IsNullOrEmpty(webProxyInfo.Username))
                {
                    proxy.UseDefaultCredentials = false;
                    proxy.Credentials = new NetworkCredential(webProxyInfo.Username, webProxyInfo.Password);
                }
                proxy.BypassProxyOnLocal = true;

                return proxy;
            }
            return null;
        }
    }
}
