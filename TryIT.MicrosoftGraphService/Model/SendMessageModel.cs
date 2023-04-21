using System;
using System.Collections.Generic;

namespace TryIT.MicrosoftGraphService.Model
{
    public class SendMessageModel
    {
        public SendMessageModel()
        {
            this.BodyContentType = BodyContentType.Text;
        }

        public string Subject { get; set; }
        public string Body { get; set; }
        /// <summary>
        /// contentType for Body, default Text
        /// </summary>
        public BodyContentType BodyContentType{ get; set; }
        public string[] ToRecipients { get; set; }
        public string[] CcRecipients { get; set; }
        public string[] BccRecipients { get; set; }
        public List<Attachment> Attachments { get; set; }
    }
    public enum BodyContentType
    {
        Text,
        Html
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

