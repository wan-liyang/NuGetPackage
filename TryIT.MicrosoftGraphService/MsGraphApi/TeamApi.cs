using System;
using System.Collections.Generic;
using System.Linq;
using TryIT.MicrosoftGraphService.ApiModel.Team;
using TryIT.MicrosoftGraphService.Helper;
using TryIT.MicrosoftGraphService.HttpClientHelper;
using TryIT.MicrosoftGraphService.Model;
using TryIT.MicrosoftGraphService.Model.Team;

namespace TryIT.MicrosoftGraphService.MsGraphApi
{
    /// <summary>
    /// Teams api
    /// </summary>
    public class TeamApi
    {
        private TeamHelper _helper;

        /// <summary>
        /// init Teams api with configuration
        /// </summary>
        /// <param name="config"></param>
        /// <param name="teamNamePolicy">prefix-{teamName}-suffix</param>
        public TeamApi(MsGraphApiConfig config, string teamNamePolicy)
        {
            MsGraphHelper graphHelper = new MsGraphHelper(config);
            _helper = new TeamHelper(graphHelper.GetHttpClient(), teamNamePolicy);
        }

        /// <summary>
        /// create a team, 
        /// </summary>
        /// <param name="createTeam"></param>
        /// <param name="force">force create new team if team with same name exists</param>
        /// <returns>true: new team created successfully, false: team with same name already exists</returns>
        public bool CreateTeam(CreateTeamModel createTeam, bool force = false)
        {
            if (!force)
            {
                var team = _helper.GetTeam(createTeam.DisplayName);
                if (team != null)
                {
                    return false;
                }
            }

            _helper.CreateTeam(createTeam);
            return true;
        }

        /// <summary>
        /// get joined team
        /// </summary>
        /// <returns></returns>
        public List<TeamModel.Team> GetJoinedTeam()
        {
            var response = _helper.GetJoinedTeam("");

            var teams = response.value.Select(p => p.ToTeamModel()).ToList();
            return teams;
        }

        /// <summary>
        /// add member to a team
        /// </summary>
        /// <param name="addMember"></param>
        public void AddMember(AddMemberModel addMember)
        {
            _helper.AddMember(addMember);
        }

        /// <summary>
        /// create channel when same name channel not exists
        /// </summary>
        /// <param name="model"></param>
        public void CreateChannel(CreateChannelModel model)
        {
            var channel = _helper.GetChannel(model.TeamName, model.ChannelName);

            if (channel == null)
            {
                _helper.CreateChannel(model);
            }
        }

        /// <summary>
        /// add channel member
        /// </summary>
        /// <param name="model"></param>
        public void AddChannelMember(AddChannelMemberModel model)
        {
            _helper.AddChannelMember(model);
        }

        /// <summary>
        /// get channel sharepoint information
        /// </summary>
        /// <param name="teamName"></param>
        /// <param name="channelName"></param>
        /// <returns></returns>
        public ChannelSharepointModel GetChannelSharepoint(string teamName, string channelName)
        {
            var response = _helper.GetChannelSharepoint(teamName, channelName);
            return response.ToChannelSharepointModel();
        }
    }
}
