using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace TryIT.MicrosoftGraphService.ApiModel
{
    internal class MailboxResponseList
    {
        [JsonProperty("@odata.context")]
        public string OdataContext { get; set; }

        [JsonProperty("@odata.nextLink")]
        public string OdataNextLink { get; set; }

        public List<MailboxResponse> value { get; set; }
    }

    internal class MessageRequestModel
    {
        public Message message { get; set; }

        public class Message
        {
            public string subject { get; set; }
            public Body body { get; set; }
            public List<Recipient> toRecipients { get; set; }
            public List<Recipient> ccRecipients { get; set; }
            public List<Recipient> bccRecipients { get; set; }
        }
    }

    internal class MailboxResponse
    {
        [JsonProperty("@odata.etag")]
        public string OdataEtag { get; set; }
        public string id { get; set; }
        public DateTime createdDateTime { get; set; }
        public DateTime lastModifiedDateTime { get; set; }
        public string changeKey { get; set; }
        public List<string> categories { get; set; }
        public DateTime receivedDateTime { get; set; }
        public DateTime sentDateTime { get; set; }
        public bool? hasAttachments { get; set; }
        public string internetMessageId { get; set; }
        public string subject { get; set; }
        public string bodyPreview { get; set; }
        public string importance { get; set; }
        public string parentFolderId { get; set; }
        public string conversationId { get; set; }
        public string conversationIndex { get; set; }
        public bool? isDeliveryReceiptRequested { get; set; }
        public bool? isReadReceiptRequested { get; set; }
        public bool? isRead { get; set; }
        public bool? isDraft { get; set; }
        public string webLink { get; set; }
        public string inferenceClassification { get; set; }
        public Body body { get; set; }
        public Recipient sender { get; set; }
        public Recipient from { get; set; }
        public List<Recipient> toRecipients { get; set; }
        public List<Recipient> ccRecipients { get; set; }
        public List<Recipient> bccRecipients { get; set; }
        public object replyTo { get; set; }
        public Flag flag { get; set; }

        public class DueDateTime
        {
            public DateTime? dateTime { get; set; }
            public string timeZone { get; set; }
        }

        public class StartDateTime
        {
            public DateTime? dateTime { get; set; }
            public string timeZone { get; set; }
        }

        public class Flag
        {
            public string flagStatus { get; set; }
            public DueDateTime dueDateTime { get; set; }
            public StartDateTime startDateTime { get; set; }
        }
    }

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
    public class Recipient
    {
        public EmailAddress emailAddress { get; set; }
    }
}
