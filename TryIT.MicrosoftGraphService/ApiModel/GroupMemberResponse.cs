using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.MicrosoftGraphService.ApiModel
{
    internal class GroupMemberResponse
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class GetGroupMemberResponse
        {
            [JsonProperty("@odata.context")]
            public string odatacontext { get; set; }

            [JsonProperty("@odata.nextLink")]
            public string odatanextLink { get; set; }

            [JsonProperty("@odata.count")]
            public int odatacount { get; set; }
            public List<Member> value { get; set; }
        }

        public class Member
        {
            [JsonProperty("@odata.type")]
            public string odatatype { get; set; }
            public string id { get; set; }
            public List<string> businessPhones { get; set; }
            public string displayName { get; set; }
            public string givenName { get; set; }
            public string jobTitle { get; set; }
            public string mail { get; set; }
            public string mobilePhone { get; set; }
            public string officeLocation { get; set; }
            public object preferredLanguage { get; set; }
            public string surname { get; set; }
            public string userPrincipalName { get; set; }
        }
    }
}
