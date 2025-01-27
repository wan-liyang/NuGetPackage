using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using TryIT.MicrosoftGraphApi.Helper;
using TryIT.MicrosoftGraphApi.Model.Outlook;
using TryIT.MicrosoftGraphApi.Request.Outlook;
using TryIT.MicrosoftGraphApi.Response.Outlook;

namespace TryIT.MicrosoftGraphApi.HttpClientHelper
{
    internal class OutlookHelper : BaseHelper
    {
        private TryIT.RestApi.Api api;
        private HttpClient _httpClient;

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
        public List<GetMessageResponse.Message> GetMessages()
        {
            string url = $"{GraphApiRootUrl}/me/mailFolders/Inbox/messages";            

            try
            {
                var response = _httpClient.GetAsync(url).GetAwaiter().GetResult();
                CheckStatusCode(response);

                string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var result = content.JsonToObject<GetMessageResponse.Response>();
                return result.value;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// get message from inbox folder
        /// <para>https://graph.microsoft.com/v1.0/users/{EmailAddress}/mailFolders/{FolderId}/messages?$top={MaxItems}&$filter={Filter}&$select={Select}&$search={Search}&$count={Count}</para>
        /// </summary>
        /// <returns></returns>
        public List<GetMessageResponse.Message> GetMessages(GetMessageModel getMessage)
        {
            string mailbox = !string.IsNullOrEmpty(getMessage?.mailbox) ? getMessage?.mailbox : "me";
            string folder = !string.IsNullOrEmpty(getMessage?.folder) ? getMessage.folder : "inbox";

            string url = $"{GraphApiRootUrl}/{mailbox}/mailFolders/{folder}/messages";

            if (getMessage?.top > 0)
            {
                url = $"{url}?$top={getMessage.top}";
            }

            try
            {
                List<GetMessageResponse.Message> messages = new List<GetMessageResponse.Message>();

                var response = _httpClient.GetAsync(url).GetAwaiter().GetResult();
                CheckStatusCode(response, api.RetryResults);

                string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var responseObj = content.JsonToObject<GetMessageResponse.Response>();

                messages.AddRange(responseObj.value);
                if (!string.IsNullOrEmpty(responseObj.odatanextLink))
                {
                    _getnextlink(responseObj.odatanextLink, messages);
                }

                return responseObj.value;
            }
            catch
            {
                throw;
            }
        }

        private void _getnextlink(string nextLink, List<GetMessageResponse.Message> list)
        {
            try
            {
                var response = api.GetAsync(nextLink).GetAwaiter().GetResult();
                CheckStatusCode(response, api.RetryResults);

                string content = response.Content.ReadAsStringAsync().Result;
                var responseObj = content.JsonToObject<GetMessageResponse.Response>();

                list.AddRange(responseObj.value);

                if (!string.IsNullOrEmpty(responseObj.odatanextLink))
                {
                    _getnextlink(responseObj.odatanextLink, list);
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// send a message
        /// </summary>
        /// <param name="message"></param>
        public void SendMessage(SendMessageModel message)
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

            try
            {
                string url = $"{GraphApiRootUrl}/me/sendMail";
                if (!string.IsNullOrEmpty(message.From))
                {
                    url = $"{GraphApiRootUrl}/users/{message.From}/sendMail";
                }

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
    
        public void Move(MoveMessageModel moveMessage)
        {
            // /users/{id | userPrincipalName}/mailFolders/{id}/messages/{id}/move
            // /users/{id | userPrincipalName}/messages/{id}/move

            string mailbox = !string.IsNullOrEmpty(moveMessage?.mailbox) ? moveMessage?.mailbox : "me";

            string url = $"{GraphApiRootUrl}/{mailbox}/messages/{moveMessage.messageId}/move";

            try
            {
                MoveMessageRequest request = new MoveMessageRequest
                {
                    destinationId = moveMessage.destinationFolder,
                };

                HttpContent content = GetJsonHttpContent(request);
                var response = api.PostAsync(url, content).GetAwaiter().GetResult();

                CheckStatusCode(response, api.RetryResults);
            }
            catch
            {
                throw;
            }
        }
    }
}
