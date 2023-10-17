using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security;

namespace TryIT.RestApi.Models
{
    public class RequestModel
    {
        public RequestModel()
        {
        }

        /// <summary>
        /// request Url contains parameter if parameter pass via Url
        /// </summary>
        public string Url { get; set; }
        public Dictionary<string, string> Headers { get; set; }

        /// <summary>
        /// new StringContent(bodyString, System.Text.Encoding.UTF8, "application/json");
        /// </summary>
        public HttpContent HttpContent { get; set; }
    }
}
