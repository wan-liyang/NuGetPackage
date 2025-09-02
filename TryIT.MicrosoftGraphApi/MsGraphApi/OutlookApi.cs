using System.Collections.Generic;
using System.Threading.Tasks;
using TryIT.MicrosoftGraphApi.HttpClientHelper;
using TryIT.MicrosoftGraphApi.Model;
using TryIT.MicrosoftGraphApi.Model.Outlook;
using TryIT.MicrosoftGraphApi.Response.Outlook;

namespace TryIT.MicrosoftGraphApi.MsGraphApi
{
    /// <summary>
    /// API for Outlook Mail
    /// </summary>
    public class OutlookApi
    {
        private readonly OutlookHelper _helper;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public OutlookApi(MsGraphApiConfig config)
        {
            _helper = new OutlookHelper(config);
        }

        /// <summary>
        /// get message from the specified user's mailbox
        /// </summary>
        /// <param name="getMessageModel"></param>
        /// <returns></returns>
        public async Task<List<GetMessageResponse.Message>> GetMessagesAsync(GetMessageModel getMessageModel)
        {
            return await _helper.GetMessagesAsync(getMessageModel);
        }

        /// <summary>
        /// get a set of messages that have been added, deleted, or updated in a specified folder.
        /// <para>if <see cref="GetMessageResponse.Response.odatanextLink"/> is available, continues call <see cref="GetMessagesFromLinkAsync(string)"/> to get next page messages</para>
        /// <para>if <see cref="GetMessageResponse.Response.odatadeltaLink"/> is available, continues call <see cref="GetMessagesFromLinkAsync(string)"/> to get newly messages</para>
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<GetMessageResponse.Response> GetDeltaMessagesAsync(GetDeltaMessage model)
        {
            return await _helper.GetDeltaMessagesAsync(model);
        }

        /// <summary>
        /// get messages from the provided deltalink or nextlink
        /// </summary>
        /// <param name="deltalink"></param>
        /// <returns></returns>
        public async Task<GetMessageResponse.Response> GetMessagesFromLinkAsync(string deltalink)
        {
            return await _helper.GetMessagesFromLinkAsync(deltalink);
        }

        /// <summary>
        /// send message as current user
        /// </summary>
        /// <param name="message"></param>
        public async Task SendMessageAsync(SendMessageModel message)
        {
            await _helper.SendMessageAsync(message);
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

        /// <summary>
        /// Get the mail folder collection directly under the root folder
        /// <para>https://learn.microsoft.com/en-us/graph/api/user-list-mailfolders?view=graph-rest-1.0&tabs=http</para>
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<List<GetMailboxFolderResponse.Folder>> GetMaiboxFoldersAsync(GetMailboxFolderModel model)
        {
            return await _helper.GetMailboxFoldersAsync(model);
        }
    }
}
