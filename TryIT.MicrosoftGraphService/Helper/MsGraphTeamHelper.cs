using TryIT.MicrosoftGraphService.HttpClientHelper;
using TryIT.MicrosoftGraphService.ApiModel;
using TryIT.MicrosoftGraphService.Model;
using System.Collections.Generic;
using System.Linq;

namespace TryIT.MicrosoftGraphService.Helper
{
    internal class MsGraphTeamHelper
    {
        private static TeamHelper _helper;
        public MsGraphTeamHelper(MsGraphApiConfig config)
        {
            MsGraphHelper graphHelper = new MsGraphHelper(config);
            _helper = new TeamHelper(graphHelper.GetHttpClient());
        }

        public TeamModel.Team GetTeam(string teamName, string userEmail)
        {
            var teams = _helper.GetJoinedTeam(userEmail);

            var team = teams.value.Where(p => p.displayName == teamName).First();
            return team.ToTeamModel_Team();
        }

        /// <summary>
        /// get list members of team, the <paramref name="userEmail"/> need be in the team, so that have permission to get the list
        /// </summary>
        /// <param name="teamName"></param>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        public List<TeamModel.Member> GetMembers(string teamName, string userEmail)
        {
            var team = GetTeam(teamName, userEmail);
            var list = _helper.GetMembers(team.Id).value;

            return list.Select(p => p.ToTeamModel_Member()).ToList();
        }

        public void AddMember(string teamName, string userEmail)
        {
            TeamAddMemberModel model = new TeamAddMemberModel
            {
                EmailAddress = userEmail,
                Roles = "member"
            };

            var team = GetTeam(teamName, userEmail);

            _helper.AddMember(team.Id, model);
        }

        /// <summary>
        /// remove member from team by <paramref name="membershipId"/>
        /// </summary>
        /// <param name="teamName"></param>
        /// <param name="userEmail"></param>
        /// <param name="membershipId"></param>
        public void RemoveMember(string teamName, string userEmail, string membershipId)
        {
            var team = GetTeam(teamName, userEmail);

            _helper.RemoveMember(team.Id, membershipId);
        }
    }
}
