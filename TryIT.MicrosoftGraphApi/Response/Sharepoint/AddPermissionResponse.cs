using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.MicrosoftGraphApi.Response.Sharepoint
{
    public class AddPermissionResponse
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

        public class User
        {
            public string email { get; set; }
            public string id { get; set; }
            public string displayName { get; set; }
        }

        public class Value
        {
            public string id { get; set; }
            public List<string> roles { get; set; }
            public GrantedTo grantedTo { get; set; }
        }
    }
}
