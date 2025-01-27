using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.MicrosoftGraphApi.Model.Outlook
{
    public class MoveMessageModel
    {
        /// <summary>
        /// leave empty if get from current user
        /// </summary>
        public string mailbox { get; set; }

        /// <summary>
        /// the id of message to move
        /// </summary>
        public string messageId { get; set; }

        /// <summary>
        /// destination folder name, refer to here for well-know folder name
        /// <para>https://learn.microsoft.com/en-us/graph/api/resources/mailfolder?view=graph-rest-1.0</para>
        /// </summary>
        public string destinationFolder { get; set; }
    }
}
