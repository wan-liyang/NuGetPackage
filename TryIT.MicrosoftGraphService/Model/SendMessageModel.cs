using System;
namespace TryIT.MicrosoftGraphService.Model
{
    public class SendMessageModel
    {
        public string Subject { get; set; }
        public string Body { get; set; }
        public string[] ToRecipients { get; set; }
        public string[] CcRecipients { get; set; }
        public string[] BccRecipients { get; set; }
    }
}

