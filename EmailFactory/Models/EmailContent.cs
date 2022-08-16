using System.Collections.Generic;

namespace EmailFactory.Models
{
    public class EmailContent
    {
        /// <summary>
        /// mandatory, the target api to send email, should pass different value for different environment
        /// </summary>
        public string ApiUrl { get; set; }
        /// <summary>
        /// the From name & address shows in outlook, default is "Liyang-noreply.ncs.com.sg"
        /// </summary>
        public string From { get; set; }
        public string To { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public List<EmailAttachment> Attachment { get; set; }

        /// <summary>
        /// specific message to be display in email content for Non-Production environment
        /// </summary>
        public string State { get; set; }
    }
}
