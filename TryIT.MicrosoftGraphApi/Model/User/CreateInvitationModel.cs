using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.MicrosoftGraphApi.Model.User
{
    public class CreateInvitationModel
    {
        public string UserDisplayName { get; set; }
        public string UserEmailAddress { get; set; }
        public string RedirectUrl { get; set; }
        public bool SendInvitationMessage { get; set; }
    }
}
