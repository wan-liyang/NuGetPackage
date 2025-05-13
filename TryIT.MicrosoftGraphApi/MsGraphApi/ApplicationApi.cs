using System.Collections.Generic;
using TryIT.MicrosoftGraphApi.Helper;
using TryIT.MicrosoftGraphApi.HttpClientHelper;
using TryIT.MicrosoftGraphApi.Model;
using TryIT.MicrosoftGraphApi.Response.Application;

namespace TryIT.MicrosoftGraphApi.MsGraphApi
{
    /// <summary>
    /// application api
    /// </summary>
    public class ApplicationApi
    {
        private readonly ApplicationHelper _helper;
        /// <summary>
        /// init application api with configuration
        /// </summary>
        /// <param name="config"></param>
        public ApplicationApi(MsGraphApiConfig config)
        {
            _helper = new ApplicationHelper(config);
        }

        /// <summary>
        /// get application
        /// </summary>
        /// <param name="appDisplayName">application display name</param>
        /// <returns></returns>
        public GetAppliationResponse.Appliation GetApplication(string appDisplayName)
        {
            return _helper.GetApplication(appDisplayName);
        }

        /// <summary>
        /// get owned applications by user id
        /// </summary>
        /// <param name="userId">userPrincipalName or objectId</param>
        /// <returns></returns>
        public List<GetAppliationResponse.Appliation> GetOwnedApplications(string userId)
        {
            return _helper.GetOwnedApplications(userId);
        }
    }
}
