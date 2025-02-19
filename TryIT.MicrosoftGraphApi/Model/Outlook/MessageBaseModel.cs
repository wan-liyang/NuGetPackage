using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.MicrosoftGraphApi.Model.Outlook
{
    public class MessageBaseModel
    {
        /// <summary>
        /// leave empty if get from current user
        /// </summary>
        public string mailbox { get; set; }
    }
}
