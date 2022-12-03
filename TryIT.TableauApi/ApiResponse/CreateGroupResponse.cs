using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.TableauApi.ApiResponse
{
    internal class CreateGroupResponse
    {
        public class Group
        {
            public string id { get; set; }
            public string name { get; set; }
        }

        public class Response
        {
            public Group group { get; set; }
        }
    }
}
