using System.Collections.Generic;
using System.Security.Cryptography;
using TryIT.MicrosoftGraphApi.Helper;
using TryIT.MicrosoftGraphApi.HttpClientHelper;
using TryIT.MicrosoftGraphApi.Model;
using TryIT.MicrosoftGraphApi.Model.Outlook;
using TryIT.MicrosoftGraphApi.Response.Outlook;

namespace TryIT.MicrosoftGraphApi.MsGraphApi
{
    public class OutlookApi
    {
        private static OutlookHelper _helper;
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


        public void Move(string messageId, string destinationFolder)
        {

        }
    }
}
