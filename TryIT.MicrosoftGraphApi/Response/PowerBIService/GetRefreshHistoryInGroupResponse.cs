using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace TryIT.MicrosoftGraphApi.Response.PowerBIService
{
    public class GetRefreshHistoryInGroupResponse
    {
        public class RefreshAttemptsItem
        {
            /// <summary>
            /// 
            /// </summary>
            public int attemptId { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string startTime { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string endTime { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string type { get; set; }
        }

        public class RefreshHistory
        {
            /// <summary>
            /// 
            /// </summary>
            public string requestId { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public Int64 id { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string refreshType { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string startTime { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string endTime { get; set; }
            /// <summary>
            /// Unknown - if the completion state is unknown or a refresh is in progress.
            /// <para>Completed - for a successfully completed refresh.</para>
            /// <para>Failed for an unsuccessful refresh (serviceExceptionJson will contain the error code).</para>
            /// <para>Disabled if the refresh is disabled by a selective refresh.</para>
            /// </summary>
            public string status { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public List<RefreshAttemptsItem> refreshAttempts { get; set; }
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
            public List<RefreshHistory> value { get; set; }
        }

    }
}
