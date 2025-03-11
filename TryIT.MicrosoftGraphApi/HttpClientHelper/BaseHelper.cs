using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using TryIT.MicrosoftGraphApi.Helper;
using TryIT.MicrosoftGraphApi.Model;
using TryIT.RestApi.Models;

namespace TryIT.MicrosoftGraphApi.HttpClientHelper
{
    /// <summary>
    /// 
    /// </summary>
    public class BaseHelper
    {
        private readonly TryIT.RestApi.Api api;
        private readonly HttpClient _httpClient;

        /// <summary>
        /// https://graph.microsoft.com/v1.0
        /// </summary>
        private readonly string _graphApiRootUrl = "https://graph.microsoft.com/v1.0";

        internal HttpClient HttpClient
        {
            get
            {
                return _httpClient;
            }
        }
        internal RestApi.Api RestApi
        {
            get
            {
                return api;
            }
        }

        internal string GraphApiRootUrl
        {
            get
            {
                return _graphApiRootUrl;
            }
        }

        /// <summary>
        /// initialize base helper with HttpClient and RestApi instance
        /// </summary>
        /// <param name="config"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public BaseHelper(MsGraphApiConfig config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }
            _httpClient = new MsGraphHelper(config).GetHttpClient();
            api = GetRestApiInstance(_httpClient, config.RetryProperty);
        }

        /// <summary>
        /// add request header to HttpClient, to avoid duplicate header add into client
        /// </summary>
        /// <param name="client"></param>
        /// <param name="headerKey"></param>
        /// <param name="headerValue"></param>
        protected static void AddDefaultRequestHeaders(HttpClient client, string headerKey, string headerValue)
        {
            client.DefaultRequestHeaders.Remove(headerKey);
            client.DefaultRequestHeaders.Add(headerKey, headerValue);
        }

        /// <summary>
        /// check response status and throw exception if not success status code
        /// </summary>
        /// <param name="responseMessage"></param>
        /// <param name="retryResults"></param>
        /// <exception cref="Exception"></exception>
        protected static void CheckStatusCode(HttpResponseMessage responseMessage, List<TryIT.RestApi.Models.RetryResult> retryResults)
        {
            if (!responseMessage.IsSuccessStatusCode)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("Invalid Request");

                stringBuilder.Append("StatusCode: ");
                stringBuilder.AppendLine(responseMessage.StatusCode.ToString());

                stringBuilder.Append("Response Content: ");
                stringBuilder.AppendLine(responseMessage.Content.ReadAsStringAsync().Result);

                if (retryResults != null && retryResults.Count > 0)
                {
                    stringBuilder.Append("Retry Result: ");
                    string json = retryResults.ObjectToJson();
                    stringBuilder.Append(json);
                }

                throw new Exception(stringBuilder.ToString());
            }
        }

        /// <summary>
        /// check response status and throw exception if not success status code
        /// </summary>
        /// <param name="responseMessage"></param>
        /// <exception cref="Exception"></exception>
        protected static void CheckStatusCode(HttpResponseMessage responseMessage)
        {
            if (!responseMessage.IsSuccessStatusCode)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("Invalid Request");

                stringBuilder.Append("StatusCode: ");
                stringBuilder.AppendLine(responseMessage.StatusCode.ToString());

                stringBuilder.Append("Response Content: ");
                stringBuilder.AppendLine(responseMessage.Content.ReadAsStringAsync().Result);

                throw new Exception(stringBuilder.ToString());
            }
        }

        /// <summary>
        /// get application/json HttpContent
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        protected static HttpContent GetJsonHttpContent(object source)
        {
            string jsonContent = source.ObjectToJson();

            HttpContent httpContent = new StringContent(jsonContent);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return httpContent;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected static string EscapeExpression(string expression)
        {
            return Uri.EscapeDataString(expression);
        }

        /// <summary>
        /// get rest api instance, with retry mechanism
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="retryProperty"></param>
        /// <returns></returns>
        protected RestApi.Api GetRestApiInstance(HttpClient httpClient, ApiRetryProperty retryProperty)
        {
            var config = new HttpClientConfig
            {
                HttpClient = httpClient,
            };

            if (retryProperty != null)
            {
                config.RetryCount = retryProperty.RetryCount;
                config.RetryDelay = retryProperty.RetryDelay;
                config.RetryStatusCodes = retryProperty.RetryStatusCodes;
            }

            return new RestApi.Api(new HttpClientConfig
            {
                HttpClient = httpClient,
            });
        }
    }
}
