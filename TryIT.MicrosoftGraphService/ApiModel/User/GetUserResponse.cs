using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TryIT.MicrosoftGraphService.Model.User;

namespace TryIT.MicrosoftGraphService.ApiModel.User
{
    internal class GetUserResponse
    {
        public class Response
        {
            [JsonProperty("@odata.context")]
            public string OdataContext { get; set; }
            public List<object> businessPhones { get; set; }
            public string displayName { get; set; }
            public string givenName { get; set; }
            public string jobTitle { get; set; }
            public string mail { get; set; }
            public string mobilePhone { get; set; }
            public string officeLocation { get; set; }
            public object preferredLanguage { get; set; }
            public string surname { get; set; }
            public string userPrincipalName { get; set; }
            public string id { get; set; }
        }
    }

        internal static class Extension
        {
            public static UserModel ToUserModel(this GetUserResponse.Response user)
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
