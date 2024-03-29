﻿using System.Collections.Generic;

namespace EmailService.Models
{
    public class EmailContent
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public List<EmailAttachment> Attachment { get; set; }
    }
}
