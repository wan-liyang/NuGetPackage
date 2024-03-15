using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace TryIT.MicrosoftGraphApi.Response.PowerBIService
{
    public class GetDatasetsInGroupResponse
    {
        public class QueryScaleOutSettings
        {
            /// <summary>
            /// 
            /// </summary>
            public string autoSyncReadOnlyReplicas { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int maxReadOnlyReplicas { get; set; }
        }

        public class Dataset
        {
            /// <summary>
            /// 
            /// </summary>
            public string id { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string name { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string webUrl { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string addRowsAPIEnabled { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string configuredBy { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string isRefreshable { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string isEffectiveIdentityRequired { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string isEffectiveIdentityRolesRequired { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string isOnPremGatewayRequired { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string targetStorageMode { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string createdDate { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string createReportEmbedURL { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string qnaEmbedURL { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public List<string> upstreamDatasets { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public List<string> users { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public QueryScaleOutSettings queryScaleOutSettings { get; set; }
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
            public List<Dataset> value { get; set; }
        }

    }
}
