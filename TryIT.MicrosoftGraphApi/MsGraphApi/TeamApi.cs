﻿using System.Collections.Generic;
using TryIT.MicrosoftGraphApi.Helper;
using TryIT.MicrosoftGraphApi.HttpClientHelper;
using TryIT.MicrosoftGraphApi.Model;
using TryIT.MicrosoftGraphApi.Model.Team;
using TryIT.MicrosoftGraphApi.Response.Team;

namespace TryIT.MicrosoftGraphApi.MsGraphApi
{
    /// <summary>
    /// Teams api
    /// </summary>
    public class TeamApi
    {
        private readonly TeamHelper _helper;

        /// <summary>
        /// init Teams api with configuration
        /// </summary>
        /// <param name="config"></param>
        public TeamApi(MsGraphApiConfig config)
        {
            _helper = new TeamHelper(config);
        }

        /// <summary>
        /// init Teams api with configuration
        /// </summary>
        /// <param name="config"></param>
        /// <param name="teamNamePolicy">prefix-{teamName}-suffix</param>
        public TeamApi(MsGraphApiConfig config, string teamNamePolicy)
        {
            _helper = new TeamHelper(config, teamNamePolicy);
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
        public List<GetJoinedTeamResponse.Team> GetJoinedTeam()
        {
            return _helper.GetJoinedTeam("");
        }

        /// <summary>
        /// get specific team
        /// </summary>
        /// <returns></returns>
        public GetJoinedTeamResponse.Team GetTeam(string teamName)
        {
            return _helper.GetTeam(teamName);
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
        /// Remove member from team, https://learn.microsoft.com/en-us/graph/api/team-delete-members?view=graph-rest-1.0&tabs=http
        /// </summary>
        /// <param name="teamId"></param>
        /// <param name="membershipId"></param>
        public void RemoveMember(string teamId, string membershipId)
        {
            _helper.RemoveMember(teamId, membershipId);
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
        public GetFilesFolderResponse.Response GetChannelSharepoint(string teamName, string channelName)
        {
            return _helper.GetChannelSharepoint(teamName, channelName);
        }

        /// <summary>
        /// get team member list
        /// </summary>
        /// <param name="teamName"></param>
        /// <returns></returns>
        public List<GetMembersResponse.Member> GetMembers(string teamName)
        { 
            return _helper.GetMembers(teamName);
        }
    }
}
