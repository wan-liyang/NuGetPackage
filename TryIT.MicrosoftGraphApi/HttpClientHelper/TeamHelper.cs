using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using TryIT.MicrosoftGraphApi.Helper;
using TryIT.MicrosoftGraphApi.Model;
using TryIT.MicrosoftGraphApi.Model.Team;
using TryIT.MicrosoftGraphApi.Request.Team;
using TryIT.MicrosoftGraphApi.Response.Team;

namespace TryIT.MicrosoftGraphApi.HttpClientHelper
{
    /// <summary>
    /// Use the Microsoft Graph API to work with Microsoft Teams
    /// https://docs.microsoft.com/en-us/graph/api/resources/teams-api-overview?view=graph-rest-1.0
    /// </summary>
    internal class TeamHelper : BaseHelper
    {
        private readonly string _teamNamePolicy;
        public TeamHelper(MsGraphApiConfig config) : base(config) { }
        public TeamHelper(MsGraphApiConfig config, string teamNamePolicy) : base(config) 
        {
            _teamNamePolicy = teamNamePolicy;
        }

        /// <summary>
        /// create a team
        /// </summary>
        /// <param name="createTeam"></param>
        public void CreateTeam(CreateTeamModel createTeam)
        {
            string url = $"{GraphApiRootUrl}/teams";

            CreateTeamRequest.Body request = new CreateTeamRequest.Body
            {
                templateodatabind = $"{GraphApiRootUrl}/teamsTemplates('standard')",
                displayName = createTeam.DisplayName,
                description = createTeam.Description
            };

            HttpContent httpContent = new StringContent(request.ObjectToJson());
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = RestApi.PostAsync(url, httpContent).GetAwaiter().GetResult();
            CheckStatusCode(response);
        }

        /// <summary>
        /// get specific user joinded teams, if <paramref name="userEmail"/> is empty, then get my joined teams
        /// </summary>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        public List<GetJoinedTeamResponse.Team> GetJoinedTeam(string userEmail)
        {
            string url = $"{GraphApiRootUrl}/users/{userEmail}/joinedTeams";

            if (string.IsNullOrEmpty(userEmail))
            {
                url = $"{GraphApiRootUrl}/me/joinedTeams";
            }

            var response = RestApi.GetAsync(url).GetAwaiter().GetResult();
            CheckStatusCode(response);

            string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return content.JsonToObject<GetJoinedTeamResponse.Response>().value;
        }

        /// <summary>
        /// get team by display name
        /// </summary>
        /// <param name="teamName"></param>
        /// <returns></returns>
        public GetJoinedTeamResponse.Team GetTeam(string teamName)
        {
            var teams = GetJoinedTeam("");

            string displayName = teamName;
            if (!string.IsNullOrEmpty(_teamNamePolicy))
            {
                displayName = _teamNamePolicy.Replace("{teamName}", teamName);
            }

            return teams.FirstOrDefault(p => p.displayName.IsEquals(displayName));
        }

        /// <summary>
        /// get team member list
        /// </summary>
        /// <param name="teamName"></param>
        /// <returns></returns>
        public List<GetMembersResponse.Member> GetMembers(string teamName)
        {
            List<GetMembersResponse.Member> result = new List<GetMembersResponse.Member>();

            string teamId = GetTeam(teamName).id;
            string url = $"{GraphApiRootUrl}/teams/{teamId}/members";

            var response = RestApi.GetAsync(url).GetAwaiter().GetResult();
            CheckStatusCode(response);

            string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var responseObj = content.JsonToObject<GetMembersResponse.Response>();

            result.AddRange(responseObj.value);

            if (!string.IsNullOrEmpty(responseObj.odatanextLink))
            {
                _getnextlink(responseObj.odatanextLink, result);
            }

            return result;
        }

        private void _getnextlink(string nextLink, List<GetMembersResponse.Member> list)
        {
            var response = RestApi.GetAsync(nextLink).GetAwaiter().GetResult();
            CheckStatusCode(response, RestApi.RetryResults);

            string content = response.Content.ReadAsStringAsync().Result;
            var responseObj = content.JsonToObject<GetMembersResponse.Response>();

            list.AddRange(responseObj.value);

            if (!string.IsNullOrEmpty(responseObj.odatanextLink))
            {
                _getnextlink(responseObj.odatanextLink, list);
            }
        }

