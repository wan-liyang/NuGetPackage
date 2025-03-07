﻿using Polly;
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
        private HttpClient _httpClient;

        /// <summary>
        /// retry results, capture each response or exception
        /// </summary>
        public List<RetryResult> RetryResults = new List<RetryResult>();

        /// <summary>
        /// init Api instance with HttpClient instance
        /// </summary>
        /// <param name="apiConfig"></param>
        public Api(ApiConfig apiConfig)
        {
            _httpClient = apiConfig.HttpClient;

            EnableRetry(apiConfig.EnableRetry);
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
            HttpClient client = new HttpClient(clientHandler);

            if (clientConfig.TimeoutSecond > 0)
            {
                client.Timeout = TimeSpan.FromSeconds(clientConfig.TimeoutSecond);
            }

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            ServicePointManager.SecurityProtocol = clientConfig.securityProtocolType;

            if (clientConfig.BasicAuth != null)
            {
                if (!string.IsNullOrEmpty(clientConfig.BasicAuth.Username) || !string.IsNullOrEmpty(clientConfig.BasicAuth.Password))
                {
                    string basicToken = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes($"{clientConfig.BasicAuth.Username}:{clientConfig.BasicAuth.Password}"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", basicToken);
                }
            }

            if (clientConfig.Headers != null && clientConfig.Headers.Count > 0)
            {
                foreach (var item in clientConfig.Headers)
                {
                    client.DefaultRequestHeaders.Remove(item.Key);
                    client.DefaultRequestHeaders.Add(item.Key, item.Value);
                }
            }

            _httpClient = client;

            EnableRetry(clientConfig.EnableRetry);
        }

        /// <summary>
        /// init retry pipeline
        /// </summary>
        /// <param name="isEnable"></param>
        private void EnableRetry(bool isEnable)
        {
            if (isEnable)
            {
                _pipeline = new ResiliencePipelineBuilder<HttpResponseMessage>()
                       .AddRetry(new RetryStrategyOptions<HttpResponseMessage>
                       {
                           ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                                .Handle<TaskCanceledException>(result => result.InnerException is TimeoutException) // handle timeout exception
                                .HandleResult(result => result.StatusCode == HttpStatusCode.BadGateway
                                  || result.StatusCode == HttpStatusCode.GatewayTimeout
                                  || result.StatusCode == HttpStatusCode.BadRequest), // handle specific response message

                           Delay = TimeSpan.FromSeconds(1),
                           MaxRetryAttempts = 3,
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

                               RetryResults.Add(new RetryResult
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
