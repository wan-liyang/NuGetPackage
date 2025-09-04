using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace TryIT.MicrosoftGraphApi.Response.Outlook
{
    public class GetMessageResponse
    {
        public class Body
        {
            public string contentType { get; set; }
            public string content { get; set; }
        }

        public class EmailAddress
        {
            public string name { get; set; }
            public string address { get; set; }
        }

        public class Flag
        {
            public string flagStatus { get; set; }
        }

        public class From
        {
            public EmailAddress emailAddress { get; set; }
        }

        /// <summary>
        /// api response for get messages
        /// </summary>
        public class Response
        {
            /// <summary>
            /// tells the "shape" of the response and what kind of resource the payload represents
            /// </summary>
            [JsonProperty("@odata.context")]
            public string odatacontext { get; set; }

            /// <summary>
            /// list of messages
            /// </summary>
            public List<Message> value { get; set; }

            /// <summary>
            /// mailbox pagination
            /// </summary>
            [JsonProperty("@odata.nextLink")]
            public string odatanextLink { get; set; }

            /// <summary>
            /// mailbox state as of now, use for delta query
            /// </summary>
            [JsonProperty("@odata.deltaLink")]
            public string odatadeltaLink { get; set; }
        }

        public class Sender
        {
            public EmailAddress emailAddress { get; set; }
        }

        public class Recipient
        {
            public EmailAddress emailAddress { get; set; }
        }
        public class Removed
        {
            public string reason { get; set; }
        }

        public class Message
        {
            [JsonProperty("@odata.etag")]
            public string odataetag { get; set; }
            public string id { get; set; }
            public DateTime createdDateTime { get; set; }
            public DateTime lastModifiedDateTime { get; set; }
            public string changeKey { get; set; }
            public List<object> categories { get; set; }
            public DateTime receivedDateTime { get; set; }
            public DateTime sentDateTime { get; set; }
            public bool hasAttachments { get; set; }
            public string internetMessageId { get; set; }
            public string subject { get; set; }
            public string bodyPreview { get; set; }
            public string importance { get; set; }
            public string parentFolderId { get; set; }
            public string conversationId { get; set; }
            public string conversationIndex { get; set; }
            public bool? isDeliveryReceiptRequested { get; set; }
            public bool isReadReceiptRequested { get; set; }
            public bool isRead { get; set; }
            public bool isDraft { get; set; }
            public string webLink { get; set; }
            public string inferenceClassification { get; set; }
            public Body body { get; set; }
            public Sender sender { get; set; }
            public From from { get; set; }
            public List<Recipient> toRecipients { get; set; }
            public List<Recipient> ccRecipients { get; set; }
            public List<Recipient> bccRecipients { get; set; }
            public List<object> replyTo { get; set; }
            public Flag flag { get; set; }

            [JsonProperty("@removed")]
            public Removed removed { get; set; }
        }
    }
}
