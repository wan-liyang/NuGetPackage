using Newtonsoft.Json;
using System.Collections.Generic;

namespace TryIT.MicrosoftGraphApi.Request.Outlook
{
    internal class SendMessageRequest
    {
        public class Request
        {
            public Message message { get; set; }

            /// <summary>
            /// Indicates whether to save the message in Sent Items. Specify it only if the parameter is false; default is true. Optional. 
            /// </summary>
            public bool saveToSentItems { get; set; }
        }

        public class Message
        {
            public string subject { get; set; }
            public Body body { get; set; }
            public List<Recipient> toRecipients { get; set; }
            public List<Recipient> ccRecipients { get; set; }
            public List<Recipient> bccRecipients { get; set; }
            public List<Attachment> attachments { get; set; }
        }

        public class Body
        {
            public string contentType { get; set; }
            public string content { get; set; }
        }

        public class Recipient
        {
            public EmailAddress emailAddress { get; set; }
        }

        public class EmailAddress
        {
            public string address { get; set; }
        }
        public class Attachment
        {
            [JsonProperty("@odata.type")]
            public string odatatype { get; set; }
            public string name { get; set; }
            public string contentType { get; set; }
            public string contentBytes { get; set; }
        }
    }
}
