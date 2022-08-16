using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MicrosoftGraphService.Model
{
    class SharePointRequestBody
    {
    }

    public enum ConflictBehavior
    {
        fail,
        replace,
        rename
    }

    public class CreateUploadSessionRequestBody
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
