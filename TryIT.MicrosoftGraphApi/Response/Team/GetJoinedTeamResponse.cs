using Newtonsoft.Json;
using System.Collections.Generic;

namespace TryIT.MicrosoftGraphApi.Response.Team
{
    public class GetJoinedTeamResponse
    {
        public class Response
        {
            [JsonProperty("@odata.context")]
            public string OdataContext { get; set; }

            [JsonProperty("@odata.count")]
            public int OdataCount { get; set; }
            public List<Team> value { get; set; }
        }

        public class Team
        {
            public string id { get; set; }
            public object createdDateTime { get; set; }
            public string displayName { get; set; }
            public string description { get; set; }
            public object internalId { get; set; }
            public object classification { get; set; }
            public object specialization { get; set; }
            public object visibility { get; set; }
            public object webUrl { get; set; }
            public bool isArchived { get; set; }
            public object isMembershipLimitedToOwners { get; set; }
            public object memberSettings { get; set; }
            public object guestSettings { get; set; }
            public object messagingSettings { get; set; }
            public object funSettings { get; set; }
            public object discoverySettings { get; set; }
        }
    }
}
