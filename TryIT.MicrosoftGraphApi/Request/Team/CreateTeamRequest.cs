using Newtonsoft.Json;

namespace TryIT.MicrosoftGraphApi.Request.Team
{
    internal class CreateTeamRequest
    {
        internal class Body
        {
            [JsonProperty("template@odata.bind")]
            public string templateodatabind { get; set; }
            public string displayName { get; set; }
            public string description { get; set; }
        }
    }
}
