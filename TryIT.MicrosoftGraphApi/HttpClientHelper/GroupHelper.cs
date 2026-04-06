using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TryIT.MicrosoftGraphApi.Helper;
using TryIT.MicrosoftGraphApi.Model;
using TryIT.MicrosoftGraphApi.Request.Group;
using TryIT.MicrosoftGraphApi.Response.Group;
using TryIT.MicrosoftGraphApi.Response.Site;

namespace TryIT.MicrosoftGraphApi.HttpClientHelper
{
    internal class GroupHelper : BaseHelper
    {
        private readonly UserHelper _userHelper;
        public GroupHelper(MsGraphApiConfig config) : base(config)
        {
            _userHelper = new UserHelper(config);
        }

        public async Task<GetGroupResponse.Group> GetGroupAsync(string groupDisplayName)
        {
            if (string.IsNullOrEmpty(groupDisplayName))
            {
                throw new ArgumentNullException(nameof(groupDisplayName));
            }

            string url = $"{GraphApiRootUrl}/groups?$filter={EscapeExpression($"displayName eq '{groupDisplayName}'")}";

            AddConsistencyLevelHeader(HttpClient);

            var response = await RestApi.GetAsync(url);
            CheckStatusCode(response);

            string content = await response.Content.ReadAsStringAsync();
            return content.JsonToObject<GetGroupResponse.Response>().value.FirstOrDefault();
        }

        private List<GetGroupMemberResponse.Member> GroupMembers;
        /// <summary>
        /// get group members
        /// </summary>
        /// <param name="groupDisplayName">group display name</param>
        /// <returns></returns>
        public async Task<List<GetGroupMemberResponse.Member>> GetMembersAsync(string groupDisplayName)
        {
            var group = await GetGroupAsync(groupDisplayName);

            if (group == null)
            {
                throw new InvalidOperationException($"group '{groupDisplayName}' not found");
            }

            string url = $"{GraphApiRootUrl}/groups/{group.id}/members";

            AddConsistencyLevelHeader(HttpClient);

            var response = await RestApi.GetAsync(url);
            CheckStatusCode(response);

            string content = await response.Content.ReadAsStringAsync();

            var getMembersResponse = content.JsonToObject<GetGroupMemberResponse.Response>();

            GroupMembers = new List<GetGroupMemberResponse.Member>();
            GroupMembers.AddRange(getMembersResponse.value);

            if (!string.IsNullOrEmpty(getMembersResponse.odatanextLink))
            {
                await GetMembersNextLinkAsync(getMembersResponse.odatanextLink);
            }

            return GroupMembers;
        }

        /// <summary>
        /// get group members
        /// </summary>
        /// <param name="nextLink">next link url</param>
        /// <returns></returns>
        private async Task GetMembersNextLinkAsync(string nextLink)
        {
            AddConsistencyLevelHeader(HttpClient);

            var response = await RestApi.GetAsync(nextLink);
            CheckStatusCode(response);

            string content = await response.Content.ReadAsStringAsync();

            var getMembersResponse = content.JsonToObject<GetGroupMemberResponse.Response>();
            GroupMembers.AddRange(getMembersResponse.value);

            if (!string.IsNullOrEmpty(getMembersResponse.odatanextLink))
            {
                await GetMembersNextLinkAsync(getMembersResponse.odatanextLink);
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
        public async Task<bool> IsMemberOfAsync(string userEmail, string groupDisplayName)
        {
            var group = await GetGroupAsync(groupDisplayName);

            if (group == null)
            {
                throw new InvalidOperationException($"group '{groupDisplayName}' not found");
            }

            var user = _userHelper.GetUserByMail(userEmail);
            if (user == null)
            {
                throw new InvalidOperationException($"user '{userEmail}' not found");
            }

            string url = $"{GraphApiRootUrl}/groups/{group.id}/members?$count=true&$filter={EscapeExpression($"mail eq '{userEmail}'")}";

            AddConsistencyLevelHeader(HttpClient);

            var response = await RestApi.GetAsync(url);
            CheckStatusCode(response);

            string content = await response.Content.ReadAsStringAsync();

            var result = content.JsonToObject<GetGroupMemberResponse.Response>();
            return result.odatacount > 0;
        }

        /// <summary>
        /// add user into a group, if user already exists, will throw exception
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="groupDisplayName"></param>
        /// <exception cref="Exception"></exception>
        public async Task AddMemberAsync(string userEmail, string groupDisplayName)
        {
            var group = await GetGroupAsync(groupDisplayName);
            if (group == null)
            {
                throw new InvalidOperationException($"group '{groupDisplayName}' not found");
            }

            var user = _userHelper.GetUserByMail(userEmail);
            if (user == null)
            {
                throw new InvalidOperationException($"user '{userEmail}' not found");
            }

            var newMember = new NewMemberModel
            {
                odataid = $"{GraphApiRootUrl}/directoryObjects/{user.id}"
            };

            string url = $"{GraphApiRootUrl}/groups/{group.id}/members/$ref";
            HttpContent httpContent = GetJsonHttpContent(newMember);
            var response = await RestApi.PostAsync(url, httpContent);
            CheckStatusCode(response);
        }


        /// <summary>
        /// remove user from group when the user belong to the group, if not belong to group, do nothing
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="groupDisplayName"></param>
        /// <exception cref="Exception"></exception>
        public async Task RemoveMemberAsync(string userEmail, string groupDisplayName)
        {
            var group = await GetGroupAsync(groupDisplayName);
            if (group == null)
            {
                throw new InvalidOperationException($"group '{groupDisplayName}' not found");
            }

            var user = _userHelper.GetUserByMail(userEmail);
            if (user == null)
            {
                throw new InvalidOperationException($"user '{userEmail}' not found");
            }

            var isMember = await IsMemberOfAsync(userEmail, groupDisplayName);

            if (isMember)
            {
                string url = $"{GraphApiRootUrl}/groups/{group.id}/members/{user.id}/$ref";

                var response = await RestApi.DeleteAsync(url);
                CheckStatusCode(response);
            }           
        }

        public async Task<CreateGroupResponse.Response> CreateGroupAsync(CreateGroupRequest.Request request)
        {
            string url = $"{GraphApiRootUrl}/groups";
            HttpContent httpContent = GetJsonHttpContent(request);
            var response = await RestApi.PostAsync(url, httpContent);
            CheckStatusCode(response);
            string content = await response.Content.ReadAsStringAsync();
            return content.JsonToObject<CreateGroupResponse.Response>();
        }

        /// <summary>
        /// use group id to get the sharepoint site associated with the group
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public async Task<GetSiteResponse.Response> GetGroupSiteAsync(string groupId)
        {
            string url = $"{GraphApiRootUrl}/groups/{groupId}/sites/root";
            var response = await RestApi.GetAsync(url);
            CheckStatusCode(response);
            string content = await response.Content.ReadAsStringAsync();
            return content.JsonToObject<GetSiteResponse.Response>();
        }

        /// <summary>
        /// new member object for add member into group
        /// </summary>
        private sealed class NewMemberModel
        {
            [JsonProperty("@odata.id")]
            public string odataid { get; set; }
        }
    }
}
