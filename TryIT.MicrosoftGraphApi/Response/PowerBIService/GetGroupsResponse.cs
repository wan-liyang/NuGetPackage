using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace TryIT.MicrosoftGraphApi.Response.PowerBIService
{
    public class GetGroupsResponse
    {
        public class Group
        {
            /// <summary>
            /// 
            /// </summary>
            public string id { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string isReadOnly { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string isOnDedicatedCapacity { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string type { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string name { get; set; }
        }

        public class Response
        {
            /// <summary>
            /// 
            /// </summary>
            [JsonProperty("@odata.context")]
            public string odatacontext { get; set; }
            /// <summary>
            /// 
            /// </summary>
            [JsonProperty("@odata.count")]
            public int odatacount { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public List<Group> value { get; set; }
        }

    }
}
