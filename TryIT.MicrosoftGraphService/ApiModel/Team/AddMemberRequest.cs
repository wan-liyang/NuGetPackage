using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.MicrosoftGraphService.ApiModel.Team
{
    internal class AddMemberRequest
    {
        [JsonProperty("@odata.type")]
        public string odatatype { get; set; }
        public List<string> roles { get; set; }

        [JsonProperty("user@odata.bind")]
        public string userodatabind { get; set; }
    }
}
