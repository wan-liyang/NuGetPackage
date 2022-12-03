using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.TableauApi.ApiResponse
{
    internal class AddUserResponse
    {        
        public class Response
        {
            public User user { get; set; }
        }

        public class User
        {
            public string authSetting { get; set; }
            public string externalAuthUserId { get; set; }
            public string id { get; set; }
            public string name { get; set; }
            public string siteRole { get; set; }
        }
    }
}
