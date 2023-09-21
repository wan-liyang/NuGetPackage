using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.MicrosoftGraphApi.Response.User
{
    public class CreateInvitationResponse
    {
        public class Response
        {
            [JsonProperty("@odata.context")]
            public string odatacontext { get; set; }
            public string id { get; set; }
            public string inviteRedeemUrl { get; set; }
            public string invitedUserDisplayName { get; set; }
            public string invitedUserEmailAddress { get; set; }
            public bool resetRedemption { get; set; }
            public bool sendInvitationMessage { get; set; }
            public InvitedUserMessageInfo invitedUserMessageInfo { get; set; }
            public string inviteRedirectUrl { get; set; }
            public string status { get; set; }
            public InvitedUser invitedUser { get; set; }
        }

        public class CcRecipient
        {
            public EmailAddress emailAddress { get; set; }
        }

        public class EmailAddress
        {
            public object name { get; set; }
            public object address { get; set; }
        }

        public class InvitedUser
        {
            public string id { get; set; }
        }

        public class InvitedUserMessageInfo
        {
            public object messageLanguage { get; set; }
            public List<CcRecipient> ccRecipients { get; set; }
            public object customizedMessageBody { get; set; }
        }
    }
}
