using TryIT.MicrosoftGraphApi.Helper;
using TryIT.MicrosoftGraphApi.HttpClientHelper;
using TryIT.MicrosoftGraphApi.Model;
using TryIT.MicrosoftGraphApi.Response.Site;

namespace TryIT.MicrosoftGraphApi.MsGraphApi
{
    /// <summary>
    /// sharepoint site api, can use to get sharepoint site and drive information
    /// </summary>
    public class SiteApi
    {
        private readonly SiteHelper _helper;

        /// <summary>
        /// init Teams api with configuration
        /// </summary>
        /// <param name="config">configuration for api request, e.g token, timeout, proxy etc</param>
        /// <param name="hostName">the host(domain) of the site, use for api request to get site under specific host</param>
        public SiteApi(MsGraphApiConfig config, string hostName)
        {
            _helper = new SiteHelper(config, hostName);
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
