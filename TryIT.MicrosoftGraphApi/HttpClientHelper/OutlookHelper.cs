using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TryIT.MicrosoftGraphApi.Helper;
using TryIT.MicrosoftGraphApi.Model;
using TryIT.MicrosoftGraphApi.Model.Outlook;
using TryIT.MicrosoftGraphApi.Request.Outlook;
using TryIT.MicrosoftGraphApi.Response.Outlook;

namespace TryIT.MicrosoftGraphApi.HttpClientHelper
{
    internal class OutlookHelper : BaseHelper
    {
        public OutlookHelper(MsGraphApiConfig config) : base(config) { }

        /// <summary>
        /// get message from inbox folder
        /// <para>https://graph.microsoft.com/v1.0/users/{EmailAddress}/mailFolders/{FolderId}/messages?$top={MaxItems}&$filter={Filter}&$select={Select}&$search={Search}&$count={Count}</para>
        /// </summary>
        /// <returns></returns>
        public async Task<List<GetMessageResponse.Message>> GetMessagesAsync(GetMessageModel getMessage)
        {
            string folder = !string.IsNullOrEmpty(getMessage?.folder) ? getMessage.folder : "inbox";

            string url = $"{GraphApiRootUrl}/me/mailFolders/{folder}/messages";

            if (!string.IsNullOrEmpty(getMessage.mailbox))
            {
                url = $"{GraphApiRootUrl}/users/{getMessage.mailbox}/mailFolders/{folder}/messages";
            }

            if (getMessage?.top > 0)
            {
                url = url.AppendQueryToUrl($"$top={getMessage.top}");
            }

            if (!string.IsNullOrEmpty(getMessage.filterExpression))
            {
                url = url.AppendQueryToUrl($"$filter={EscapeExpression(getMessage.filterExpression)}");
            }

            List<GetMessageResponse.Message> messages = new List<GetMessageResponse.Message>();

            var response = await RestApi.GetAsync(url);
            CheckStatusCode(response, RestApi.RetryResults);

            string content = await response.Content.ReadAsStringAsync();
            var responseObj = content.JsonToObject<GetMessageResponse.Response>();

            messages.AddRange(responseObj.value);

            if (!string.IsNullOrEmpty(responseObj.odatanextLink) 
                && getMessage?.top <= 0 
                && (getMessage?.top <= 0 || messages.Count < getMessage.top))
            {
                await _getnextlink_messages(responseObj.odatanextLink, messages, getMessage?.top);
            }

            return messages;
        }

        private async Task _getnextlink_messages(string nextLink, List<GetMessageResponse.Message> list, int? top)
        {
            var response = await RestApi.GetAsync(nextLink);
            CheckStatusCode(response, RestApi.RetryResults);

            string content = await response.Content.ReadAsStringAsync();
            var responseObj = content.JsonToObject<GetMessageResponse.Response>();

            list.AddRange(responseObj.value);

            if (!string.IsNullOrEmpty(responseObj.odatanextLink) && (top <= 0 || list.Count < top))
            {
                await _getnextlink_messages(responseObj.odatanextLink, list, top);
            }
        }

        public async Task<string> GetMIMEContentAsync(GetMIMEContentModel model)
        {
            string url = $"{GraphApiRootUrl}/me/messages/{model.messageId}/$value";
            if (!string.IsNullOrEmpty(model.mailbox))
            {
                url = $"{GraphApiRootUrl}/users/{model.mailbox}/messages/{model.messageId}/$value";
            }

            HttpResponseMessage response = await RestApi.GetAsync(url);
            CheckStatusCode(response);

            return await response.Content.ReadAsStringAsync();
        }

        public async Task DeleteMessageAsync(DeleteMessageModel model)
        {
            string url = $"{GraphApiRootUrl}/me/messages/{model.messageId}";
            if (!string.IsNullOrEmpty(model.mailbox))
            {
                url = $"{GraphApiRootUrl}/users/{model.mailbox}/messages/{model.messageId}";
            }

            HttpResponseMessage response = await RestApi.DeleteAsync(url);
            CheckStatusCode(response);
        }

        public async Task<GetMessageResponse.Message> MoveMessageAsync(MoveMessageModel model)
        {
            string url = $"{GraphApiRootUrl}/me/messages/{model.messageId}/move";
            if (!string.IsNullOrEmpty(model.mailbox))
            {
                url = $"{GraphApiRootUrl}/users/{model.mailbox}/messages/{model.messageId}/move";
            }

            MoveMessageRequest request = new MoveMessageRequest
            {
                destinationId = model.destinationFolder
            };

            HttpContent httpContent = new StringContent(request.ObjectToJson());
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            HttpResponseMessage response = await RestApi.PostAsync(url, httpContent);
            CheckStatusCode(response);

            var content = await response.Content.ReadAsStringAsync();

            return content.JsonToObject<GetMessageResponse.Message>();
        }

