namespace TryIT.MicrosoftGraphService.Model
{
    public class MailboxModel
    {
        public class Message
        {
            public string FromAddress { get; set; }
            public string Subject { get; set; }
            public string Body { get; set; }
        }
    }
}
