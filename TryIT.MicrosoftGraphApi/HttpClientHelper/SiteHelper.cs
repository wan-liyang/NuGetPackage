using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using TryIT.MicrosoftGraphApi.Helper;
using TryIT.MicrosoftGraphApi.Response.Sharepoint;

namespace TryIT.MicrosoftGraphApi.HttpClientHelper
{
    internal class SiteHelper : BaseHelper
    {
        private TryIT.RestApi.Api api;

        public SiteHelper(HttpClient httpClient)
        {
            if (null == httpClient)
                throw new ArgumentNullException(nameof(httpClient));

            // use RestApi library and enable retry
            api = new RestApi.Api(new RestApi.Models.ApiConfig
            {
                HttpClient = httpClient,
                EnableRetry = true,
            });
        }

        public GetSiteResponse.Site GetSite(string siteName)
        {
            string url = $"{GraphApiRootUrl}/sites/groupncs.sharepoint.com:/sites/{siteName}";

            try
            {
                var response = api.GetAsync(url).GetAwaiter().GetResult();
                CheckStatusCode(response, api.RetryResults);

                string content = response.Content.ReadAsStringAsync().Result;
                return content.JsonToObject<GetSiteResponse.Site>();
            }
            catch
            {
                throw;
            }
        }

        public GetSiteResponse.Site GetSiteByUrl(string folderUrl)
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
                return content.JsonToObject<GetSiteResponse.Site>();
            }
            catch
            {
                throw;
            }
        }
    }
}
