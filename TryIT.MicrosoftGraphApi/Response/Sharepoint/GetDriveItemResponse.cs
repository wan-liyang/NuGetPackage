using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace TryIT.MicrosoftGraphApi.Response.Sharepoint
{
    public class GetDriveItemResponse
    {
        public class Response
        {
            [JsonProperty("@odata.context")]
            public string OdataContext { get; set; }

            public List<Item> value { get; set; }
        }

        public class Item
        {
            [JsonProperty("@odata.context")]
            public string OdataContext { get; set; }

            [JsonProperty("@microsoft.graph.downloadUrl")]
            public string MicrosoftGraphDownloadUrl { get; set; }
            public DateTime? createdDateTime { get; set; }
            public string eTag { get; set; }
            public string id { get; set; }
            public DateTime? lastModifiedDateTime { get; set; }
            public string name { get; set; }
            public string webUrl { get; set; }
            public string cTag { get; set; }
            public Int64 size { get; set; }
            public User createdBy { get; set; }
            public User lastModifiedBy { get; set; }
            public ParentReference parentReference { get; set; }
            public File file { get; set; }
            public FileSystemInfo fileSystemInfo { get; set; }
            public Folder folder { get; set; }

            public class User
            {
                public string email { get; set; }
                public string displayName { get; set; }
            }

            public class ParentReference
            {
                public string driveId { get; set; }
                public string driveType { get; set; }
                public string id { get; set; }
                public string path { get; set; }
                public string siteId { get; set; }
            }

            public class Hashes
            {
                public string quickXorHash { get; set; }
            }

            public class File
            {
                public string mimeType { get; set; }
                public Hashes hashes { get; set; }
            }

            public class Folder
            {
                public int childCount { get; set; }
            }

            public class FileSystemInfo
            {
                public DateTime createdDateTime { get; set; }
                public DateTime lastModifiedDateTime { get; set; }
            }
        }
    }
}
