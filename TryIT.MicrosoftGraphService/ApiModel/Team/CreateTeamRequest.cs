using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.MicrosoftGraphService.ApiModel.Team
{
    internal class CreateTeamRequest
    {
        [JsonProperty("template@odata.bind")]
        public string templateodatabind { get; set; }
        public string displayName { get; set; }
        public string description { get; set; }
    }
}
