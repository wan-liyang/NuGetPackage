using TryIT.MicrosoftGraphService.ExtensionHelper;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace TryIT.MicrosoftGraphService.HttpClientHelper
{
    internal class BaseHelper
    {
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
