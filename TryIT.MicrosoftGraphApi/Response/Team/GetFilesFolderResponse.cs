using Newtonsoft.Json;
using System;

namespace TryIT.MicrosoftGraphApi.Response.Team
{
    public class GetFilesFolderResponse
    {
        public class Response
        {
            [JsonProperty("@odata.context")]
            public string odatacontext { get; set; }
            public string id { get; set; }
            public DateTime createdDateTime { get; set; }
            public DateTime lastModifiedDateTime { get; set; }
            public string name { get; set; }
            public string webUrl { get; set; }
            public int size { get; set; }
            public ParentReference parentReference { get; set; }
            public FileSystemInfo fileSystemInfo { get; set; }
            public Folder folder { get; set; }
        }
        public class FileSystemInfo
        {
            public DateTime createdDateTime { get; set; }
            public DateTime lastModifiedDateTime { get; set; }
        }

        public class Folder
        {
            public int childCount { get; set; }
        }

        public class ParentReference
        {
            public string driveId { get; set; }
            public string driveType { get; set; }
        }
    }
}
