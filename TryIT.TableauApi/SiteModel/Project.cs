using System;
namespace TryIT.TableauApi.SiteModel
{
    public class Project
    {
        public string ownerId { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
        public string contentPermissions { get; set; }
    }
}

