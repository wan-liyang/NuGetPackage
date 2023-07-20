using Newtonsoft.Json;
using System.Collections.Generic;
using TryIT.MicrosoftGraphService.Model.User;

namespace TryIT.MicrosoftGraphService.ApiModel.User
{
    internal class GetUserResponse
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
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
        }
    }

    internal static class Extension
    {
        public static UserModel ToUserModel(this GetUserResponse.User user)
        {
            UserModel module = new UserModel();
            if (user != null)
            {
                module.Id = user.id;
                module.EmailAddress = user.mail;
                module.Name = user.displayName;
            }
            return module;
        }
    }
}