using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.MicrosoftGraphService.ApiModel
{
    internal class SharePointRequestBody
    {
    }

    internal enum ConflictBehavior
    {
        fail,
        replace,
        rename
    }

    internal class CreateUploadSessionRequestBody
    {
        /// <summary>
        /// file name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("@microsoft.graph.conflictBehavior")]
        public string ConflictBehavior { get; set; }
    }

    internal class MoveItemRequestBody
    {
        public ParentReference parentReference { get; set; }
        public string name { get; set; }
    }

    internal class ParentReference
    {
        public string id { get; set; }
    }
}