        /// <summary>
        /// send a message
        /// </summary>
        /// <param name="message"></param>
        public async Task SendMessageAsync(SendMessageModel message)
        {
            var model = new SendMessageRequest.Request
            {
                message = new SendMessageRequest.Message
                {
                    subject = message.Subject,
                    body = new SendMessageRequest.Body
                    {
                        contentType = message.BodyContentType.ToString(),
                        content = message.Body
                    }
                },
                saveToSentItems = message.SaveToSentItems
            };

            model.message.toRecipients = new List<SendMessageRequest.Recipient>();
            model.message.ccRecipients = new List<SendMessageRequest.Recipient>();
            model.message.bccRecipients = new List<SendMessageRequest.Recipient>();
            model.message.attachments = new List<SendMessageRequest.Attachment>();

            if (message.ToRecipients != null && message.ToRecipients.Length > 0)
            {
                foreach (var item in message.ToRecipients)
                {
                    model.message.toRecipients.Add(new SendMessageRequest.Recipient { emailAddress = new SendMessageRequest.EmailAddress { address = item } });
                }
            }

            if (message.CcRecipients != null && message.CcRecipients.Length > 0)
            {
                foreach (var item in message.CcRecipients)
                {
                    model.message.ccRecipients.Add(new SendMessageRequest.Recipient { emailAddress = new SendMessageRequest.EmailAddress { address = item } });
                }
            }

            if (message.BccRecipients != null && message.BccRecipients.Length > 0)
            {
                foreach (var item in message.BccRecipients)
                {
                    model.message.bccRecipients.Add(new SendMessageRequest.Recipient { emailAddress = new SendMessageRequest.EmailAddress { address = item } });
                }
            }

            if (message.Attachments != null && message.Attachments.Count > 0)
            {
                var validAttachments = message.Attachments.Where(p => !string.IsNullOrEmpty(p.FileName) && p.FileContent != null).ToList();

                foreach (var item in validAttachments)
                {
                    model.message.attachments.Add(new SendMessageRequest.Attachment
                    {
                        odatatype = "#microsoft.graph.fileAttachment",
                        name = item.FileName,
                        contentType = MIMEType.GetContentType(item.FileName),
                        contentBytes = Convert.ToBase64String(item.FileContent)
                    });
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
              "saveToSentItems": "false",
              "attachments": [
                  {
                    "@odata.type": "#microsoft.graph.fileAttachment",
                    "name": "attachment.txt",
                    "contentType": "text/plain",
                    "contentBytes": "SGVsbG8gV29ybGQh"
                  }
                ]
            }
             */

            string url = $"{GraphApiRootUrl}/me/sendMail";
            if (!string.IsNullOrEmpty(message.From))
            {
                url = $"{GraphApiRootUrl}/users/{message.From}/sendMail";
            }

            string jsonContent = model.ObjectToJson();

            HttpContent httpContent = new StringContent(jsonContent);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            HttpResponseMessage response = await RestApi.PostAsync(url, httpContent);
            CheckStatusCode(response);
        }
    
        public async Task MoveAsync(MoveMessageModel moveMessage)
        {
            // /users/{id | userPrincipalName}/mailFolders/{id}/messages/{id}/move
            // /users/{id | userPrincipalName}/messages/{id}/move

            string mailbox = !string.IsNullOrEmpty(moveMessage?.mailbox) ? moveMessage?.mailbox : "me";

            string url = $"{GraphApiRootUrl}/{mailbox}/messages/{moveMessage.messageId}/move";

            MoveMessageRequest request = new MoveMessageRequest
            {
                destinationId = moveMessage.destinationFolder,
            };

            HttpContent content = GetJsonHttpContent(request);
            var response = await RestApi.PostAsync(url, content);

            CheckStatusCode(response, RestApi.RetryResults);
        }

        public async Task<List<GetMailboxFolderResponse.Folder>> GetMailboxFoldersAsync(GetMailboxFolderModel model)
        {
            List<GetMailboxFolderResponse.Folder> folders = new List<GetMailboxFolderResponse.Folder>();

            string url = $"{GraphApiRootUrl}/me/mailFolders";

            if (!string.IsNullOrEmpty(model.mailbox))
            {
                url = $"{GraphApiRootUrl}/users/{model.mailbox}/mailFolders";
            }

            if (!string.IsNullOrEmpty(model.filterExpression))
            {
                url = url.AppendQueryToUrl($"$filter={EscapeExpression(model.filterExpression)}");
            }

            var response = await RestApi.GetAsync(url);
            CheckStatusCode(response);

            string content = await response.Content.ReadAsStringAsync();
            var responseObj = content.JsonToObject<GetMailboxFolderResponse.Response>();

            folders.AddRange(responseObj.value);

            if (!string.IsNullOrEmpty(responseObj.odatanextLink))
            {
                await _getnextlink_folders(responseObj.odatanextLink, folders);
            }

            return folders;
        }

        private async Task _getnextlink_folders(string nextLink, List<GetMailboxFolderResponse.Folder> list)
        {
            var response = await RestApi.GetAsync(nextLink);
            CheckStatusCode(response, RestApi.RetryResults);

            string content = await response.Content.ReadAsStringAsync();
            var responseObj = content.JsonToObject<GetMailboxFolderResponse.Response>();

            list.AddRange(responseObj.value);

            if (!string.IsNullOrEmpty(responseObj.odatanextLink))
            {
                await _getnextlink_folders(responseObj.odatanextLink, list);
            }
        }
    }
}
