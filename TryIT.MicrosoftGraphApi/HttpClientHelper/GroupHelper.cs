using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using TryIT.MicrosoftGraphApi.Helper;
using TryIT.MicrosoftGraphApi.Model;
using TryIT.MicrosoftGraphApi.Response.Group;

namespace TryIT.MicrosoftGraphApi.HttpClientHelper
{
    internal class GroupHelper : BaseHelper
    {
        private readonly UserHelper _userHelper;
        public GroupHelper(MsGraphApiConfig config) : base(config)
        {
            _userHelper = new UserHelper(config);
        }

        public GetGroupResponse.Group GetGroup(string groupDisplayName)
        {
            if (string.IsNullOrEmpty(groupDisplayName))
            {
                throw new ArgumentNullException(nameof(groupDisplayName));
            }

            string url = $"{GraphApiRootUrl}/groups?$filter={EscapeExpression($"displayName eq '{groupDisplayName}'")}";

            AddDefaultRequestHeaders(HttpClient, "ConsistencyLevel", "eventual");

            var response = RestApi.GetAsync(url).GetAwaiter().GetResult();
            CheckStatusCode(response);

            string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return content.JsonToObject<GetGroupResponse.Response>().value.FirstOrDefault();
        }

        private List<GetGroupMemberResponse.Member> GroupMembers;
        /// <summary>
        /// get group members
        /// </summary>
        /// <param name="groupDisplayName">group display name</param>
        /// <returns></returns>
        public List<GetGroupMemberResponse.Member> GetMembers(string groupDisplayName)
        {
            var group = GetGroup(groupDisplayName);

            if (group == null)
            {
                throw new Exception($"group '{groupDisplayName}' not found");
            }

            string url = $"{GraphApiRootUrl}/groups/{group.id}/members";

            AddDefaultRequestHeaders(HttpClient, "ConsistencyLevel", "eventual");

            var response = RestApi.GetAsync(url).GetAwaiter().GetResult();
            CheckStatusCode(response);

            string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            var getMembersResponse = content.JsonToObject<GetGroupMemberResponse.Response>();

            GroupMembers = new List<GetGroupMemberResponse.Member>();
            GroupMembers.AddRange(getMembersResponse.value);

            if (!string.IsNullOrEmpty(getMembersResponse.odatanextLink))
            {
                GetMembersNextLink(getMembersResponse.odatanextLink);
            }

            return GroupMembers;
        }

        /// <summary>
        /// get group members
        /// </summary>
        /// <param name="nextLink">next link url</param>
        /// <returns></returns>
        private void GetMembersNextLink(string nextLink)
        {
            AddDefaultRequestHeaders(HttpClient, "ConsistencyLevel", "eventual");

            var response = RestApi.GetAsync(nextLink).GetAwaiter().GetResult();
            CheckStatusCode(response);

            string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            var getMembersResponse = content.JsonToObject<GetGroupMemberResponse.Response>();
            GroupMembers.AddRange(getMembersResponse.value);

            if (!string.IsNullOrEmpty(getMembersResponse.odatanextLink))
            {
                GetMembersNextLink(getMembersResponse.odatanextLink);
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

            if (group == null)
            {
                throw new Exception($"group '{groupDisplayName}' not found");
            }

            var user = _userHelper.GetUserByMail(userEmail);
            if (user == null)
            {
                throw new Exception($"user '{userEmail}' not found");
            }

            string url = $"{GraphApiRootUrl}/groups/{group.id}/members?$count=true&$filter={EscapeExpression($"mail eq '{userEmail}'")}";

            AddDefaultRequestHeaders(HttpClient, "ConsistencyLevel", "eventual");

            var response = RestApi.GetAsync(url).GetAwaiter().GetResult();
            CheckStatusCode(response);

            string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            var result = content.JsonToObject<GetGroupMemberResponse.Response>();
            return result.odatacount > 0;
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
            if (group == null)
            {
                throw new Exception($"group '{groupDisplayName}' not found");
            }

            var user = _userHelper.GetUserByMail(userEmail);
            if (user == null)
            {
                throw new Exception($"user '{userEmail}' not found");
            }

            var newMember = new NewMemberModel
            {
                odataid = $"{GraphApiRootUrl}/directoryObjects/{user.id}"
            };

            string url = $"{GraphApiRootUrl}/groups/{group.id}/members/$ref";

            string jsonContent = newMember.ObjectToJson();

            HttpContent httpContent = new StringContent(jsonContent);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = RestApi.PostAsync(url, httpContent).GetAwaiter().GetResult();
            CheckStatusCode(response);
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
            if (group == null)
            {
                throw new Exception($"group '{groupDisplayName}' not found");
            }

            var user = _userHelper.GetUserByMail(userEmail);
            if (user == null)
            {
                throw new Exception($"user '{userEmail}' not found");
            }

            var isMember = IsMemberOf(userEmail, groupDisplayName);

            if (isMember)
            {
                string url = $"{GraphApiRootUrl}/groups/{group.id}/members/{user.id}/$ref";

                var response = RestApi.DeleteAsync(url).GetAwaiter().GetResult();
                CheckStatusCode(response);
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
