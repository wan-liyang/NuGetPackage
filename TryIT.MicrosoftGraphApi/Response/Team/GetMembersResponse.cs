using Newtonsoft.Json;
using System.Collections.Generic;

namespace TryIT.MicrosoftGraphApi.Response.Team
{
    internal class GetMembersResponse
    {
        public class Response
        {
            [JsonProperty("@odata.context")]
            public string OdataContext { get; set; }

            [JsonProperty("@odata.count")]
            public int OdataCount { get; set; }
            public List<Member> value { get; set; }
        }

        public class Member
        {
            [JsonProperty("@odata.type")]
            public string OdataType { get; set; }
            public string id { get; set; }
            public List<string> roles { get; set; }
            public string displayName { get; set; }
            public string userId { get; set; }
            public string email { get; set; }
        }
    }
}
