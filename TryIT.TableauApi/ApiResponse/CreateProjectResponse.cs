using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.TableauApi.ApiResponse
{
    internal class CreateProjectResponse
    {
        public class Owner
        {
            public string id { get; set; }
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

        public class Response
        {
            public Project project { get; set; }
        }
    }
}
