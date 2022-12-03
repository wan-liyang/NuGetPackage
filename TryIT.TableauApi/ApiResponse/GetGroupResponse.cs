using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.TableauApi.ApiResponse
{
    internal class GetGroupResponse
    {
        public class Domain
        {
            public string name { get; set; }
        }

        public class Group
        {
            public Domain domain { get; set; }
            public string id { get; set; }
            public string name { get; set; }
        }

        public class Groups
        {
            public List<Group> group { get; set; }
        }

        public class Pagination
        {
            public string pageNumber { get; set; }
            public string pageSize { get; set; }
            public string totalAvailable { get; set; }
        }

        public class Response
        {
            public Pagination pagination { get; set; }
            public Groups groups { get; set; }
        }
    }
}
