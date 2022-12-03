using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.TableauApi.ApiResponse
{
    internal class GetProjectResponse
    {
        public class Owner
        {
            public string id { get; set; }
        }

        public class Pagination
        {
            public string pageNumber { get; set; }
            public string pageSize { get; set; }
            public string totalAvailable { get; set; }
        }

        public class Project
        {
            public Owner owner { get; set; }
            public string id { get; set; }
            public string name { get; set; }
            public string description { get; set; }
            public DateTime createdAt { get; set; }
            public DateTime updatedAt { get; set; }
            public string contentPermissions { get; set; }
        }

        public class Projects
        {
            public List<Project> project { get; set; }
        }

        public class Response
        {
            public Pagination pagination { get; set; }
            public Projects projects { get; set; }
        }
    }
}
