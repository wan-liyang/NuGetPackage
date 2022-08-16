using TryIT.MicrosoftGraphService.ExtensionHelper;
using TryIT.MicrosoftGraphService.Model;
using System;
using System.Net.Http;

namespace TryIT.MicrosoftGraphService.HttpClientHelper
{
    public class UserHelper
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
        public UserResponse GetUserInfo(string userEmail)
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

                return content.JsonToObject<UserResponse>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
