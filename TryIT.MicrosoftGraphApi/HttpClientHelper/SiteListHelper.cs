using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using TryIT.MicrosoftGraphApi.Helper;
using TryIT.MicrosoftGraphApi.Response.SiteList;

namespace TryIT.MicrosoftGraphApi.HttpClientHelper
{
    internal class SiteListHelper : BaseHelper
    {
        private TryIT.RestApi.Api api;
        private readonly SiteHelper _siteHelper;
        public SiteListHelper(HttpClient httpClient, string hostName)
        {
            if (null == httpClient)
                throw new ArgumentNullException(nameof(httpClient));

            // use RestApi library and enable retry
            api = new RestApi.Api(new RestApi.Models.ApiConfig
            {
                HttpClient = httpClient,
                EnableRetry = true,
            });

            _siteHelper = new SiteHelper(httpClient, hostName);
        }

        /// <summary>
        /// get all list under a site
        /// </summary>
        /// <param name="siteName"></param>
        /// <returns></returns>
        public List<GetListResponse.SiteList> GetAllList(string siteName)
        {
            string siteId = _siteHelper.GetSite(siteName).id;

            string url = $"{GraphApiRootUrl}/sites/{siteId}/lists";

            try
            {
                var response = api.GetAsync(url).GetAwaiter().GetResult();
                CheckStatusCode(response, api.RetryResults);

                string content = response.Content.ReadAsStringAsync().Result;
                return content.JsonToObject<GetListResponse.Response>().value;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// get a specific list under a site
        /// </summary>
        /// <param name="siteName"></param>
        /// <param name="listName"></param>
        /// <returns></returns>
        public GetListResponse.SiteList GetList(string siteName, string listName)
        {
            string siteId = _siteHelper.GetSite(siteName).id;

            string url = $"{GraphApiRootUrl}/sites/{siteId}/lists?$filter={EscapeExpression($"DisplayName eq '{listName}'")}";

            try
            {
                var response = api.GetAsync(url).GetAwaiter().GetResult();
                CheckStatusCode(response, api.RetryResults);

                string content = response.Content.ReadAsStringAsync().Result;
                var lists = content.JsonToObject<GetListResponse.Response>().value;

                return lists.Where(p => p.displayName.IsEquals(listName)).FirstOrDefault();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// get items with default columns, no other customize column
        /// </summary>
        /// <param name="siteName"></param>
        /// <param name="listName"></param>
        /// <returns></returns>
        public List<GetItemResponse.Item> GetItems(string siteName, string listName)
        {
            string siteId = _siteHelper.GetSite(siteName).id;
            string listId = GetList(siteName, listName).id;

            string url = $"{GraphApiRootUrl}/sites/{siteId}/lists/{listId}/items?expand=fields";

            try
            {
                var response = api.GetAsync(url).GetAwaiter().GetResult();
                CheckStatusCode(response, api.RetryResults);

                string content = response.Content.ReadAsStringAsync().Result;
                return content.JsonToObject<GetItemResponse.Response>().value;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// get specific item from list as json string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="siteName"></param>
        /// <param name="listName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public T GetItem<T>(string siteName, string listName, string id)
        {
            string siteId = _siteHelper.GetSite(siteName).id;
            string listId = GetList(siteName, listName).id;

            string url = $"{GraphApiRootUrl}/sites/{siteId}/lists/{listId}/items/{id}";

            try
            {
                var response = api.GetAsync(url).GetAwaiter().GetResult();
                CheckStatusCode(response, api.RetryResults);

                string content = response.Content.ReadAsStringAsync().Result;
                return content.GetJsonValue<T>("fields");
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// create a item into list, refer to https://learn.microsoft.com/en-us/graph/api/listitem-create?view=graph-rest-1.0&tabs=http
        /// </summary>
        /// <param name="siteName"></param>
        /// <param name="listName"></param>
        /// <param name="jsonBody">json body with necessary columns in list</param>
        /// <returns></returns>
        public GetItemResponse.Item CreateItem(string siteName, string listName, string jsonBody)
        {
            string siteId = _siteHelper.GetSite(siteName).id;
            string listId = GetList(siteName, listName).id;

            string url = $"{GraphApiRootUrl}/sites/{siteId}/lists/{listId}/items/";

            try
            {
                HttpContent httpContent = new StringContent(jsonBody);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = api.PostAsync(url, httpContent).GetAwaiter().GetResult();
                CheckStatusCode(response, api.RetryResults);

                string content = response.Content.ReadAsStringAsync().Result;
                return content.JsonToObject<GetItemResponse.Item>();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// update a item, return the default fields, no customize fields, refer to https://learn.microsoft.com/en-us/graph/api/listitem-update
        /// </summary>
        /// <param name="siteName"></param>
        /// <param name="listName"></param>
        /// <param name="itemId"></param>
        /// <param name="jsonBody">json body with necessary columns in list</param>
        /// <returns></returns>
        public GetItemResponse.Fields UpdateItem(string siteName, string listName, string itemId, string jsonBody)
        {
            string siteId = _siteHelper.GetSite(siteName).id;
            string listId = GetList(siteName, listName).id;

            string url = $"{GraphApiRootUrl}/sites/{siteId}/lists/{listId}/items/{itemId}/fields";

            try
            {
                HttpContent httpContent = new StringContent(jsonBody);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = api.PatchAsync(url, httpContent).GetAwaiter().GetResult();
                CheckStatusCode(response, api.RetryResults);

                string content = response.Content.ReadAsStringAsync().Result;
                return content.JsonToObject<GetItemResponse.Fields>();
            }
            catch
            {
                throw;
            }
        }
    }
}
