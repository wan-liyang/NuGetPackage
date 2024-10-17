using TryIT.MicrosoftGraphApi.Helper;
using TryIT.MicrosoftGraphApi.HttpClientHelper;
using TryIT.MicrosoftGraphApi.Model;
using TryIT.MicrosoftGraphApi.Response.Site;

namespace TryIT.MicrosoftGraphApi.MsGraphApi
{
    public class SiteApi
    {
        private SiteHelper _helper;

        /// <summary>
        /// init Teams api with configuration
        /// </summary>
        /// <param name="config"></param>
        /// <param name="hostName"></param>
        public SiteApi(MsGraphApiConfig config, string hostName)
        {
            MsGraphHelper graphHelper = new MsGraphHelper(config);
            _helper = new SiteHelper(graphHelper.GetHttpClient(), hostName);
        }

        /// <summary>
        /// get all list under a site
        /// </summary>
        /// <param name="siteName"></param>
        /// <returns></returns>
        public GetSiteResponse.Response GetSite(string siteName)
        {
            return _helper.GetSite(siteName);
        }

        /// <summary>
        /// get drive information, to get driveId for sharepoint operation
        /// </summary>
        /// <param name="siteId"></param>
        /// <returns></returns>
        public GetDriveResponse.Response GetDrive(string siteId)
        {
            return _helper.GetDrive(siteId);
        }
    }
}
