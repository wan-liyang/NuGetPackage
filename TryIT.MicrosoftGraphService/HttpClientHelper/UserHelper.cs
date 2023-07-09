using TryIT.MicrosoftGraphService.ExtensionHelper;
using TryIT.MicrosoftGraphService.ApiModel;
using System;
using System.Net.Http;
using TryIT.MicrosoftGraphService.ApiModel.User;

namespace TryIT.MicrosoftGraphService.HttpClientHelper
{
    internal class UserHelper
    {
        private HttpClient _httpClient;

        public UserHelper(HttpClient httpClient)
        {
            if (null == httpClient)
                throw new ArgumentNullException(nameof(httpClient));

            _httpClient = httpClient;
        }

        /// <summary>
        /// get user info, if <paramref name="userEmail"/> is empty, get me
        /// </summary>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        public GetUserResponse.Response GetUserInfo(string userEmail)
        {
            string url = $"https://graph.microsoft.com/v1.0/users/{userEmail}";

            if (string.IsNullOrEmpty(userEmail))
            {
                url = $"https://graph.microsoft.com/v1.0/me";
            }

            try
            {
                var response = _httpClient.GetAsync(url).GetAwaiter().GetResult();

                string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                return content.JsonToObject<GetUserResponse.Response>();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// get user by employeeid
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        public GetUserByEmployeeIdResponse.Response GetUserByEmployeeId(string employeeId)
        {
            string url = $"https://graph.microsoft.com/v1.0/users?$select=userPrincipalName,displayName,mail,employeeID&$filter=employeeID eq '{employeeId}'";

            try
            {
                var response = _httpClient.GetAsync(url).GetAwaiter().GetResult();

                string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                return content.JsonToObject<GetUserByEmployeeIdResponse.Response>();
            }
            catch
            {
                throw;
            }
        }
    }
}
