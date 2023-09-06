using System.Collections.Generic;

namespace TryIT.MicrosoftGraphApi.Model.Outlook
{
    public class SendMessageModel
    {
        public SendMessageModel()
        {
            this.BodyContentType = BodyContentType.Text;
            this.SaveToSentItems = true;
        }

        /// <summary>
        /// indicator the from address, leave empty if send as current user
        /// </summary>
        public string From { get; set; }
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

        /// <summary>
        /// Indicates whether to save the message in Sent Items. Specify it only if the parameter is false; default is true. Optional. 
        /// </summary>
        public bool SaveToSentItems { get; set; }
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

