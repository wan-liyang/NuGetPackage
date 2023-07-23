using Newtonsoft.Json;
using System.Collections.Generic;

namespace TryIT.MicrosoftGraphApi.Request.Team
{
    internal class AddMemberRequest
    {
        internal class Body
        {
            [JsonProperty("@odata.type")]
            public string odatatype { get; set; }
            public List<string> roles { get; set; }

            [JsonProperty("user@odata.bind")]
            public string userodatabind { get; set; }
        }
    }
}
