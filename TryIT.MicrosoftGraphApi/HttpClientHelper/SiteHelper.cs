using TryIT.MicrosoftGraphApi.Helper;
using TryIT.MicrosoftGraphApi.Model;
using TryIT.MicrosoftGraphApi.Response.Site;

namespace TryIT.MicrosoftGraphApi.HttpClientHelper
{
    internal class SiteHelper : BaseHelper
    {
        public SiteHelper(MsGraphApiConfig config) : base(config) 
        {
        }

        public GetSiteResponse.Response GetSite(string siteName, string hostName)
        {
            string url = $"{GraphApiRootUrl}/sites/{hostName}:/sites/{siteName}";

            var response = RestApi.GetAsync(url).GetAwaiter().GetResult();
            CheckStatusCode(response, RestApi.RetryResults);

            string content = response.Content.ReadAsStringAsync().Result;
            return content.JsonToObject<GetSiteResponse.Response>();
        }

        public GetDriveResponse.Response GetDrive(string siteId)
        {
            string url = $"{GraphApiRootUrl}/sites/{siteId}/drive";

            var response = RestApi.GetAsync(url).GetAwaiter().GetResult();
            CheckStatusCode(response, RestApi.RetryResults);

            string content = response.Content.ReadAsStringAsync().Result;
            return content.JsonToObject<GetDriveResponse.Response>();
        }

        public GetSiteResponse.Response GetSiteByUrl(string folderUrl)
        {
            folderUrl = folderUrl.Replace("https://", "");
            string host = folderUrl.Substring(0, folderUrl.IndexOf('/'));

            folderUrl = folderUrl.Replace($"{host}/sites/", "");
            string site = folderUrl.Substring(0, folderUrl.IndexOf('/'));

            string url = $"{GraphApiRootUrl}/sites/{host}:/sites/{site}";

            var response = RestApi.GetAsync(url).GetAwaiter().GetResult();
            CheckStatusCode(response, RestApi.RetryResults);

            string content = response.Content.ReadAsStringAsync().Result;
            return content.JsonToObject<GetSiteResponse.Response>();
        }
    }
}
