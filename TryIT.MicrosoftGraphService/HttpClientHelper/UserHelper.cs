using System;
using System.Net.Http;
using System.Xml.Linq;
using TryIT.MicrosoftGraphService.ApiModel.User;
using TryIT.MicrosoftGraphService.ExtensionHelper;

namespace TryIT.MicrosoftGraphService.HttpClientHelper
{
    internal class UserHelper : BaseHelper
    {
        private HttpClient _httpClient;

        public UserHelper(HttpClient httpClient)
        {
            if (null == httpClient)
                throw new ArgumentNullException(nameof(httpClient));

            _httpClient = httpClient;
        }

        /// <summary>
        /// get user info, if <paramref name="userPrincipalName"/> is empty, get me
        /// </summary>
        /// <param name="userPrincipalName"></param>
        /// <returns></returns>
        public GetUserResponse.User GetUserInfo(string userPrincipalName)
        {
            string url = $"https://graph.microsoft.com/v1.0/users/{userPrincipalName}";

            if (string.IsNullOrEmpty(userPrincipalName))
            {
                url = $"https://graph.microsoft.com/v1.0/me";
            }

            try
            {
                var response = _httpClient.GetAsync(url).GetAwaiter().GetResult();
                CheckStatusCode(response);
                string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                return content.JsonToObject<GetUserResponse.User>();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// get user info, if <paramref name="userEmail"/> is empty, get me
        /// </summary>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        public GetUserResponse.User GetUserByMail(string userEmail)
        {
            string url = $"https://graph.microsoft.com/v1.0/users?$filter=mail eq '{userEmail}'";

            if (string.IsNullOrEmpty(userEmail))
            {
                throw new ArgumentNullException(nameof(userEmail));
            }

            try
            {
                var response = _httpClient.GetAsync(url).GetAwaiter().GetResult();
                CheckStatusCode(response);
                string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                return content.JsonToObject<GetUserResponse.Response>()?.value[0];
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
