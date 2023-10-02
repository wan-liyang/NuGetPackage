using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security;

namespace TryIT.RestApi
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

        public BasicAuth BasicAuth { get; set; }
        public Dictionary<string, string> Headers { get; set; }

        /// <summary>
        /// new StringContent(bodyString, System.Text.Encoding.UTF8, "application/json");
        /// </summary>
        public HttpContent HttpContent { get; set; }
    }

    public class BasicAuth
    {
        /// <summary>
        /// username for Basic Authorization, if provided will use Basic Authorization
        /// </summary>
        public string Username { get; set; }

        private static SecureString secureString;
        /// <summary>
        /// password for Basic Authorization, if provided will use Basic Authorization
        /// </summary>
        public string Password
        {
            get
            {
                return new System.Net.NetworkCredential(string.Empty, secureString).Password;
            }
            set
            {
                secureString = new NetworkCredential(string.Empty, value).SecurePassword;
            }
        }
    }
}
