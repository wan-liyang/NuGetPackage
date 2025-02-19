using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.MicrosoftGraphApi.Model.Outlook
{
    /// <summary>
    /// get message MIME content
    /// </summary>
    public class GetMIMEContentModel : MessageBaseModel
    {
        /// <summary>
        /// the Id of the message
        /// </summary>
        public string messageId { get; set; }
    }
}
