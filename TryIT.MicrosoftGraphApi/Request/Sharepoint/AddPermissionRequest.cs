using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.MicrosoftGraphApi.Request.Sharepoint
{
    internal class AddPermissionRequest
    {
        public class Recipient
        {
            public string email { get; set; }
        }

        public class Body
        {
            public List<Recipient> recipients { get; set; }
            //public string message { get; set; }
            public bool requireSignIn { get; set; }
            public bool sendInvitation { get; set; }
            public List<string> roles { get; set; }
            //public string password { get; set; }
            //public string expirationDateTime { get; set; }
        }
    }
}
