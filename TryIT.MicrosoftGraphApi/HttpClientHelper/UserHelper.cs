using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TryIT.MicrosoftGraphApi.Helper;
using TryIT.MicrosoftGraphApi.Model.User;
using TryIT.MicrosoftGraphApi.Request.User;
using TryIT.MicrosoftGraphApi.Response.User;

namespace TryIT.MicrosoftGraphApi.HttpClientHelper
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
        /// get my info
        /// </summary>
        /// <returns></returns>
        public GetUserResponse.User GetMe()
        {
            return GetUser("me");
        }

        /// <summary>
        /// get user info
        /// </summary>
        /// <param name="userPrincipalName"></param>
        /// <returns></returns>
        public GetUserResponse.User GetUser(string userPrincipalName)
        {
            string url = $"{GraphApiRootUrl}/users/{userPrincipalName}";
            if (userPrincipalName.IsEquals("me"))
            {
                url = $"{GraphApiRootUrl}/me";
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
        /// get user info by mail, because userPrincipalName may different with mail
        /// </summary>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        public GetUserResponse.User GetUserByMail(string userEmail)
        {
            if (string.IsNullOrEmpty(userEmail))
            {
                throw new ArgumentNullException(nameof(userEmail));
            }
         
            string url = $"{GraphApiRootUrl}/users?$filter={EscapeExpression($"mail eq '{userEmail}'")}";
            try
            {
                var response = _httpClient.GetAsync(url).GetAwaiter().GetResult();
                CheckStatusCode(response);

                string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                return content.JsonToObject<GetUserResponse.Response>().value.FirstOrDefault();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// get user info
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="additionalAttribute"></param>
        /// <returns></returns>
        public GetUserResponse.User GetUserByMail(string userEmail, params string[] additionalAttribute)
        {
            if (string.IsNullOrEmpty(userEmail))
            {
                throw new ArgumentNullException(nameof(userEmail));
            }

            string url = $"{GraphApiRootUrl}/users?$filter={EscapeExpression($"mail eq '{userEmail}'")}";

            var props = typeof(GetUserResponse.User).GetProperties().Where(p => !p.Name.IsEquals("AdditionalAttributes"));
            if (additionalAttribute != null && additionalAttribute.Length > 0)
            {
                string select = $"&$select={string.Join(",", additionalAttribute)}";
                foreach (var item in props)
                {
                    select += $",{item.Name}";
                }

                url += select;
            }

            try
            {
                var response = _httpClient.GetAsync(url).GetAwaiter().GetResult();
                CheckStatusCode(response);

                string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var result = content.JsonToObject<GetUserResponse.Response>().value.FirstOrDefault();

                if (result != null)
                {
                    result.AdditionalAttributes = new Dictionary<string, object>();

                    var values = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
                    JArray jArray = (JArray)values["value"];
                    JObject jObj = (JObject)jArray.First();

                    foreach (var item in jObj)
                    {
                        if (!props.Any(p => p.Name.IsEquals(item.Key)))
                        {
                            result.AdditionalAttributes[item.Key] = item.Value;
                        }
                    }
                }

                return result;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// https://learn.microsoft.com/en-us/graph/aad-advanced-queries?tabs=http
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">employeeId eq 'xxx'</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public List<T> FilterUser<T>(string expression) where T : class
        {
            List<T> list = new List<T>();

            if (string.IsNullOrEmpty(expression))
            {
                throw new ArgumentNullException(nameof(expression));
            }

            string url = $"{GraphApiRootUrl}/users?$filter={EscapeExpression(expression)}";
            try
            {
                var props = typeof(T).GetProperties();
                string select = $"&$count=true&$select=";
                foreach (var item in props)
                {
                    select += $"{item.Name},";
                }
                select = select.TrimEnd(',');

                url += select;

                AddDefaultRequestHeaders(_httpClient, "ConsistencyLevel", "eventual");

                var response = _httpClient.GetAsync(url).GetAwaiter().GetResult();
                CheckStatusCode(response);

                string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                var data = content.GetJsonValue<List<T>>("value");
                list.AddRange(data);

                string nextLink = content.GetJsonValue<string>("@odata.nextLink");

                if (!string.IsNullOrEmpty(nextLink))
                {
                    FilterUserNextLink<T>(nextLink, list);
                }

                return list;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// get next page data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nextlink"></param>
        /// <param name="list"></param>
        private void FilterUserNextLink<T>(string nextlink, List<T> list)
        {
            var response = _httpClient.GetAsync(nextlink).GetAwaiter().GetResult();
            CheckStatusCode(response);

            string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            var data = content.GetJsonValue<List<T>>("value");
            list.AddRange(data);

            string next = content.GetJsonValue<string>("@odata.nextLink");

            if (!string.IsNullOrEmpty(next))
            {
                FilterUserNextLink<T>(next, list);
            }
        }

        /// <summary>
        /// get user by following url
        /// <para>https://graph.microsoft.com/v1.0/users?$filter={attrKey} eq '{attrValue}'</para>
        /// </summary>
        /// <param name="attrKey"></param>
        /// <param name="attrValue"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public GetUserResponse.User GetUserByAttribute(string attrKey, string attrValue)
        {
            if (string.IsNullOrEmpty(attrKey))
            {
                throw new ArgumentNullException(nameof(attrKey));
            }

            string url = $"{GraphApiRootUrl}/users?$filter={EscapeExpression($"{attrKey} eq '{attrValue}'")}";
            try
            {
                var response = _httpClient.GetAsync(url).GetAwaiter().GetResult();
                CheckStatusCode(response);

                string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                return content.JsonToObject<GetUserResponse.Response>().value.FirstOrDefault();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// post invitation
        /// <para>https://graph.microsoft.com/v1.0/invitations</para>
        /// </summary>
        /// <param name="invitationModel"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public CreateInvitationResponse.Response CreateInvitation(CreateInvitationModel invitationModel)
        {
            string url = $"{GraphApiRootUrl}/invitations";
            try
            {
                CreateInvitationRequest.Body request = new CreateInvitationRequest.Body
                {
                    invitedUserDisplayName = invitationModel.UserDisplayName,
                    invitedUserEmailAddress= invitationModel.UserEmailAddress,
                    inviteRedirectUrl = invitationModel.RedirectUrl,
                    sendInvitationMessage = invitationModel.SendInvitationMessage
                };

                HttpContent httpContent = new StringContent(request.ObjectToJson());
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = _httpClient.PostAsync(url, httpContent).GetAwaiter().GetResult();
                CheckStatusCode(response);

                string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                return content.JsonToObject<CreateInvitationResponse.Response>();
            }
            catch
            {
                throw;
            }
        }



        /// <summary>
        /// Delete user.
        /// <para>When deleted, user resources are moved to a temporary container and can be restored within 30 days. After that time, they are permanently deleted. To learn more, see deletedItems.</para>
        /// <para>https://learn.microsoft.com/en-us/graph/api/user-delete</para>
        /// </summary>
        /// <param name="userEmail"></param>
        /// <returns>true: delete success, false: user not exists</returns>
        public bool DeleteUserByEmail(string userEmail)
        {
            var user = GetUserByMail(userEmail);

            if (user != null && !string.IsNullOrEmpty(user.id))
            {
                string url = $"{GraphApiRootUrl}/users/{user.id}";
                try
                {
                    var response = _httpClient.DeleteAsync(url).GetAwaiter().GetResult();
                    CheckStatusCode(response);

                    return true;
                }
                catch
                {
                    throw;
                }
            }

            return false;            
        }

        //public byte[] GetPhoto(string email)
        //{
        //    // /users/{id | userPrincipalName}/photo
        //    var user = GetUserByMail(email);

        //    if (user != null && !string.IsNullOrEmpty(user.id))
        //    {
        //        string url = $"{GraphApiRootUrl}/users/{user.id}/photo/$value";
        //        try
        //        {
        //            var response = _httpClient.GetAsync(url).GetAwaiter().GetResult();
        //            //CheckStatusCode(response);

        //            return response.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
        //        }
        //        catch
        //        {
        //            throw;
        //        }
        //    }
        //    else
        //    {
        //        throw new Exception($"user {email} not found");
        //    }
        //}
    }
}
