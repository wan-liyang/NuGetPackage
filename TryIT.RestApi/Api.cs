using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using TryIT.RestApi.Models;
using TryIT.RestApi.Utilities;

namespace TryIT.RestApi
{
    /// <summary>
    /// make a api call with retry
    /// </summary>
    public class Api
    {
        private ResiliencePipeline<HttpResponseMessage> _pipeline;
        private readonly HttpClient _httpClient;

        private List<RetryResult> _retryResults = new List<RetryResult>();
        /// <summary>
        /// retry results, capture each response or exception
        /// </summary>
        public List<RetryResult> RetryResults
        {
            get
            {
                return _retryResults;
            }
        }

        /// <summary>
        /// init api instance with HttpClient configuration
        /// </summary>
        /// <param name="clientConfig"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public Api(HttpClientConfig clientConfig)
        {
            if (clientConfig == null)
            {
                throw new ArgumentNullException(nameof(clientConfig));
            }

            HttpClient httpClient = null;
            if (clientConfig.HttpClient != null)
            {
                httpClient = clientConfig.HttpClient;
            }
            else
            {
                HttpClientHandler clientHandler = new HttpClientHandler()
                {
                    AllowAutoRedirect = true,
                    AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
                };

                if (clientConfig.Proxy != null)
                {
                    var proxy = UtliFunction.GetWebProxy(clientConfig.Proxy);
                    if (proxy != null)
                    {
                        clientHandler.Proxy = proxy;
                    }
                }
                httpClient = new HttpClient(clientHandler);
            }

            

            if (clientConfig.TimeoutSecond > 0)
            {
                httpClient.Timeout = TimeSpan.FromSeconds(clientConfig.TimeoutSecond);
            }

            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            ServicePointManager.SecurityProtocol = clientConfig.securityProtocolType;

            // set basic auth when username and password are not empty
            if (!string.IsNullOrEmpty(clientConfig?.BasicAuth?.Username) && !string.IsNullOrEmpty(clientConfig?.BasicAuth?.Password))
            {
                string basicToken = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes($"{clientConfig.BasicAuth.Username}:{clientConfig.BasicAuth.Password}"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", basicToken);
            }

            if (clientConfig.Headers != null && clientConfig.Headers.Count > 0)
            {
                foreach (var item in clientConfig.Headers)
                {
                    httpClient.DefaultRequestHeaders.Remove(item.Key);
                    httpClient.DefaultRequestHeaders.Add(item.Key, item.Value);
                }
            }

            _httpClient = httpClient;

            EnableRetry(clientConfig);
        }

        private void EnableRetry(HttpClientConfig config)
        {
            if (config.RetryProperty != null 
                && config.RetryProperty.RetryStatusCodes  != null 
                && config.RetryProperty.RetryStatusCodes.Any())
            {
                _pipeline = new ResiliencePipelineBuilder<HttpResponseMessage>()
                       .AddRetry(new RetryStrategyOptions<HttpResponseMessage>
                       {
                           ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                                //.Handle<TaskCanceledException>(result => result.InnerException is TimeoutException) // handle timeout exception
                                .HandleResult(result => config.RetryProperty.RetryStatusCodes.Contains(result.StatusCode)),
                           Delay = config.RetryProperty.RetryDelay,
                           MaxRetryAttempts = config.RetryProperty.RetryCount,
                           BackoffType = DelayBackoffType.Constant,
                           OnRetry = args =>
                           {
                               ResultMessage resultMessage = new ResultMessage();
                               var result = args.Outcome.Result;
                               if (result != null)
                               {
                                   resultMessage.StatusCode = $"{(int)result.StatusCode} - {result.StatusCode.ToString()}";
                                   resultMessage.ReasonPhrase = result.ReasonPhrase;
                                   resultMessage.RequestUri = result.RequestMessage.RequestUri;
                               }

                               _retryResults.Add(new RetryResult
                               {
                                   AttemptNumber = args.AttemptNumber,
                                   Result = resultMessage,
                                   Exception = args.Outcome.Exception
                               });

                               return default;
                           }
                       })
                       .Build();
            }
            else
            {
                _pipeline = new ResiliencePipelineBuilder<HttpResponseMessage>().Build();
            }
        }

        /// <summary>
        /// call Get method
        /// </summary>
        /// <param name="url">url with or without parameters, if with parameters, the parameter key and value must UrlEncode</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> GetAsync(string url)
        {
            return await _pipeline.ExecuteAsync(async exec =>
            {
                return await _httpClient.GetAsync(url);
            });
        }

        /// <summary>
        /// call Get method
        /// </summary>
        /// <param name="url">base url without parameters</param>
        /// <param name="parameters">url parameters</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> GetAsync(string url, Dictionary<string, string> parameters)
        {
            return await _pipeline.ExecuteAsync(async exec =>
            {
                string paras = string.Empty;

                if (paras != null && paras.Length > 0)
                {
                    paras = string.Join("&", parameters.Select(p => $"{HttpUtility.UrlEncode(p.Key)}={HttpUtility.UrlEncode(p.Value)}"));
                }

                if (!string.IsNullOrEmpty(paras))
                {
                    if (url.Contains("?"))
                    {
                        url = $"{url}&{paras}";
                    }
                    else
                    {
                        url = $"{url}?{paras}";
                    }
                }                

                return await _httpClient.GetAsync(url);
            });
        }

        /// <summary>
        /// call Post method
        /// </summary>
        /// <param name="url"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PostAsync(string url, HttpContent content)
        {
            return await _pipeline.ExecuteAsync(async exec =>
            {
                return await _httpClient.PostAsync(url, content);
            });
        }

        /// <summary>
        /// call Put method
        /// </summary>
        /// <param name="url"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PutAsync(string url, HttpContent content)
        {
            return await _pipeline.ExecuteAsync(async exec =>
            {
                return await _httpClient.PutAsync(url, content);
            });
        }

        /// <summary>
        /// call Patch method
        /// </summary>
        /// <param name="url"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PatchAsync(string url, HttpContent content)
        {
            return await _pipeline.ExecuteAsync(async exec =>
            {
                var request = new HttpRequestMessage(new HttpMethod("PATCH"), url);
                request.Content = content;

                return await _httpClient.SendAsync(request);
            });
        }

        /// <summary>
        /// call Delete method
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> DeleteAsync(string url)
        {
            return await _pipeline.ExecuteAsync(async exec =>
            {
                return await _httpClient.DeleteAsync(url);
            });
        }
    }
}
