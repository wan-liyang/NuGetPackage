using System;
using System.Collections.Generic;
using System.Text;

namespace TryIT.MicrosoftGraphApi.HttpModel.Chat
{
    /// <summary>
    /// send message request
    /// </summary>
    public class SendMessageRequest
    {
        /// <summary>
        /// message body
        /// </summary>
        public class Body
        {
            /// <summary>
            /// message content
            /// </summary>
            public string content { get; set; }
        }

        /// <summary>
        /// message request
        /// </summary>
        public class Request
        {
            /// <summary>
            /// message body
            /// </summary>
            public Body body { get; set; }
        }
    }
}
