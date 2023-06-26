using System.Collections.Generic;
using TryIT.MicrosoftGraphService.ApiModel;
using TryIT.MicrosoftGraphService.Helper;
using TryIT.MicrosoftGraphService.HttpClientHelper;
using TryIT.MicrosoftGraphService.Model;

namespace TryIT.MicrosoftGraphService.MsGraphApi
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
        public GroupModel GetGroup(string groupDisplayName)
        {
            var group = _helper.GetGroup(groupDisplayName);

            return group.ToGroupModel();
        }

        /// <summary>
        /// get group members
        /// </summary>
        /// <param name="groupDisplayName"></param>
        /// <returns></returns>
        public List<GroupMemberModel> GetMembers(string groupDisplayName)
        {
            var members = _helper.GetMembers(groupDisplayName);

            return members.ToGroupModels();
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
