using System.Collections.Generic;

namespace TryIT.MicrosoftGraphService.Model
{
    public class MailboxSearchModel
    {
        public string EmailAddress { get; set; }
        public string FolderName { get; set; }
        public int TopItems { get; set; }
        public List<MessageAttribute> Select { get; set; }
    }

    public enum MessageAttribute
    {
        subject
    }
}