        /// <summary>
        /// Add a member to a team using user principal name
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public AddMemberResponse.Response AddMember(AddMemberModel model)
        {
            string teamId = GetTeam(model.TeamName).id;
            string url = $"{GraphApiRootUrl}/teams/{teamId}/members";

            AddMemberRequest.Body request = new AddMemberRequest.Body
            {
                odatatype = "#microsoft.graph.aadUserConversationMember",
                userodatabind = $"{GraphApiRootUrl}/users('{model.UserEmail}')"
            };

            if (model.Role == MemberRoleEnum.owner)
            {
                request.roles = new List<string> { model.Role.ToString() };
            }
            else
            {
                request.roles = new List<string> { };
            }

            string jsonContent = request.ObjectToJson();

            HttpContent httpContent = new StringContent(jsonContent);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = RestApi.PostAsync(url, httpContent).GetAwaiter().GetResult();
            CheckStatusCode(response);

            string content = response.Content.ReadAsStringAsync().Result;
            return content.JsonToObject<AddMemberResponse.Response>();
        }

        /// <summary>
        /// Remove member from team
        /// </summary>
        /// <param name="teamId"></param>
        /// <param name="membershipId"></param>
        public void RemoveMember(string teamId, string membershipId)
        {
            string url = $"{GraphApiRootUrl}/teams/{teamId}/members/{membershipId}";

            var response = RestApi.DeleteAsync(url).GetAwaiter().GetResult();
            CheckStatusCode(response);
        }

        /// <summary>
        /// get channels from a team
        /// </summary>
        /// <param name="teamName"></param>
        /// <returns></returns>
        public GetChannelResponse.Response GetChannels(string teamName)
        {
            string teamId = GetTeam(teamName).id;
            string url = $"{GraphApiRootUrl}/teams/{teamId}/channels";

            var response = RestApi.GetAsync(url).GetAwaiter().GetResult();
            CheckStatusCode(response);

            string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return content.JsonToObject<GetChannelResponse.Response>();
        }

        /// <summary>
        /// get channel from a team by channel name
        /// </summary>
        /// <param name="teamName"></param>
        /// <param name="channelName"></param>
        /// <returns></returns>
        public GetChannelResponse.Channel GetChannel(string teamName, string channelName)
        {
            var response = GetChannels(teamName);
            return response.value.FirstOrDefault(p => p.displayName.Equals(channelName, StringComparison.CurrentCultureIgnoreCase));
        }


        /// <summary>
        /// create team channel
        /// </summary>
        /// <param name="model"></param>
        public void CreateChannel(CreateChannelModel model)
        {
            string teamId = GetTeam(model.TeamName).id;
            string url = $"{GraphApiRootUrl}/teams/{teamId}/channels";

            CreateChannelRequest.Body request = new CreateChannelRequest.Body
            {
                displayName = model.ChannelName,
                description = model.Description,
                membershipType = "standard"
            };

            string jsonContent = request.ObjectToJson();
            HttpContent httpContent = new StringContent(jsonContent);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = RestApi.PostAsync(url, httpContent).GetAwaiter().GetResult();
            CheckStatusCode(response);
        }

        /// <summary>
        /// add channel member
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public AddMemberResponse AddChannelMember(AddChannelMemberModel model)
        {
            string teamId = GetTeam(model.TeamName).id;
            string channelId = GetChannel(model.TeamName, model.ChannelName).id;

            string url = $"{GraphApiRootUrl}/teams/{teamId}/channels/{channelId}/members";

            AddMemberRequest.Body request = new AddMemberRequest.Body
            {
                odatatype = "#microsoft.graph.aadUserConversationMember",
                roles = new List<string> { model.Role.ToString() },
                userodatabind = $"{GraphApiRootUrl}/users('{model.UserEmail}')"
            };

            string jsonContent = request.ObjectToJson();

            HttpContent httpContent = new StringContent(jsonContent);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = RestApi.PostAsync(url, httpContent).GetAwaiter().GetResult();
            CheckStatusCode(response);
            string content = response.Content.ReadAsStringAsync().Result;

            return content.JsonToObject<AddMemberResponse>();
        }

        public GetFilesFolderResponse.Response GetChannelSharepoint(string teamName, string channelName)
        {
            string teamId = GetTeam(teamName).id;
            string channelId = GetChannel(teamName, channelName).id;

            string url = $"{GraphApiRootUrl}/teams/{teamId}/channels/{channelId}/filesFolder";

            var response = RestApi.GetAsync(url).GetAwaiter().GetResult();
            CheckStatusCode(response);

            string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return content.JsonToObject<GetFilesFolderResponse.Response>();
        }
    }
}
