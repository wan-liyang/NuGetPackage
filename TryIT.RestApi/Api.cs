using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace TryIT.RestApi
{
    /// <summary>
    /// configuration
    /// </summary>
    public class ApiConfig
    {
        /// <summary>
        /// the ready HttpClient with necessary information e.g. Proxy, Auth, Header
        /// </summary>
        public HttpClient HttpClient { get; set; }

        /// <summary>
        /// indicator whether enable retry (max 3 times) if respons status code is 
        /// <see cref="HttpStatusCode.GatewayTimeout"/>
        /// <see cref="HttpStatusCode.BadGateway"/>
        /// <see cref="HttpStatusCode.BadRequest"/>
        /// or timeout exception happen
        /// </summary>
        public bool EnableRetry { get; set; }
    }

    /// <summary>
    /// retry result
    /// </summary>
    public class RetryResult
    {
        /// <summary>
        /// number of retry
        /// </summary>
        public int AttemptNumber { get; set; }
        /// <summary>
        /// response message for each retry
        /// </summary>
        public HttpResponseMessage Result { get; set; }
        /// <summary>
        /// exception for each retry
        /// </summary>
        public Exception Exception { get; set; }
    }

    /// <summary>
    /// make a api call with retry
    /// </summary>
    public class Api
    {
        private readonly ResiliencePipeline<HttpResponseMessage> _pipeline;
        private HttpClient _httpClient;

        /// <summary>
        /// retry results, capture each response or exception
        /// </summary>
        public List<RetryResult> RetryResults = new List<RetryResult>();

        /// <summary>
        /// init Api instance with HttpClient
        /// </summary>
        /// <param name="apiConfig"></param>
        public Api(ApiConfig apiConfig)
        {
            _httpClient = apiConfig.HttpClient;

            if (apiConfig.EnableRetry)
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
                               RetryResults.Add(new RetryResult
                               {
                                   AttemptNumber = args.AttemptNumber,
                                   Result = args.Outcome.Result,
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
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> GetAsync(string url)
        {
            return await _pipeline.ExecuteAsync(async exec =>
            {
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
