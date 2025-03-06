using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TryIT.MicrosoftGraphApi.Helper;
using TryIT.MicrosoftGraphApi.Model.Outlook;
using TryIT.MicrosoftGraphApi.Request.Outlook;
using TryIT.MicrosoftGraphApi.Response.Outlook;

namespace TryIT.MicrosoftGraphApi.HttpClientHelper
{
    internal class OutlookHelper : BaseHelper
    {
        private readonly TryIT.RestApi.Api api;
        private readonly HttpClient _httpClient;

        public OutlookHelper(HttpClient httpClient)
        {
            if (null == httpClient) 
                throw new ArgumentNullException(nameof(httpClient));

            // use RestApi library and enable retry
            api = new RestApi.Api(new RestApi.Models.ApiConfig
            {
                HttpClient = httpClient,
                EnableRetry = true,
            });

            _httpClient = httpClient;
        }

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

            var response = await _httpClient.GetAsync(url);
            CheckStatusCode(response, api.RetryResults);

            string content = await response.Content.ReadAsStringAsync();
            var responseObj = content.JsonToObject<GetMessageResponse.Response>();

            messages.AddRange(responseObj.value);
            if (!string.IsNullOrEmpty(responseObj.odatanextLink) 
                && getMessage?.top <= 0 
                && (getMessage?.top <= 0 || messages.Count < getMessage.top))
            {
                await _getnextlink(responseObj.odatanextLink, messages, getMessage?.top);
            }

            return responseObj.value;
        }

        private async Task _getnextlink(string nextLink, List<GetMessageResponse.Message> list, int? top)
        {
            var response = await api.GetAsync(nextLink);
            CheckStatusCode(response, api.RetryResults);

            string content = await response.Content.ReadAsStringAsync();
            var responseObj = content.JsonToObject<GetMessageResponse.Response>();

            list.AddRange(responseObj.value);

            if (!string.IsNullOrEmpty(responseObj.odatanextLink) && (top <= 0 || list.Count < top))
            {
                await _getnextlink(responseObj.odatanextLink, list, top);
            }
        }

        public async Task<string> GetMIMEContentAsync(GetMIMEContentModel model)
        {
            string url = $"{GraphApiRootUrl}/me/messages/{model.messageId}/$value";
            if (!string.IsNullOrEmpty(model.mailbox))
            {
                url = $"{GraphApiRootUrl}/users/{model.mailbox}/messages/{model.messageId}/$value";
            }

            HttpResponseMessage response = await _httpClient.GetAsync(url);
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

            HttpResponseMessage response = await _httpClient.DeleteAsync(url);
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

            HttpResponseMessage response = await _httpClient.PostAsync(url, httpContent);
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

            HttpResponseMessage response = await _httpClient.PostAsync(url, httpContent);
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
            var response = await api.PostAsync(url, content);

            CheckStatusCode(response, api.RetryResults);
        }
    }
}
