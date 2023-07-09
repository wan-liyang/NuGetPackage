using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TryIT.MicrosoftGraphService.Model;

namespace TryIT.MicrosoftGraphService.ApiModel.Team
{
    internal class GetJoinedTeamsResponse
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

    internal static class Extension
    {
        public static TeamModel.Team ToTeamModel(this GetJoinedTeamsResponse.Team response)
        {
            TeamModel.Team model = new TeamModel.Team();
            if (response != null)
            {
                model.Id = response.id;
                model.DisplayName = response.displayName;
            }
            return model;
        }
    }
}
