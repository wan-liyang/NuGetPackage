using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.TableauApi.ApiResponse
{
    internal class GetGroupUserResponse
    {
        public class Pagination
        {
            public string pageNumber { get; set; }
            public string pageSize { get; set; }
            public string totalAvailable { get; set; }
        }

        public class Response
        {
            public Pagination pagination { get; set; }
            public Users users { get; set; }
        }

        public class User
        {
            public string externalAuthUserId { get; set; }
            public string id { get; set; }
            public DateTime lastLogin { get; set; }
            public string name { get; set; }
            public string siteRole { get; set; }
            public string locale { get; set; }
            public string language { get; set; }
        }

        public class Users
        {
            public List<User> user { get; set; }
        }
    }
}
