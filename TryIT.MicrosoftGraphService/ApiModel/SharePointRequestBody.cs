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
}
