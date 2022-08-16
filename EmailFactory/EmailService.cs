using EmailFactory.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace EmailFactory
{
    public class EmailService
    {
        /// <summary>
        /// send email, the email will be recored into Liyang Email Service, the service will perform to send email to recipients
        /// </summary>
        /// <param name="emailContent"></param>
        public static async Task<string> SendEmailAsync(EmailContent emailContent)
        {
            return await Send(emailContent);
        }

        private static async Task<string> Send(EmailContent model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            
            if (string.IsNullOrEmpty(model.ApiUrl))
            {
                throw new ArgumentNullException(nameof(model.ApiUrl));
            }

            if (string.IsNullOrEmpty(model.From))
            {
                throw new ArgumentNullException(nameof(model.From));
            }

            if (string.IsNullOrEmpty(model.Subject))
            {
                throw new ArgumentNullException(nameof(model.Subject));
            }

            if (model.Attachment != null && model.Attachment.Count > 0)
            {
                foreach (var item in model.Attachment)
                {
                    if (!string.IsNullOrEmpty(item.FileName) && !string.IsNullOrEmpty(item.FileNameAndPath))
                    {
                        item.FileName = System.IO.Path.GetFileName(item.FileNameAndPath);
                    }
                }
            }

            return await _SendEmailAsync(model);
        }

        private class EmailModel
        {
            public class Attachment
            {
                public string Name { get; set; }
                /// <summary>
                /// content with base64string format
                /// </summary>
                public string Content { get; set; }
            }

            public class Content
            {
                public string From { get; set; }
                public string To { get; set; }
                public string Cc { get; set; }
                public string Bcc { get; set; }
                public string Subject { get; set; }
                public string Body { get; set; }
                public List<Attachment> Attachment { get; set; }
            }
        }

        private static async Task<string> _SendEmailAsync(EmailContent email)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

                EmailModel.Content emailModel = new EmailModel.Content
                {
                    From = email.From,
                    To = email.To,
                    Cc = email.Cc,
                    Bcc = email.Bcc,
                    Subject = email.Subject,
                    Body = email.Body,
                    Attachment = new List<EmailModel.Attachment>()
                };

                if (email.Attachment != null && email.Attachment.Count > 0)
                {
                    foreach (var item in email.Attachment)
                    {
                        byte[] bytes = System.IO.File.ReadAllBytes(item.FileNameAndPath);
                        string base64 = Convert.ToBase64String(bytes);

                        emailModel.Attachment.Add(new EmailModel.Attachment
                        {
                            Name = item.FileName,
                            Content = base64
                        });
                    }
                }

                string json = JsonConvert.SerializeObject(emailModel);

                // new MultipartFormDataContent() not work for this case
                using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                {
                    // for resolve "The underlying connection was closed: An unexpected error occurred on a send" error
                    ServicePointManager.SecurityProtocol = GetSecurityProtocol();
                    using (var message = client.PostAsync(email.ApiUrl, content))
                    {
                        //var clientResult = message.GetAwaiter().GetResult();
                        //string result = clientResult.Content.ReadAsStringAsync().Result;

                        string result = await message.Result.Content.ReadAsStringAsync();
                        return result;
                    }
                }
            }
        }
        private static SecurityProtocolType GetSecurityProtocol()
        {
            /*
             * force to use Tls12 protocol

                Insecure Transport: Weak SSL Protocol (4 issues)

                Abstract
                The SSLv2, SSLv23, SSLv3, TLSv1.0 and TLSv1.1 protocols contain flaws that make them insecure and
                should not be used to transmit sensitive data.

                Explanation
                The Transport Layer Security (TLS) and Secure Sockets Layer (SSL) protocols provide a protection
                mechanism to ensure the authenticity, confidentiality, and integrity of data transmitted between a client and
                web server. Both TLS and SSL have undergone revisions resulting in periodic version updates. Each new
                revision is designed to address the security weaknesses discovered in previous versions. Use of an
                insecure version of TLS/SSL weakens the data protection strength and might allow an attacker to
                compromise, steal, or modify sensitive information.
                Weak versions of TLS/SSL might exhibit one or more of the following properties:
                - No protection against man-in-the-middle attacks - Same key used for authentication and encryption -
                Weak message authentication control - No protection against TCP connection closing - Use of weak cipher
                suites
                The presence of these properties might allow an attacker to intercept, modify, or tamper with sensitive data.

                Recommendation
                Fortify highly recommends forcing the client to use only the most secure protocols.                 
            */

            return SecurityProtocolType.Tls12;
        }
    }
}
