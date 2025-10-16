using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.MicrosoftGraphApi.Request.Group
{
    public class CreateGroupRequest
    {
        /// <summary>
        /// Request body for creating a group
        /// <para>https://learn.microsoft.com/en-us/graph/api/group-post-groups</para>
        /// </summary>
        public class Request
        {
            public string displayName { get; set; }
            public string description { get; set; }

            /// <summary>
            /// Type of group to create. Possible values are: "Unified" for Microsoft 365 groups, "DynamicMembership" for groups with dynamic membership, or an empty list for security groups. Required.
            /// <para>https://learn.microsoft.com/en-us/graph/api/group-post-groups#grouptypes-options</para>
            /// </summary>
            public List<string> groupTypes { get; set; }
            public bool mailEnabled { get; set; }
            public string mailNickname { get; set; }
            public bool securityEnabled { get; set; }
            public string visibility { get; set; } = "private";

            [JsonProperty("owners@odata.bind", NullValueHandling = NullValueHandling.Ignore)]
            public List<string> ownersodatabind { get; set; }

            [JsonProperty("members@odata.bind", NullValueHandling = NullValueHandling.Ignore)]
            public List<string> membersodatabind { get; set; }
        }
    }
}
