using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.MicrosoftGraphApi.HttpModel.Chat
{
    public class SendMessageResponse
    {
        public class Body
        {
            public string contentType { get; set; }
            public string content { get; set; }
        }

        public class From
        {
            public object application { get; set; }
            public object device { get; set; }
            public object conversation { get; set; }
            public User user { get; set; }
        }

        public class Response
        {
            [JsonProperty("@odata.context")]
            public string odatacontext { get; set; }
            public string id { get; set; }
            public object replyToId { get; set; }
            public string etag { get; set; }
            public string messageType { get; set; }
            public DateTime createdDateTime { get; set; }
            public DateTime lastModifiedDateTime { get; set; }
            public object lastEditedDateTime { get; set; }
            public object deletedDateTime { get; set; }
            public object subject { get; set; }
            public object summary { get; set; }
            public string chatId { get; set; }
            public string importance { get; set; }
            public string locale { get; set; }
            public object webUrl { get; set; }
            public object channelIdentity { get; set; }
            public object policyViolation { get; set; }
            public object eventDetail { get; set; }
            public From from { get; set; }
            public Body body { get; set; }
            public List<object> attachments { get; set; }
            public List<object> mentions { get; set; }
            public List<object> reactions { get; set; }
            public List<object> messageHistory { get; set; }
        }

        public class User
        {
            public string id { get; set; }
            public string displayName { get; set; }
            public string userIdentityType { get; set; }
        }
    }
}
