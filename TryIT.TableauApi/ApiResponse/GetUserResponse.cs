using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.TableauApi.ApiResponse
{
    internal class GetUserResponse
    {
        public class Response
        {
            public User user { get; set; }
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
    }
}
