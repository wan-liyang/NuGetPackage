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
        private static ApplicationHelper _helper;
        /// <summary>
        /// init application api with configuration
        /// </summary>
        /// <param name="config"></param>
        public ApplicationApi(MsGraphApiConfig config)
        {
            MsGraphHelper graphHelper = new MsGraphHelper(config);
            _helper = new ApplicationHelper(graphHelper.GetHttpClient());
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
    }
}
