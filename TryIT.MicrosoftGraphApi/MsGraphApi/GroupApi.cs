using System.Collections.Generic;
using TryIT.MicrosoftGraphApi.Helper;
using TryIT.MicrosoftGraphApi.HttpClientHelper;
using TryIT.MicrosoftGraphApi.Model;
using TryIT.MicrosoftGraphApi.Response.Group;

namespace TryIT.MicrosoftGraphApi.MsGraphApi
{
    /// <summary>
    /// group api
    /// </summary>
    public class GroupApi
    {
        private static GroupHelper _helper;
        /// <summary>
        /// init application api with configuration
        /// </summary>
        /// <param name="config"></param>
        public GroupApi(MsGraphApiConfig config)
        {
            MsGraphHelper graphHelper = new MsGraphHelper(config);
            _helper = new GroupHelper(graphHelper.GetHttpClient());
        }

        /// <summary>
        /// get group
        /// </summary>
        /// <param name="groupDisplayName">group display name</param>
        /// <returns></returns>
        public GetGroupResponse.Group GetGroup(string groupDisplayName)
        {
            return _helper.GetGroup(groupDisplayName);
        }

        /// <summary>
        /// get group members
        /// </summary>
        /// <param name="groupDisplayName"></param>
        /// <returns></returns>
        public List<GetGroupMemberResponse.Member> GetMembers(string groupDisplayName)
        {
            return _helper.GetMembers(groupDisplayName);
        }

        /// <summary>
        /// check user is member of a group
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="groupDisplayName"></param>
        /// <returns></returns>
        public bool IsMemberOf(string userEmail, string groupDisplayName)
        {
            return _helper.IsMemberOf(userEmail, groupDisplayName);
        }

        /// <summary>
        /// add user into group
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="groupDisplayName"></param>
        public void AddMember(string userEmail, string groupDisplayName)
        {
            _helper.AddMember(userEmail, groupDisplayName);
        }

        /// <summary>
        /// remove user from group when the user belong to the group, if not belong to group, do nothing
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="groupDisplayName"></param>
        public void RemoveMember(string userEmail, string groupDisplayName)
        {
            _helper.RemoveMember(userEmail, groupDisplayName);
        }
    }
}
