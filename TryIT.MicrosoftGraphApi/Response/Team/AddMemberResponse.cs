using Newtonsoft.Json;
using System.Collections.Generic;

namespace TryIT.MicrosoftGraphApi.Response.Team
{
    internal class AddMemberResponse
    {
        internal class Response
        {
            [JsonProperty("@odata.type")]
            public string OdataType { get; set; }
            public string id { get; set; }
            public List<string> roles { get; set; }
            public string userId { get; set; }
            public string displayName { get; set; }
            public string email { get; set; }
        }
    }
}
