using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.MicrosoftGraphApi.Response.SiteList
{
    public class GetListResponse
    {
        public class Response
        {
            [JsonProperty("@odata.context")]
            public string odatacontext { get; set; }
            public List<SiteList> value { get; set; }
        }
        public class CreatedBy
        {
            public User user { get; set; }
        }

        public class List
        {
            public bool contentTypesEnabled { get; set; }
            public bool hidden { get; set; }
            public string template { get; set; }
        }

        public class ParentReference
        {
            public string siteId { get; set; }
        }

        public class User
        {
            public string displayName { get; set; }
            public string email { get; set; }
            public string id { get; set; }
        }

        public class SiteList
        {
            [JsonProperty("@odata.etag")]
            public string odataetag { get; set; }
            public DateTime createdDateTime { get; set; }
            public string description { get; set; }
            public string eTag { get; set; }
            public string id { get; set; }
            public DateTime lastModifiedDateTime { get; set; }
            public string name { get; set; }
            public string webUrl { get; set; }
            public string displayName { get; set; }
            public CreatedBy createdBy { get; set; }
            public ParentReference parentReference { get; set; }
            public List list { get; set; }
        }
    }
}
