using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.MicrosoftGraphApi.HttpModel.Chat
{
    internal class CreateChatResponse
    {
        public class Response
        {
            [JsonProperty("@odata.context")]
            public string odatacontext { get; set; }
            public string id { get; set; }
            public object topic { get; set; }
            public DateTime createdDateTime { get; set; }
            public DateTime lastUpdatedDateTime { get; set; }
            public string chatType { get; set; }
        }
    }
}
