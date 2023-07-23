using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace TryIT.MicrosoftGraphService.ApiModel
{
    internal class SharePointResponse
    {
        public class GetSiteResponse
        {
            [JsonProperty("@odata.context")]
            public string OdataContext { get; set; }
            public DateTime createdDateTime { get; set; }
            public string description { get; set; }
            public string id { get; set; }
            public DateTime lastModifiedDateTime { get; set; }
            public string name { get; set; }
            public string webUrl { get; set; }
            public string displayName { get; set; }
            public Root root { get; set; }
            public SiteCollection siteCollection { get; set; }

            public class Root
            {
            }

            public class SiteCollection
            {
                public string hostname { get; set; }
            }
        }

        public class GetDriveItemListResponse
        {
            [JsonProperty("@odata.context")]
            public string OdataContext { get; set; }

            public List<GetDriveItemResponse> value { get; set; }
        }

        public class GetDriveItemResponse
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
            public int size { get; set; }
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

        public class GetDriveItemPreviewResponse
        {
            [JsonProperty("@odata.context")]
            public string OdataContext { get; set; }
            public string getUrl { get; set; }
            public string postParameters { get; set; }
            public string postUrl { get; set; }
        }

        public class CreateUploadSessionResponse
        {
            public string uploadUrl { get; set; }
            public DateTime expirationDateTime { get; set; }
        }
    }
}
