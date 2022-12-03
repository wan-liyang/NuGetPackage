using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.TableauApi.ApiResponse
{
    internal class GetProjectPermssionResponse
    {
        public class Capabilities
        {
            public List<Capability> capability { get; set; }
        }

        public class Capability
        {
            public string name { get; set; }
            public string mode { get; set; }
        }

        public class GranteeCapability
        {
            public Group group { get; set; }
            public Capabilities capabilities { get; set; }
            public User user { get; set; }
        }

        public class Group
        {
            public string id { get; set; }
        }

        public class Owner
        {
            public string id { get; set; }
        }

        public class Permissions
        {
            public Project project { get; set; }
            public List<GranteeCapability> granteeCapabilities { get; set; }
        }

        public class Project
        {
            public Owner owner { get; set; }
            public string id { get; set; }
            public string name { get; set; }
        }

        public class Response
        {
            public Permissions permissions { get; set; }
        }

        public class User
        {
            public string id { get; set; }
        }
    }
}
