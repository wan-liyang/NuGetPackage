using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using TryIT.MicrosoftGraphService.ApiModel;
using TryIT.MicrosoftGraphService.ApiModel.Team;
using TryIT.MicrosoftGraphService.ExtensionHelper;
using static TryIT.MicrosoftGraphService.ApiModel.GroupMemberResponse;
using static TryIT.MicrosoftGraphService.ApiModel.GroupResponse;

namespace TryIT.MicrosoftGraphService.HttpClientHelper
{
    internal class GroupHelper : BaseHelper
    {
        private HttpClient _httpClient;

        public GroupHelper(HttpClient httpClient)
        {
            if (null == httpClient)
                throw new ArgumentNullException(nameof(httpClient));

            _httpClient = httpClient;
        }

        public GetGroupResponse GetGroup(string groupDisplayName)
        {
            if (string.IsNullOrEmpty(groupDisplayName))
            {
                throw new ArgumentNullException(nameof(groupDisplayName));
            }

            string url = $"https://graph.microsoft.com/v1.0/groups?$filter=displayName eq '{groupDisplayName}'";

            try
            {
                AddDefaultRequestHeaders(_httpClient, "ConsistencyLevel", "eventual");

                var response = _httpClient.GetAsync(url).GetAwaiter().GetResult();

                string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                CheckStatusCode(response);

                return content.JsonToObject<GetGroupResponse>();
            }
            catch
            {
                throw;
            }
        }

        private List<Member> GroupMembers;
        /// <summary>
        /// get group members
        /// </summary>
        /// <param name="groupDisplayName">group display name</param>
        /// <returns></returns>
        public List<Member> GetMembers(string groupDisplayName)
        {
            var group = GetGroup(groupDisplayName);

            if (group == null || group.value == null || group.value.Count == 0)
            {
                throw new Exception($"group '{groupDisplayName}' not found");
            }

            string url = $"https://graph.microsoft.com/v1.0/groups/{group.value[0].id}/members";

            try
            {
                AddDefaultRequestHeaders(_httpClient, "ConsistencyLevel", "eventual");

                var response = _httpClient.GetAsync(url).GetAwaiter().GetResult();

                string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                CheckStatusCode(response);

                var getMembersResponse = content.JsonToObject<GetGroupMemberResponse>();

                GroupMembers = new List<Member>();
                GroupMembers.AddRange(getMembersResponse.value);

                if (!string.IsNullOrEmpty(getMembersResponse.odatanextLink))
                {
                    GetMembersNextLink(getMembersResponse.odatanextLink);
                }

                return GroupMembers;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// get group members
        /// </summary>
        /// <param name="nextLink">next link url</param>
        /// <returns></returns>
        private void GetMembersNextLink(string nextLink)
        {
            try
            {
                AddDefaultRequestHeaders(_httpClient, "ConsistencyLevel", "eventual");

                var response = _httpClient.GetAsync(nextLink).GetAwaiter().GetResult();

                string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                CheckStatusCode(response);

                var getMembersResponse = content.JsonToObject<GetGroupMemberResponse>();
                GroupMembers.AddRange(getMembersResponse.value);

                if (!string.IsNullOrEmpty(getMembersResponse.odatanextLink))
                {
                    GetMembersNextLink(getMembersResponse.odatanextLink);
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// check user is member of a group, return true if is member of the group
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="groupDisplayName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        public bool IsMemberOf(string userEmail, string groupDisplayName)
        {
            var group = GetGroup(groupDisplayName);

            if (group == null || group.value == null || group.value.Count == 0)
            {
                throw new Exception($"group '{groupDisplayName}' not found");
            }

            UserHelper userHelper = new UserHelper(_httpClient);
            var user = userHelper.GetUserByMail(userEmail);
            if (user == null)
            {
                throw new Exception($"user '{userEmail}' not found");
            }

            string url = $"https://graph.microsoft.com/v1.0/groups/{group.value[0].id}/members?$count=true&$filter=mail eq '{userEmail}'";

            try
            {
                AddDefaultRequestHeaders(_httpClient, "ConsistencyLevel", "eventual");

                var response = _httpClient.GetAsync(url).GetAwaiter().GetResult();

                string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                CheckStatusCode(response);

                var result = content.JsonToObject<GetGroupMemberResponse>();
                return result.odatacount > 0;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// add user into a group, if user already exists, will throw exception
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="groupDisplayName"></param>
        /// <exception cref="Exception"></exception>
        public void AddMember(string userEmail, string groupDisplayName)
        {
            var group = GetGroup(groupDisplayName);
            if (group == null || group.value == null || group.value.Count == 0)
            {
                throw new Exception($"group '{groupDisplayName}' not found");
            }

            UserHelper userHelper = new UserHelper(_httpClient);
            var user = userHelper.GetUserByMail(userEmail);
            if (user == null)
            {
                throw new Exception($"user '{userEmail}' not found");
            }

            var newMember = new NewMemberModel
            {
                odataid = $"https://graph.microsoft.com/v1.0/directoryObjects/{user.id}"
            };

            string url = $"https://graph.microsoft.com/v1.0/groups/{group.value[0].id}/members/$ref";

            try
            {
                string jsonContent = newMember.ObjectToJson();

                HttpContent httpContent = new StringContent(jsonContent);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = _httpClient.PostAsync(url, httpContent).GetAwaiter().GetResult();

                string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                CheckStatusCode(response);
            }
            catch
            {
                throw;
            }
        }


        /// <summary>
        /// remove user from group when the user belong to the group, if not belong to group, do nothing
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="groupDisplayName"></param>
        /// <exception cref="Exception"></exception>
        public void RemoveMember(string userEmail, string groupDisplayName)
        {
            var group = GetGroup(groupDisplayName);
            if (group == null || group.value == null || group.value.Count == 0)
            {
                throw new Exception($"group '{groupDisplayName}' not found");
            }

            UserHelper userHelper = new UserHelper(_httpClient);
            var user = userHelper.GetUserByMail(userEmail);
            if (user == null)
            {
                throw new Exception($"user '{userEmail}' not found");
            }

            var isMember = IsMemberOf(userEmail, groupDisplayName);

            if (isMember)
            {
                string url = $"https://graph.microsoft.com/v1.0/groups/{group.value[0].id}/members/{user.id}/$ref";

                try
                {
                    var response = _httpClient.DeleteAsync(url).GetAwaiter().GetResult();
                    CheckStatusCode(response);
                }
                catch
                {
                    throw;
                }
            }           
        }

        /// <summary>
        /// new member object for add member into group
        /// </summary>
        private class NewMemberModel
        {
            [JsonProperty("@odata.id")]
            public string odataid { get; set; }
        }
    }
}
