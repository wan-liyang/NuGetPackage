using Newtonsoft.Json;

namespace TryIT.MicrosoftGraphService.ApiModel
{
    internal class TeamAddMemberModel
    {
        [JsonProperty("@odata.type")]
        public string Type
        {
            get
            {
                return "#microsoft.graph.aadUserConversationMember";
            }
        }
        [JsonProperty("roles")]
        public string Roles { get; set; }
        public string EmailAddress { get; set; }

        [JsonProperty("user@odata.bind")]
        public string UserODataBind
        {
            get
            {
                return $"https://graph.microsoft.com/v1.0/users('{EmailAddress}')";
            }
        }
    }
}
