using TryIT.MicrosoftGraphService.Model;

namespace TryIT.MicrosoftGraphService.ApiModel
{
    internal static class MailboxResponseExt
    {
        public static MailboxModel.Message ToMessageModule(this MailboxResponse message)
        {
            MailboxModel.Message module = new MailboxModel.Message();

            if (message == null)
            {
                return module;
            }

            if (message.sender != null && message.sender.emailAddress != null)
            {
                module.FromAddress = message.sender.emailAddress.address;
            }            
            module.Subject = message.subject;

            return module;
        }
    }
}
