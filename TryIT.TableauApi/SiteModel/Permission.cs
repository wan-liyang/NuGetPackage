using System;
using System.Collections.Generic;

namespace TryIT.TableauApi.SiteModel
{
    public class Permission
    {
        public Project project { get; set; }
        public List<GranteeCapability> granteeCapabilities { get; set; }

        public class Capability
        {
            public string name { get; set; }
            public string mode { get; set; }
        }

        public class GranteeCapability
        {
            public string groupId { get; set; }
            public List<Capability> capabilities { get; set; }
            public string userId { get; set; }
        }

        public class Project
        {
            public string ownerId { get; set; }
            public string id { get; set; }
            public string name { get; set; }
        }
    }
}

