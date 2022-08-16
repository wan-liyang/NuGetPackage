using MicrosoftGraphService.ExtensionHelper;
using MicrosoftGraphService.Model;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using static MicrosoftGraphService.Model.TeamResponse;

namespace MicrosoftGraphService.HttpClientHelper
{
    /// <summary>
    /// Use the Microsoft Graph API to work with Microsoft Teams
    /// https://docs.microsoft.com/en-us/graph/api/resources/teams-api-overview?view=graph-rest-1.0
    /// </summary>
    public class TeamHelper : BaseHelper
    {
        private HttpClient _httpClient;

        public TeamHelper(HttpClient httpClient)
        {
            if (null == httpClient)
                throw new ArgumentNullException(nameof(httpClient));

            _httpClient = httpClient;
        }

        /// <summary>
        /// get specific user joinded teams, if <paramref name="userEmail"/> is empty, then get my joined teams
        /// </summary>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        public GetJoinedTeamsResponse GetJoinedTeam(string userEmail)
        {
            string url = $"https://graph.microsoft.com/v1.0/users/{userEmail}/joinedTeams";

            if (string.IsNullOrEmpty(userEmail))
            {
                url = $"https://graph.microsoft.com/v1.0/me/joinedTeams";
            }

            try
            {
                var response = _httpClient.GetAsync(url).GetAwaiter().GetResult();

                string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                return content.JsonToObject<GetJoinedTeamsResponse>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// get team member list
        /// </summary>
        /// <param name="teamId"></param>
        /// <returns></returns>
        public GetMembersResponse GetMembers(string teamId)
        {
            string url = $"https://graph.microsoft.com/v1.0/teams/{teamId}/members";

            try
            {
                var response = _httpClient.GetAsync(url).GetAwaiter().GetResult();

                string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                return content.JsonToObject<GetMembersResponse>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Add a member to a team using user principal name
        /// </summary>
        /// <param name="teamId"></param>
        /// <param name="emailAddress"></param>
        /// <returns></returns>
        public AddMemberResponse AddMember(string teamId, TeamAddMemberModel model)
        {
            string url = $"https://graph.microsoft.com/v1.0/teams/{teamId}/members";

            try
            {
                string jsonContent = model.ObjectToJson();

                HttpContent httpContent = new StringContent(jsonContent);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = _httpClient.PostAsync(url, httpContent).GetAwaiter().GetResult();
                CheckStatusCode(response);
                string content = response.Content.ReadAsStringAsync().Result;

                return content.JsonToObject<AddMemberResponse>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Remove member from team
        /// </summary>
        /// <param name="teamId"></param>
        /// <param name="membershipId"></param>
        public void RemoveMember(string teamId, string membershipId)
        {
            string url = $"https://graph.microsoft.com/v1.0/teams/{teamId}/members/{membershipId}";

            try
            {
                var response = _httpClient.DeleteAsync(url).GetAwaiter().GetResult();
                CheckStatusCode(response);
                string content = response.Content.ReadAsStringAsync().Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
