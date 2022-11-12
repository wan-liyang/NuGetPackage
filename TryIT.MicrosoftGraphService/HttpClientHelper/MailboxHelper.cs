using TryIT.MicrosoftGraphService.ExtensionHelper;
using TryIT.MicrosoftGraphService.ApiModel;
using TryIT.MicrosoftGraphService.Model;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace TryIT.MicrosoftGraphService.HttpClientHelper
{
    internal class MailboxHelper : BaseHelper
    {
        private HttpClient _httpClient;

        public MailboxHelper(HttpClient httpClient)
        {
            if (null == httpClient) 
                throw new ArgumentNullException(nameof(httpClient));

            _httpClient = httpClient;
        }

        public MailboxResponseList GetMessages(string userEmail, string folder)
        {
            var search = new MailboxSearchModel();
            search.EmailAddress = string.IsNullOrEmpty(userEmail) ? "Me" : userEmail;
            search.FolderName = string.IsNullOrEmpty(folder) ? "Inbox" : folder;

            return GetMessagesFromFolder(search);
        }

        public MailboxResponseList GetMessagesFromFolder(MailboxSearchModel search)
        {
            string FolderId = string.IsNullOrEmpty(search.FolderName) || search.FolderName.ToUpper().Equals("INBOX") ? "Inbox" : search.FolderName;
            string EmailAddress = string.IsNullOrEmpty(search.EmailAddress) || search.EmailAddress.ToUpper().Equals("ME") ? "Me" : "users/" + search.EmailAddress;

            string url = $"https://graph.microsoft.com/v1.0/{EmailAddress}/mailFolders/{FolderId}/messages";

            string queryParam_top = "";
            string queryParam_select = "";
            if (search.TopItems > 0)
            {
                queryParam_top = "$top=" + search.TopItems;
            }
            if (search.Select != null && search.Select.Count > 0)
            {
                queryParam_select = "$select=" + string.Join(",", search.Select);
            }

            if (!string.IsNullOrEmpty(queryParam_top) 
                || !string.IsNullOrEmpty(queryParam_select))
            {
                url += "?";

                if (!string.IsNullOrEmpty(queryParam_top))
                {
                    url += queryParam_top;
                }
                if (!string.IsNullOrEmpty(queryParam_select))
                {
                    url += queryParam_select;
                }
            }

            //https://graph.microsoft.com/v1.0/users/{EmailAddress}/mailFolders/{FolderId}/messages?$top={MaxItems}&$filter={Filter}&$select={Select}&$search={Search}&$count={Count}

            try
            {
                var response = _httpClient.GetAsync(url).GetAwaiter().GetResult();

                string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                return content.JsonToObject<MailboxResponseList>();
            }
            catch
            {
                throw;
            }
        }

        public void SendMessage(SendMessageModel message)
        {
            string url = $"https://graph.microsoft.com/v1.0/me/sendMail";

            var model = new MessageRequestModel
            {
                message = new MessageRequestModel.Message
                {
                    subject = message.Subject,
                    body = new Body
                    {
                        contentType = "Text",
                        content = message.Body
                    }
                }
            };

            model.message.toRecipients = new System.Collections.Generic.List<Recipient>();
            model.message.ccRecipients = new System.Collections.Generic.List<Recipient>();
            model.message.bccRecipients = new System.Collections.Generic.List<Recipient>();

            if (message.ToRecipients != null && message.ToRecipients.Length > 0)
            {
                foreach (var item in message.ToRecipients)
                {
                    model.message.toRecipients.Add(new Recipient { emailAddress = new EmailAddress { address = item } });
                }
            }

            if (message.CcRecipients != null && message.CcRecipients.Length > 0)
            {
                foreach (var item in message.CcRecipients)
                {
                    model.message.ccRecipients.Add(new Recipient { emailAddress = new EmailAddress { address = item } });
                }
            }

            if (message.BccRecipients != null && message.BccRecipients.Length > 0)
            {
                foreach (var item in message.BccRecipients)
                {
                    model.message.bccRecipients.Add(new Recipient { emailAddress = new EmailAddress { address = item } });
                }
            }

            /*
             {
              "message": {
                "subject": "Meet for lunch?",
                "body": {
                  "contentType": "Text",
                  "content": "The new cafeteria is open."
                },
                "toRecipients": [
                  {
                    "emailAddress": {
                      "address": "fannyd@contoso.onmicrosoft.com"
                    }
                  }
                ],
                "ccRecipients": [
                  {
                    "emailAddress": {
                      "address": "danas@contoso.onmicrosoft.com"
                    }
                  }
                ]
              },
              "saveToSentItems": "false"
            }
             */

            try
            {
                string jsonContent = model.ObjectToJson();

                HttpContent httpContent = new StringContent(jsonContent);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                HttpResponseMessage response = _httpClient.PostAsync(url, httpContent).GetAwaiter().GetResult();
                CheckStatusCode(response);
                string content = response.Content.ReadAsStringAsync().Result;
            }
            catch
            {
                throw;
            }
        }
    }
}
