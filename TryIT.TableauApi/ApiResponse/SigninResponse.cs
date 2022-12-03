using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.TableauApi.ApiResponse
{
    internal class SigninResponse
    {
        public class Credentials
        {
            public Site site { get; set; }
            public User user { get; set; }
            public string token { get; set; }
            public string estimatedTimeToExpiration { get; set; }
        }

        public class Response
        {
            public Credentials credentials { get; set; }
        }

        public class Site
        {
            public string id { get; set; }
            public string contentUrl { get; set; }
        }

        public class User
        {
            public string id { get; set; }
        }
    }
}
