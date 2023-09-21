using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace TryIT.MicrosoftGraphApi.Request.User
{
    internal class CreateInvitationRequest
    {
        internal class Body
        {
            public string invitedUserDisplayName { get; set; }
            public string invitedUserEmailAddress { get; set; }
            public string inviteRedirectUrl { get; set; }
            public bool sendInvitationMessage { get; set; }
        }
    }
}
