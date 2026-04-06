using System.Collections.Generic;
using System.Threading.Tasks;
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
        private readonly GroupHelper _helper;
        /// <summary>
        /// init application api with configuration
        /// </summary>
        /// <param name="config"></param>
        public GroupApi(MsGraphApiConfig config)
        {
            _helper = new GroupHelper(config);
        }

        /// <summary>
        /// get group
        /// </summary>
        /// <param name="groupDisplayName">group display name</param>
        /// <returns></returns>
        public async Task<GetGroupResponse.Group> GetGroupAsync(string groupDisplayName)
        {
            return await _helper.GetGroupAsync(groupDisplayName);
        }

        /// <summary>
        /// get group members
        /// </summary>
        /// <param name="groupDisplayName"></param>
        /// <returns></returns>
        public async Task<List<GetGroupMemberResponse.Member>> GetMembersAsync(string groupDisplayName)
        {
            return await _helper.GetMembersAsync(groupDisplayName);
        }

        /// <summary>
        /// check user is member of a group
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="groupDisplayName"></param>
        /// <returns></returns>
        public async Task<bool> IsMemberOfAsync(string userEmail, string groupDisplayName)
        {
            return await _helper.IsMemberOfAsync(userEmail, groupDisplayName);
        }

        /// <summary>
        /// add user into group
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="groupDisplayName"></param>
        public async Task AddMemberAsync(string userEmail, string groupDisplayName)
        {
            await _helper.AddMemberAsync(userEmail, groupDisplayName);
        }

        /// <summary>
        /// remove user from group when the user belong to the group, if not belong to group, do nothing
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="groupDisplayName"></param>
        public async Task RemoveMemberAsync(string userEmail, string groupDisplayName)
        {
            await _helper.RemoveMemberAsync(userEmail, groupDisplayName);
        }
    }
}
