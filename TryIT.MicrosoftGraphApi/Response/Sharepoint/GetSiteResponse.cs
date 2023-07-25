using Newtonsoft.Json;
using System;

namespace TryIT.MicrosoftGraphApi.Response.Sharepoint
{
    public class GetSiteResponse
    {
        public class Site
        {
            [JsonProperty("@odata.context")]
            public string odatacontext { get; set; }
            public DateTime createdDateTime { get; set; }
            public string description { get; set; }
            public string id { get; set; }
            public DateTime lastModifiedDateTime { get; set; }
            public string name { get; set; }
            public string webUrl { get; set; }
            public string displayName { get; set; }
            public Root root { get; set; }
            public SiteCollection siteCollection { get; set; }
        }

        public class Root
        {
        }

        public class SiteCollection
        {
            public string hostname { get; set; }
        }
    }
}
