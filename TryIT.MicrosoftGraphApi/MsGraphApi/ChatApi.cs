using System.Threading.Tasks;
using TryIT.MicrosoftGraphApi.HttpClientHelper;
using TryIT.MicrosoftGraphApi.HttpModel.Chat;
using TryIT.MicrosoftGraphApi.Model;

namespace TryIT.MicrosoftGraphApi.MsGraphApi
{
    /// <summary>
    /// chat api
    /// </summary>
    public class ChatApi
    {
        private readonly ChatHelper _helper;

        /// <summary>
        /// instantiate Chat api with configuration
        /// </summary>
        /// <param name="config"></param>
        public ChatApi(MsGraphApiConfig config)
        {
            _helper = new ChatHelper(config);
        }

        /// <summary>
        /// send one to one message to specified recipient,
        /// </summary>
        /// <param name="recipient"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<SendMessageResponse.Response> SendMessageAsync(string recipient, SendMessageRequest.Request request)
        {
            return await _helper.SendMessageAsync(new string[] { recipient }, request);
        }
    }
}
