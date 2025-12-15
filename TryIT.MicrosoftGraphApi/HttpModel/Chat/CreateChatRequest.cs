using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.MicrosoftGraphApi.HttpModel.Chat
{
    internal class CreateChatRequest
    {
        public class Member
        {
            [JsonProperty("@odata.type")]
            public string odatatype { get; set; }
            public List<string> roles { get; set; }

            [JsonProperty("user@odata.bind")]
            public string userodatabind { get; set; }
        }

        public class Request
        {
            public string chatType { get; set; }
            public List<Member> members { get; set; }
        }
    }
}
