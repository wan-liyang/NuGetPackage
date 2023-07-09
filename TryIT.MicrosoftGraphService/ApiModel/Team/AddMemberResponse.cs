using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.MicrosoftGraphService.ApiModel.Team
{
    internal class AddMemberResponse
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
