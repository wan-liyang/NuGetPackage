using System;
using System.Collections.Generic;

namespace TryIT.MicrosoftGraphService.Model
{
    public class SendMessageModel
    {
        public string Subject { get; set; }
        public string Body { get; set; }
        public string[] ToRecipients { get; set; }
        public string[] CcRecipients { get; set; }
        public string[] BccRecipients { get; set; }
        public List<Attachment> Attachments { get; set; }
    }

    public class Attachment
    {
        public string FileName { get; set; }
        /// <summary>
        /// attachment byte data
        /// </summary>
        public byte[] FileContent { get; set; }
    }
}

