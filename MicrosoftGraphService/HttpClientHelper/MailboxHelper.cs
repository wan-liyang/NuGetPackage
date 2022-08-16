using MicrosoftGraphService.ExtensionHelper;
using MicrosoftGraphService.Model;
using System;
using System.Net.Http;

namespace MicrosoftGraphService.HttpClientHelper
{
    public class MailboxHelper
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
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
