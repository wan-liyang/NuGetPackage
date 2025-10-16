using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.MicrosoftGraphApi.Request.Site
{
    /// <summary>
    /// Request for creating a site
    /// </summary>
    public class CreateSiteRequest
    {
        /// <summary>
        /// Request body for creating a group
        /// <para>https://learn.microsoft.com/en-us/graph/api/group-post-groups?view=graph-rest-1.0&tabs=http</para>
        /// </summary>
        public class Request
        {
            public string displayName { get; set; }
            public string description { get; set; }

            /// <summary>
            /// Type of group to create. Possible values are: "Unified" for Microsoft 365 groups, "DynamicMembership" for groups with dynamic membership, or an empty list for security groups. Required.
            /// <para>https://learn.microsoft.com/en-us/graph/api/group-post-groups?view=graph-rest-1.0&tabs=http#grouptypes-options</para>
            /// </summary>
            internal List<string> groupTypes { get; set; }
            internal bool mailEnabled { get; set; }
            internal string mailNickname { get; set; }
            internal bool securityEnabled { get; set; }
            internal string visibility { get; set; }

            [JsonProperty("owners@odata.bind")]
            public List<string> ownersodatabind { get; set; }

            [JsonProperty("members@odata.bind")]
            public List<string> membersodatabind { get; set; }
        }
    }
}
