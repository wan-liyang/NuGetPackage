using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using TryIT.MicrosoftGraphApi.Helper;

namespace TryIT.MicrosoftGraphApi.HttpClientHelper
{
    internal class BaseHelper
    {
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
