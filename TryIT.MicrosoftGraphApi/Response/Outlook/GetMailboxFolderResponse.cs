using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.MicrosoftGraphApi.Response.Outlook
{
    public class GetMailboxFolderResponse
    {
        public class Response
        {
            [JsonProperty("@odata.context")]
            public string odatacontext { get; set; }
            public List<Folder> value { get; set; }

            [JsonProperty("@odata.nextLink")]
            public string odatanextLink { get; set; }
        }

        public class Folder
        {
            public string id { get; set; }
            public string displayName { get; set; }
            public string parentFolderId { get; set; }
            public int childFolderCount { get; set; }
            public int unreadItemCount { get; set; }
            public int totalItemCount { get; set; }
            public Int64 sizeInBytes { get; set; }
            public bool isHidden { get; set; }
        }
    }
}
