using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using TryIT.MicrosoftGraphApi.Helper;

namespace TryIT.MicrosoftGraphApi.HttpClientHelper
{
    /// <summary>
    /// 
    /// </summary>
    public class BaseHelper
    {
        /// <summary>
        /// https://graph.microsoft.com/v1.0
        /// </summary>
        protected readonly string GraphApiRootUrl = "https://graph.microsoft.com/v1.0";

        /// <summary>
        /// add request header to HttpClient, to avoid duplicate header add into client
        /// </summary>
        /// <param name="client"></param>
        /// <param name="headerKey"></param>
        /// <param name="headerValue"></param>
        protected void AddDefaultRequestHeaders(HttpClient client, string headerKey, string headerValue)
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
        protected void CheckStatusCode(HttpResponseMessage responseMessage, List<TryIT.RestApi.Models.RetryResult> retryResults)
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
        protected void CheckStatusCode(HttpResponseMessage responseMessage)
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
        protected HttpContent GetJsonHttpContent(object source)
        {
            string jsonContent = source.ObjectToJson();

            HttpContent httpContent = new StringContent(jsonContent);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return httpContent;
        }
    }
}
