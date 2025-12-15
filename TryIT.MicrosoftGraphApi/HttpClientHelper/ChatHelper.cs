using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TryIT.MicrosoftGraphApi.Helper;
using TryIT.MicrosoftGraphApi.HttpModel.Chat;
using TryIT.MicrosoftGraphApi.Model;

namespace TryIT.MicrosoftGraphApi.HttpClientHelper
{
    internal class ChatHelper : BaseHelper
    {
        private readonly string _me;
        public ChatHelper(MsGraphApiConfig config) : base(config)
        {
            var userApi = new UserHelper(config);
            _me = userApi.GetMe().userPrincipalName;
        }

        private async Task<CreateChatResponse.Response> CreateChatAsync(string[] recipients)
        {
            var members = new List<CreateChatRequest.Member>();

            foreach (var item in recipients)
            {
                members.Add(new CreateChatRequest.Member
                {
                    odatatype = "#microsoft.graph.aadUserConversationMember",
                    roles = new List<string> { "owner" },
                    userodatabind = $"{GraphApiRootUrl}/users('{item}')"
                });
            }

            if (!recipients.Contains(_me))
            {
                members.Add(new CreateChatRequest.Member
                {
                    odatatype = "#microsoft.graph.aadUserConversationMember",
                    roles = new List<string> { "owner" },
                    userodatabind = $"{GraphApiRootUrl}/users('{_me}')"
                });
            }

            var request = new CreateChatRequest.Request
            {
                chatType = members.Count == 2 ? "oneOnOne" : "group",
                members = members
            };

            string url = $"{GraphApiRootUrl}/chats";

            var httpContent = GetJsonHttpContent(request);
            var response = await RestApi.PostAsync(url, httpContent);
            CheckStatusCode(response);
            string content = await response.Content.ReadAsStringAsync();
            return content.JsonToObject<CreateChatResponse.Response>();
        }

        public async Task<SendMessageResponse.Response> SendMessageAsync(string[] recipients, SendMessageRequest.Request request)
        {
            var chat = await CreateChatAsync(recipients);

            string url = $"{GraphApiRootUrl}/chats/{chat.id}/messages";

            var httpContent = GetJsonHttpContent(request);
            var response = await RestApi.PostAsync(url, httpContent);
            CheckStatusCode(response);
            string content = await response.Content.ReadAsStringAsync();
            return content.JsonToObject<SendMessageResponse.Response>();
        }
    }
}
