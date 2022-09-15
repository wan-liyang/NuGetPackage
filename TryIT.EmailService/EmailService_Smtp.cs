using EmailService.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;

namespace TryIT.EmailService
{
    /// <summary>
    /// send email via SMTP
    /// </summary>
    public class EmailService_Smtp
    {
        /// <summary>
        /// send email
        /// </summary>
        /// <param name="emailContent"></param>
        public static void SendEmail(EmailContent emailContent)
        {
            _SendEmail(emailContent);
        }

        private static void _SendEmail(EmailContent emailContent)
        {
            string from = emailContent.From;
            string to = emailContent.To;
            string cc = emailContent.Cc;
            string bcc = emailContent.Bcc;
            string subject = emailContent.Subject;
            string body = emailContent.Body;
            List<EmailAttachment> attachments = emailContent.Attachment;

            if (string.IsNullOrEmpty(from))
            {
                from = Config.FromAddress;
            }

            body = body.Replace("\n", "");

            List<MemoryStream> listStream = new List<MemoryStream>();
            using (MailMessage message = new MailMessage())
            {
                message.From = new MailAddress(from);

                AddEmailToMailAddressCollection(to, message.To);
                AddEmailToMailAddressCollection(cc, message.CC);
                AddEmailToMailAddressCollection(bcc, message.Bcc);

                if (attachments != null && attachments.Count > 0)
                {
                    foreach (var item in attachments)
                    {
                        string fileName = string.IsNullOrEmpty(item.FileName) ? System.IO.Path.GetFileName(item.FileNameAndPath) : item.FileName;
                        if (!string.IsNullOrEmpty(fileName))
                        {
                            try
                            {
                                MemoryStream stream = new MemoryStream(File.ReadAllBytes(item.FileNameAndPath));
                                message.Attachments.Add(new System.Net.Mail.Attachment(stream, fileName));
                                listStream.Add(stream);
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                        }
                    }
                }

                message.Subject = subject;
                message.SubjectEncoding = System.Text.Encoding.UTF8;

                message.Body = "<p style='font-family:Calibri; font-size:11px;'>" + body + "</p>";
                message.IsBodyHtml = true;
                message.BodyEncoding = System.Text.Encoding.UTF8;

                using (SmtpClient client = new SmtpClient(Config.SmtpServer, Config.SmtpPort))
                {
                    try
                    {
                        client.Send(message);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        ReleaseStream(listStream);
                    }
                }
            }
        }

        private static void ReleaseStream(List<MemoryStream> listStream)
        {
            if (listStream != null && listStream.Count > 0)
            {
                foreach (var item in listStream)
                {
                    if (item != null)
                    {
                        item.Dispose();
                    }
                }
            }
        }

        /// <summary>
        /// add email address into MailMessage receiver (To / Cc / Bcc)
        /// </summary>
        /// <param name="strEmails"></param>
        /// <param name="mailAddressColl"></param>
        private static void AddEmailToMailAddressCollection(string strEmails, MailAddressCollection mailAddressColl)
        {
            if (!string.IsNullOrEmpty(strEmails))
            {
                string[] arrEmail = strEmails.Split(',', ';');
                foreach (var item in arrEmail)
                {
                    /*
                        only add for non-empty item,
                        if strEmails = 'abc@abc.com;' => this will get two items 'abc@abc.com' & empty item,
                        and empty item will cause exception
                     */
                    if (!string.IsNullOrEmpty(item))
                    {
                        mailAddressColl.Add(item);
                    }
                }
            }
        }
    }
}
