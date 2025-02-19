using System.Collections.Generic;
using System.Threading.Tasks;
using TryIT.MicrosoftGraphApi.Helper;
using TryIT.MicrosoftGraphApi.HttpClientHelper;
using TryIT.MicrosoftGraphApi.Model;
using TryIT.MicrosoftGraphApi.Model.Outlook;
using TryIT.MicrosoftGraphApi.Response.Outlook;

namespace TryIT.MicrosoftGraphApi.MsGraphApi
{
    /// <summary>
    /// 
    /// </summary>
    public class OutlookApi
    {
        private static OutlookHelper _helper;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public OutlookApi(MsGraphApiConfig config)
        {
            MsGraphHelper graphHelper = new MsGraphHelper(config);
            _helper = new OutlookHelper(graphHelper.GetHttpClient());
        }

        /// <summary>
        /// get message from inbox folder
        /// </summary>
        /// <returns></returns>
        public List<GetMessageResponse.Message> GetMessages()
        {
            return _helper.GetMessages();
        }

        public List<GetMessageResponse.Message> GetMessages(GetMessageModel getMessageModel)
        {
            return _helper.GetMessages(getMessageModel);
        }

        /// <summary>
        /// send message as current user
        /// </summary>
        /// <param name="message"></param>
        public void SendMessage(SendMessageModel message)
        {
            _helper.SendMessage(message);
        }

        /// <summary>
        /// Get MIME content
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<string> GetMIMEContentAsync(GetMIMEContentModel model)
        {
            return await _helper.GetMIMEContentAsync(model);
        }

        /// <summary>
        /// Move a message to another folder within the specified user's mailbox. This creates a new copy of the message in the destination folder and removes the original message.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<GetMessageResponse.Message> MoveMessageAsync(MoveMessageModel model)
        {
            return await _helper.MoveMessageAsync(model);
        }

        /// <summary>
        /// Delete a message in the specified user's mailbox
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task DeleteMessageAsync(DeleteMessageModel model)
        {
            await _helper.DeleteMessageAsync(model);
        }
    }
}
