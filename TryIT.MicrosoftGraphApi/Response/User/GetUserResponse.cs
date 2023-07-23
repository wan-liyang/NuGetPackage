using Newtonsoft.Json;
using System.Collections.Generic;

namespace TryIT.MicrosoftGraphApi.Response.User
{
    public class GetUserResponse
    {
        public class Response
        {
            [JsonProperty("@odata.context")]
            public string odatacontext { get; set; }
            public List<User> value { get; set; }
        }

        public class User
        {
            public List<string> businessPhones { get; set; }
            public string displayName { get; set; }
            public string givenName { get; set; }
            public string jobTitle { get; set; }
            public string mail { get; set; }
            public object mobilePhone { get; set; }
            public string officeLocation { get; set; }
            public object preferredLanguage { get; set; }
            public string surname { get; set; }
            public string userPrincipalName { get; set; }
            public string id { get; set; }

            public Dictionary<string, object> AdditionalAttributes { get; set; }
        }
    }
}