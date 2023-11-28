using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.MicrosoftGraphApi.Response.SiteList
{
    public class GetItemResponse
    {
        public class Response
        {
            [JsonProperty("@odata.context")]
            public string odatacontext { get; set; }
            public List<Item> value { get; set; }
        }

        public class ContentType
        {
            public string id { get; set; }
            public string name { get; set; }
        }

        public class CreatedBy
        {
            public User user { get; set; }
        }

        public class Fields
        {
            [JsonProperty("@odata.etag")]
            public string odataetag { get; set; }
            public string Title { get; set; }
            public string id { get; set; }
            public string ContentType { get; set; }
            public DateTime Modified { get; set; }
            public DateTime Created { get; set; }
            public string AuthorLookupId { get; set; }
            public string EditorLookupId { get; set; }
            public string _UIVersionString { get; set; }
            public bool Attachments { get; set; }
            public string Edit { get; set; }
            public string LinkTitleNoMenu { get; set; }
            public string LinkTitle { get; set; }
            public string ItemChildCount { get; set; }
            public string FolderChildCount { get; set; }
            public string _ComplianceFlags { get; set; }
            public string _ComplianceTag { get; set; }
            public string _ComplianceTagWrittenTime { get; set; }
            public string _ComplianceTagUserId { get; set; }
        }

        public class LastModifiedBy
        {
            public User user { get; set; }
        }

        public class ParentReference
        {
            public string id { get; set; }
            public string siteId { get; set; }
        }

        public class User
        {
            public string email { get; set; }
            public string id { get; set; }
            public string displayName { get; set; }
        }

        public class Item
        {
            [JsonProperty("@odata.etag")]
            public string odataetag { get; set; }
            public DateTime createdDateTime { get; set; }
            public string eTag { get; set; }
            public string id { get; set; }
            public DateTime lastModifiedDateTime { get; set; }
            public string webUrl { get; set; }
            public CreatedBy createdBy { get; set; }
            public LastModifiedBy lastModifiedBy { get; set; }
            public ParentReference parentReference { get; set; }
            public ContentType contentType { get; set; }

            [JsonProperty("fields@odata.context")]
            public string fieldsodatacontext { get; set; }
            public Fields fields { get; set; }
        }
    }
}
