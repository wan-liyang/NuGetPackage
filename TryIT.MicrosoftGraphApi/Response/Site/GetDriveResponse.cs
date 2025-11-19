using Newtonsoft.Json;
using System;

namespace TryIT.MicrosoftGraphApi.Response.Site
{
    public class GetDriveResponse
    {
        public class CreatedBy
        {
            public User user { get; set; }
        }

        public class Group
        {
            public string email { get; set; }
            public string id { get; set; }
            public string displayName { get; set; }
        }

        public class LastModifiedBy
        {
            public User user { get; set; }
        }

        public class Owner
        {
            public Group group { get; set; }
        }

        public class Quota
        {
            public long deleted { get; set; }
            public long remaining { get; set; }
            public string state { get; set; }
            public long total { get; set; }
            public long used { get; set; }
        }

        public class Response
        {
            [JsonProperty("@odata.context")]
            public string odatacontext { get; set; }
            public DateTime createdDateTime { get; set; }
            public string description { get; set; }
            public string id { get; set; }
            public DateTime lastModifiedDateTime { get; set; }
            public string name { get; set; }
            public string webUrl { get; set; }
            public string driveType { get; set; }
            public CreatedBy createdBy { get; set; }
            public LastModifiedBy lastModifiedBy { get; set; }
            public Owner owner { get; set; }
            public Quota quota { get; set; }
        }

        public class User
        {
            public string displayName { get; set; }
            public string email { get; set; }
            public string id { get; set; }
        }
    }
}
