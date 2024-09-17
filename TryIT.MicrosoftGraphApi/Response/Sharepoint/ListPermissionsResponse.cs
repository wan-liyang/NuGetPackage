using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.MicrosoftGraphApi.Response.Sharepoint
{
    public class ListPermissionsResponse
    {
        public class Response
        {
            [JsonProperty("@odata.context")]
            public string odatacontext { get; set; }
            public List<Value> value { get; set; }
        }
        public class GrantedTo
        {
            public User user { get; set; }
        }

        public class GrantedToV2
        {
            public SiteGroup siteGroup { get; set; }
            public User user { get; set; }
            public SiteUser siteUser { get; set; }
            public Group group { get; set; }
        }

        public class Group
        {
            [JsonProperty("@odata.type")]
            public string odatatype { get; set; }
            public string displayName { get; set; }
            public string email { get; set; }
            public string id { get; set; }
        }

        public class SiteGroup
        {
            public string displayName { get; set; }
            public string id { get; set; }
            public string loginName { get; set; }
        }

        public class SiteUser
        {
            public string displayName { get; set; }
            public string email { get; set; }
            public string id { get; set; }
            public string loginName { get; set; }
        }

        public class User
        {
            [JsonProperty("@odata.type")]
            public string odatatype { get; set; }
            public string displayName { get; set; }
            public string email { get; set; }
            public string id { get; set; }
        }

        public class Value
        {
            public string id { get; set; }
            public List<string> roles { get; set; }
            public string shareId { get; set; }
            public GrantedToV2 grantedToV2 { get; set; }
            public GrantedTo grantedTo { get; set; }
        }


    }
}
