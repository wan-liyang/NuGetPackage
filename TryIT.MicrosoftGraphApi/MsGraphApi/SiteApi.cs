using System.Collections.Generic;
using System.Threading.Tasks;
using TryIT.MicrosoftGraphApi.Helper;
using TryIT.MicrosoftGraphApi.HttpClientHelper;
using TryIT.MicrosoftGraphApi.Model;
using TryIT.MicrosoftGraphApi.Request.Group;
using TryIT.MicrosoftGraphApi.Request.Site;
using TryIT.MicrosoftGraphApi.Response.Group;
using TryIT.MicrosoftGraphApi.Response.Site;

namespace TryIT.MicrosoftGraphApi.MsGraphApi
{
    /// <summary>
    /// sharepoint site api, can use to get sharepoint site and drive information
    /// </summary>
    public class SiteApi
    {
        private readonly SiteHelper _siteHelper;
        private readonly GroupHelper _groupHelper;

        /// <summary>
        /// init Teams api with configuration
        /// </summary>
        /// <param name="config">configuration for api request, e.g token, timeout, proxy etc</param>
        public SiteApi(MsGraphApiConfig config)
        {
            _siteHelper = new SiteHelper(config);
            _groupHelper = new GroupHelper(config);
        }

        /// <summary>
        /// get all list under a site
        /// </summary>
        /// <param name="siteName"></param>
        /// <param name="hostName"></param>
        /// <returns></returns>
        public GetSiteResponse.Response GetSite(string siteName, string hostName)
        {
            return _siteHelper.GetSite(siteName, hostName);
        }

        /// <summary>
        /// get drive information, to get driveId for sharepoint operation
        /// </summary>
        /// <param name="siteId"></param>
        /// <returns></returns>
        public GetDriveResponse.Response GetDrive(string siteId)
        {
            return _siteHelper.GetDrive(siteId);
        }

        /// <summary>
        /// Create a sharepoint site, the site will be created with the group
        /// <para>https://learn.microsoft.com/en-us/graph/api/group-post-groups</para> with unified type
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<CreateGroupResponse.Response> CreateSiteAsync(CreateSiteRequest.Request request)
        {

            request.groupTypes = new List<string> { "Unified" };
            request.mailEnabled = false;
            request.mailNickname = UtilityHelper.ReplaceInvalidCharForGroupName(request.displayName);
            request.securityEnabled = true;
            request.visibility = "Private";

            CreateGroupRequest.Request groupRequest = new CreateGroupRequest.Request
            {
                displayName = request.displayName,
                description = request.description,
                groupTypes = request.groupTypes,
                mailEnabled = request.mailEnabled,
                mailNickname = request.mailNickname,
                securityEnabled = request.securityEnabled,
                visibility = request.visibility,
                membersodatabind = request.membersodatabind,
                ownersodatabind = request.ownersodatabind
            };

            // create group wit unified type, then a sharepoint site will be created with the group
            return await _groupHelper.CreateGroupAsync(groupRequest);
        }


        /// <summary>
        /// Get the SharePoint site associated with a Microsoft 365 group
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public async Task<GetSiteResponse.Response> GetGroupSite(string groupId)
        {
            return await _groupHelper.GetGroupSiteAsync(groupId);
        }
    }
}
