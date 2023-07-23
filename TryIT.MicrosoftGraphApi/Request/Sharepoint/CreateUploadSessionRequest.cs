using Newtonsoft.Json;

namespace TryIT.MicrosoftGraphApi.Request.Sharepoint
{
    internal class CreateUploadSessionRequest
    {
        internal class Body
        {
            /// <summary>
            /// file name
            /// </summary>
            [JsonProperty("name")]
            public string Name { get; set; }

            /// <summary>
            /// <see cref="ConflictBehaviorEnum"/>
            /// </summary>
            [JsonProperty("@microsoft.graph.conflictBehavior")]
            public string ConflictBehavior { get; set; }
        }
    }
}
