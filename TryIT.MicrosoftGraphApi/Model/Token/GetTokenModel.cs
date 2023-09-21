using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.MicrosoftGraphApi.Model.Token
{
    public class GetTokenModel
    {
        public string tenant_id { get; set; }
        /// <summary>
        /// client_credentials
        /// </summary>
        public string grant_type { get; set; }
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public string scope { get; set; }
    }
}
