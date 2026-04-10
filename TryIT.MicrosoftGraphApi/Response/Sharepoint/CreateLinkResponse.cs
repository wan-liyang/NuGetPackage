using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.MicrosoftGraphApi.Response.Sharepoint
{
    public class CreateLinkResponse
    {
        public class Link
        {
            public string scope { get; set; }
            public string type { get; set; }
            public string webUrl { get; set; }
            public bool preventsDownload { get; set; }
        }

        public class Response
        {
            [JsonProperty("@odata.context")]
            public string odatacontext { get; set; }
            public string id { get; set; }
            public List<string> roles { get; set; }
            public string shareId { get; set; }
            public bool hasPassword { get; set; }
            public Link link { get; set; }
        }
    }
}
