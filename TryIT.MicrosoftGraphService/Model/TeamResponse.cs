using Newtonsoft.Json;
using System.Collections.Generic;

namespace TryIT.MicrosoftGraphService.Model
{
    public class TeamResponse
    {
        public class GetJoinedTeamsResponse
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

        public class AddMemberResponse
        {
            [JsonProperty("@odata.type")]
            public string OdataType { get; set; }
            public string id { get; set; }
            public List<string> roles { get; set; }
            public string userId { get; set; }
            public string displayName { get; set; }
            public string email { get; set; }
        }

        public class GetMembersResponse
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
