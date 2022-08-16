﻿using Newtonsoft.Json;
using static MicrosoftGraphService.Model.SharePointResponse.GetDriveItemResponse;

namespace MicrosoftGraphService.Model
{
    public class SharePointCreateFolderModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        /// <summary>
        /// The conflict resolution behavior for actions that create a new item. You can use the values fail, replace, or rename. The default for PUT is replace. An item will never be returned with this annotation. Write-only.
        /// </summary>
        [JsonProperty("@microsoft.graph.conflictBehavior")]
        public string ConflictBehavior { get; set; }

        [JsonProperty("folder")]
        public Folder Folder { get; set; }
    }
}
