using System;
using System.Net.Http;
using TryIT.MicrosoftGraphApi.Helper;
using TryIT.MicrosoftGraphApi.Response.Site;

namespace TryIT.MicrosoftGraphApi.HttpClientHelper
{
    internal class SiteHelper : BaseHelper
    {
        private TryIT.RestApi.Api api;
        private readonly string _hostName;
        public SiteHelper(HttpClient httpClient, string hostName)
        {
            if (null == httpClient)
                throw new ArgumentNullException(nameof(httpClient));

            // use RestApi library and enable retry
            api = new RestApi.Api(new RestApi.Models.ApiConfig
            {
                HttpClient = httpClient,
                EnableRetry = true,
            });
            _hostName = hostName;
        }

        public GetSiteResponse.Response GetSite(string siteName)
        {
            string url = $"{GraphApiRootUrl}/sites/{_hostName}:/sites/{siteName}";

            try
            {
                var response = api.GetAsync(url).GetAwaiter().GetResult();
                CheckStatusCode(response, api.RetryResults);

                string content = response.Content.ReadAsStringAsync().Result;
                return content.JsonToObject<GetSiteResponse.Response>();
            }
            catch
            {
                throw;
            }
        }

        public GetDriveResponse.Response GetDrive(string siteId)
        {
            string url = $"{GraphApiRootUrl}/sites/{siteId}/drive";

            try
            {
                var response = api.GetAsync(url).GetAwaiter().GetResult();
                CheckStatusCode(response, api.RetryResults);

                string content = response.Content.ReadAsStringAsync().Result;
                return content.JsonToObject<GetDriveResponse.Response>();
            }
            catch
            {
                throw;
            }
        }

        public GetSiteResponse.Response GetSiteByUrl(string folderUrl)
        {
            folderUrl = folderUrl.Replace("https://", "");
            string host = folderUrl.Substring(0, folderUrl.IndexOf('/'));

            folderUrl = folderUrl.Replace($"{host}/sites/", "");
            string site = folderUrl.Substring(0, folderUrl.IndexOf('/'));

            string url = $"{GraphApiRootUrl}/sites/{host}:/sites/{site}";

            try
            {
                var response = api.GetAsync(url).GetAwaiter().GetResult();
                CheckStatusCode(response, api.RetryResults);

                string content = response.Content.ReadAsStringAsync().Result;
                return content.JsonToObject<GetSiteResponse.Response>();
            }
            catch
            {
                throw;
            }
        }
    }
}
