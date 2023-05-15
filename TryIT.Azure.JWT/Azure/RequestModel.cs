using System;
namespace TryIT.JWT.Azure
{
	public class RequestModel
	{
		public RequestModel()
		{
            TokenUrl = "https://login.microsoftonline.com/{{tenant-id}}/oauth2/v2.0/token";
            GrantType = "client_credentials";
			Scope = "https://graph.microsoft.com/.default";
        }

        internal string TokenUrl { get; set; }
        public string TenantId { get; set; }
		public string ClientId { get; set; }
		public string ClientSecret { get; set; }

        /// <summary>
        /// default client_credentials
        /// </summary>
        public string GrantType { get; set; }
        /// <summary>
        /// default https://graph.microsoft.com/.default
        /// </summary>
        public string Scope { get; set; }


        public string Proxy_Url { get; set; }
        public string Proxy_Username { get; set; }
        public string Proxy_Password { get; set; }
    }
}

