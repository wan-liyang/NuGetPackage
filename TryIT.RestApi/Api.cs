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
        private const string EXCEPTION_DATA_HTTP_METHOD = "HttpMethod";
        private const string EXCEPTION_DATA_HTTP_URI = "RequestUri";
        private const string EXCEPTION_DATA_RETRY_ATTEMPTS = "RetryAttempts";

        private ResiliencePipeline _pipeline;
        private readonly HttpClient _httpClient;
        private readonly HttpLogDelegate _httpLogDelegate;

        private readonly List<RetryResult> _retryResults = new List<RetryResult>();
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

            _httpLogDelegate = clientConfig.HttpLogDelegate;

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
            var Builder = GetBuilder(config.RetryProperty);

            if (Builder.EnableRetry)
            {
                _pipeline = new ResiliencePipelineBuilder()
                       .AddRetry(new RetryStrategyOptions
                       {
                           ShouldHandle = Builder.RetryBuilder,
                           Delay = config.RetryProperty.RetryDelay,
                           MaxRetryAttempts = config.RetryProperty.RetryCount,
                           BackoffType = DelayBackoffType.Constant,
                           OnRetry = args =>
                           {
                               UpdateRetryResult(args);
                               return default;
                           }
                       })
                       .Build();
            }
            else
            {
                _pipeline = new ResiliencePipelineBuilder().Build();
            }
        }

        /// <summary>
        /// get retry builder based on retry property
        /// </summary>
        /// <param name="retryProperty"></param>
        /// <returns></returns>
        private static (bool EnableRetry, PredicateBuilder RetryBuilder) GetBuilder(RetryProperty retryProperty)
        {
            if (retryProperty == null)
            {
                return (false, null);
            }

            bool _isRetryEnabled = false;

            var builder = new PredicateBuilder();
            if (retryProperty.RetryStatusCodes != null && retryProperty.RetryStatusCodes.Any())
            {
                _isRetryEnabled = true;

                builder.HandleResult(result =>
                {
                    if (result is HttpResponseMessage httpResponse)
                    {
                        return retryProperty.RetryStatusCodes.Contains(httpResponse.StatusCode);
                    }
                    return false;
                });
            }

            if (retryProperty.RetryExceptions != null && retryProperty.RetryExceptions.Any())
            {
                _isRetryEnabled = true;

                builder.Handle<Exception>((Func<Exception, bool>)(ex =>
                {
                    var httpMethod = ex.Data[(object)Api.EXCEPTION_DATA_HTTP_METHOD] as string;

                    if (httpMethod == HttpMethod.Get.Method
                        && retryProperty.RetryExceptions.Any(
                            retryEx => retryEx.ExceptionType.IsInstanceOfType(ex)
                            && (
                                string.IsNullOrEmpty(retryEx.MessageKeyword)
                                || ex.Message.ToUpper().Contains(retryEx.MessageKeyword.ToUpper())
                                ))
                        )
                    {
                        return true;
                    }

                    return false;
                }));
            }

            return (_isRetryEnabled, builder);
        }

        private void UpdateRetryResult(OnRetryArguments<object> args)
        {
            ResultMessage resultMessage = null;
            var result = args.Outcome.Result;

            if (result is HttpResponseMessage httpResponse)
            {
                resultMessage = new ResultMessage();
                resultMessage.StatusCode = $"{(int)httpResponse.StatusCode} - {httpResponse.StatusCode.ToString()}";
                resultMessage.ReasonPhrase = httpResponse.ReasonPhrase;
                resultMessage.RequestUri = httpResponse.RequestMessage.RequestUri;
            }

            _retryResults.Add(new RetryResult
            {
                AttemptNumber = args.AttemptNumber,
                Timestamp = DateTime.Now,
                Result = resultMessage,
                Exception = args.Outcome.Exception
            });
        }

        /// <summary>
        /// add extra data into exception
        /// <para>Uri</para>
        /// <para>Method</para>
        /// <para>RetryResults</para>
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="url"></param>
        private void AddExcetionData(Exception ex, string url)
        {
            ex.Data[EXCEPTION_DATA_HTTP_URI] = url;
            ex.Data[EXCEPTION_DATA_HTTP_METHOD] = HttpMethod.Get.Method;

            if (RetryResults.Any())
            {
                ex.Data[EXCEPTION_DATA_RETRY_ATTEMPTS] = RetryResults;
            }
        }

        /// <summary>
        /// call Get method
        /// </summary>
        /// <param name="url">url with or without parameters, if with parameters, the parameter key and value must UrlEncode</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> GetAsync(string url)
        {
            try
            {
                return await _pipeline.ExecuteAsync(async exec =>
                {
                    try
                    {
                        return await WrapSendAsync(async () =>
                        {
                            return await _httpClient.GetAsync(url);
                        },
                        url,
                        HttpMethod.Get);                        
                    }
                    catch (Exception ex)
                    {
                        ex.Data.Add((object)Api.EXCEPTION_DATA_HTTP_METHOD, HttpMethod.Get.Method);
                        throw;
                    }
                });
            }
            catch (Exception ex)
            {
                AddExcetionData(ex, url);
                throw;
            }
        }

        /// <summary>
        /// call Get method
        /// </summary>
        /// <param name="url">base url without parameters</param>
        /// <param name="parameters">url parameters</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> GetAsync(string url, Dictionary<string, string> parameters)
        {
            try
            {
                return await _pipeline.ExecuteAsync(async exec =>
                {
                    try
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

                        return await WrapSendAsync(async () =>
                        {
                            return await _httpClient.GetAsync(url);
                        },
                        url,
                        HttpMethod.Get);

                    }
                    catch (Exception ex)
                    {
                        ex.Data.Add((object)Api.EXCEPTION_DATA_HTTP_METHOD, HttpMethod.Get.Method);
                        throw;
                    }
                });
            }
            catch (Exception ex)
            {
                AddExcetionData(ex, url);
                throw;
            }
        }

        /// <summary>
        /// call Post method
        /// </summary>
        /// <param name="url"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PostAsync(string url, HttpContent content)
        {
            try
            {
                return await _pipeline.ExecuteAsync(async exec =>
                {
                    return await WrapSendAsync(async () =>
                    {
                        return await _httpClient.PostAsync(url, content);
                    },
                    url,
                    HttpMethod.Post);
                });
            }
            catch (Exception ex)
            {
                AddExcetionData(ex, url);
                throw;
            }
        }

        /// <summary>
        /// call Put method
        /// </summary>
        /// <param name="url"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PutAsync(string url, HttpContent content)
        {
            try
            {
                return await _pipeline.ExecuteAsync(async exec =>
                {
                    return await WrapSendAsync(async () =>
                    {
                        return await _httpClient.PutAsync(url, content);
                    },
                    url,
                    HttpMethod.Put);
                });
            }
            catch (Exception ex)
            {
                AddExcetionData(ex, url);
                throw;
            }
        }

        /// <summary>
        /// call Patch method
        /// </summary>
        /// <param name="url"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PatchAsync(string url, HttpContent content)
        {
            try
            {
                return await _pipeline.ExecuteAsync(async exec =>
                {
                    HttpMethod httpMethod = new HttpMethod("PATCH");
                    return await WrapSendAsync(async () =>
                    {
                        var request = new HttpRequestMessage(httpMethod, url);
                        request.Content = content;

                        return await _httpClient.SendAsync(request);
                    },
                    url,
                    httpMethod);
                });
            }
            catch (Exception ex)
            {
                AddExcetionData(ex, url);
                throw;
            }
        }

        /// <summary>
        /// call Delete method
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> DeleteAsync(string url)
        {
            try
            {
                return await _pipeline.ExecuteAsync(async exec =>
                {
                    return await WrapSendAsync(async () =>
                    {
                        return await _httpClient.DeleteAsync(url);
                    }, 
                    url, 
                    HttpMethod.Delete);
                });
            }
            catch (Exception ex)
            {
                AddExcetionData(ex, url);
                throw;
            }
        }

        /// <summary>
        /// wrap send async with logging
        /// </summary>
        /// <param name="action"></param>
        /// <param name="url"></param>
        /// <param name="httpMethod"></param>
        /// <returns></returns>
        private async Task<HttpResponseMessage> WrapSendAsync(Func<Task<HttpResponseMessage>> action, string url, HttpMethod httpMethod)
        {
            var start = DateTimeOffset.UtcNow;
            var correlationId = Guid.NewGuid().ToString();
            try
            {
                if (_httpLogDelegate != null)
                {
                    await _httpLogDelegate?.Invoke(new HttpLogContext
                    {
                        CorrelationId = correlationId,
                        Stage = LogStage.BeforeRequest,
                        Method = httpMethod,
                        Url = url,
                        StartTimeUtc = start
                    });
                }                

                var resonse = await action();

                if (_httpLogDelegate != null)
                {
                    await _httpLogDelegate?.Invoke(new HttpLogContext
                    {
                        CorrelationId = correlationId,
                        Stage = LogStage.AfterResponse,
                        Method = httpMethod,
                        Url = url,
                        Response = resonse,
                        StartTimeUtc = start,
                        EndTimeUtc = DateTimeOffset.UtcNow
                    });
                }

                return resonse;
            }
            catch (Exception ex)
            {
                if (_httpLogDelegate != null)
                {
                    await _httpLogDelegate?.Invoke(new HttpLogContext
                    {
                        CorrelationId = correlationId,
                        Stage = LogStage.OnError,
                        Method = httpMethod,
                        Url = url,
                        Exception = ex,
                        StartTimeUtc = start,
                        EndTimeUtc = DateTimeOffset.UtcNow
                    });
                }                    

                throw;
            }
        }
    }
}
