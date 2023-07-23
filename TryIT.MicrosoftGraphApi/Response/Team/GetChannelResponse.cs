using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace TryIT.MicrosoftGraphApi.Response.Team
{
    internal class GetChannelResponse
    {
        public class Response
        {
            [JsonProperty("@odata.context")]
            public string odatacontext { get; set; }

            [JsonProperty("@odata.count")]
            public int odatacount { get; set; }
            public List<Channel> value { get; set; }
        }

        public class Channel
        {
            public string id { get; set; }
            public DateTime createdDateTime { get; set; }
            public string displayName { get; set; }
            public string description { get; set; }
            public object isFavoriteByDefault { get; set; }
            public string email { get; set; }
            public string tenantId { get; set; }
            public string webUrl { get; set; }
            public string membershipType { get; set; }
        }
    }
}
