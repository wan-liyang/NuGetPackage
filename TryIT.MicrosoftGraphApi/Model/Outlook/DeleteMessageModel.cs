using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.MicrosoftGraphApi.Model.Outlook
{
    public class DeleteMessageModel : MessageBaseModel
    {
        /// <summary>
        /// the Id of the message
        /// </summary>
        public string messageId { get; set; }
    }
}
