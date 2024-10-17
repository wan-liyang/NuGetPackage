using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using TryIT.MicrosoftGraphApi.Helper;
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
        private HttpClient _httpClient;
        private string _teamNamePolicy;

        /// <summary>
        /// init team api
        /// </summary>
        /// <param name="httpClient"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public TeamHelper(HttpClient httpClient)
        {
            if (null == httpClient)
                throw new ArgumentNullException(nameof(httpClient));

            _httpClient = httpClient;
        }

        /// <summary>
        /// init team api
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="teamNamePolicy">prefix-{teamName}-suffix</param>
        /// <exception cref="ArgumentNullException"></exception>
        public TeamHelper(HttpClient httpClient, string teamNamePolicy)
        {
            if (null == httpClient)
                throw new ArgumentNullException(nameof(httpClient));

            _httpClient = httpClient;
            _teamNamePolicy = teamNamePolicy;
        }

        /// <summary>
        /// create a team
        /// </summary>
        /// <param name="createTeam"></param>
        public void CreateTeam(CreateTeamModel createTeam)
        {
            string url = $"{GraphApiRootUrl}/teams";

            try
            {
                CreateTeamRequest.Body request = new CreateTeamRequest.Body
                {
                    templateodatabind = $"{GraphApiRootUrl}/teamsTemplates('standard')",
                    displayName = createTeam.DisplayName,
                    description = createTeam.Description
                };

                HttpContent httpContent = new StringContent(request.ObjectToJson());
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = _httpClient.PostAsync(url, httpContent).GetAwaiter().GetResult();
                CheckStatusCode(response);
            }
            catch
            {
                throw;
            }
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

            try
            {
                var response = _httpClient.GetAsync(url).GetAwaiter().GetResult();
                CheckStatusCode(response);

                string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                return content.JsonToObject<GetJoinedTeamResponse.Response>().value;
            }
            catch
            {
                throw;
            }
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
            string teamId = GetTeam(teamName).id;
            string url = $"{GraphApiRootUrl}/teams/{teamId}/members";

            try
            {
                var response = _httpClient.GetAsync(url).GetAwaiter().GetResult();
                CheckStatusCode(response);

                string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                return content.JsonToObject<GetMembersResponse.Response>().value;
            }
            catch
            {
                throw;
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
            try
            {
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

                var response = _httpClient.PostAsync(url, httpContent).GetAwaiter().GetResult();
                CheckStatusCode(response);

                string content = response.Content.ReadAsStringAsync().Result;
                return content.JsonToObject<AddMemberResponse.Response>();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Remove member from team
        /// </summary>
        /// <param name="teamId"></param>
        /// <param name="membershipId"></param>
        public void RemoveMember(string teamId, string membershipId)
        {
            string url = $"{GraphApiRootUrl}/teams/{teamId}/members/{membershipId}";

            try
            {
                var response = _httpClient.DeleteAsync(url).GetAwaiter().GetResult();
                CheckStatusCode(response);
                string content = response.Content.ReadAsStringAsync().Result;
            }
            catch
            {
                throw;
            }
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

            try
            {
                var response = _httpClient.GetAsync(url).GetAwaiter().GetResult();
                CheckStatusCode(response);

                string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                return content.JsonToObject<GetChannelResponse.Response>();
            }
            catch
            {
                throw;
            }

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

            try
            {
                CreateChannelRequest.Body request = new CreateChannelRequest.Body
                {
                    displayName = model.ChannelName,
                    description = model.Description,
                    membershipType = "standard"
                };

                string jsonContent = request.ObjectToJson();
                HttpContent httpContent = new StringContent(jsonContent);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = _httpClient.PostAsync(url, httpContent).GetAwaiter().GetResult();
                CheckStatusCode(response);
            }
            catch
            {
                throw;
            }
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

            try
            {
                AddMemberRequest.Body request = new AddMemberRequest.Body
                {
                    odatatype = "#microsoft.graph.aadUserConversationMember",
                    roles = new List<string> { model.Role.ToString() },
                    userodatabind = $"{GraphApiRootUrl}/users('{model.UserEmail}')"
                };

                string jsonContent = request.ObjectToJson();

                HttpContent httpContent = new StringContent(jsonContent);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = _httpClient.PostAsync(url, httpContent).GetAwaiter().GetResult();
                CheckStatusCode(response);
                string content = response.Content.ReadAsStringAsync().Result;

                return content.JsonToObject<AddMemberResponse>();
            }
            catch
            {
                throw;
            }
        }

        public GetFilesFolderResponse.Response GetChannelSharepoint(string teamName, string channelName)
        {
            string teamId = GetTeam(teamName).id;
            string channelId = GetChannel(teamName, channelName).id;

            string url = $"{GraphApiRootUrl}/teams/{teamId}/channels/{channelId}/filesFolder";
            try
            {
                var response = _httpClient.GetAsync(url).GetAwaiter().GetResult();
                CheckStatusCode(response);

                string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                return content.JsonToObject<GetFilesFolderResponse.Response>();
            }
            catch
            {
                throw;
            }
        }
    }
}
