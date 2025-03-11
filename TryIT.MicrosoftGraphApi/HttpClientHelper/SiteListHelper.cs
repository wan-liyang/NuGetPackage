using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TryIT.MicrosoftGraphApi.Helper;
using TryIT.MicrosoftGraphApi.Model;
using TryIT.MicrosoftGraphApi.Response.SiteList;

namespace TryIT.MicrosoftGraphApi.HttpClientHelper
{
    internal class SiteListHelper : BaseHelper
    {
        private readonly SiteHelper _siteHelper;

        public SiteListHelper(MsGraphApiConfig config, string hostName) : base(config) 
        {
            _siteHelper = new SiteHelper(config, hostName);
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

            var response = RestApi.GetAsync(url).GetAwaiter().GetResult();
            CheckStatusCode(response, RestApi.RetryResults);

            string content = response.Content.ReadAsStringAsync().Result;
            return content.JsonToObject<GetListResponse.Response>().value;
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

            var response = RestApi.GetAsync(url).GetAwaiter().GetResult();
            CheckStatusCode(response, RestApi.RetryResults);

            string content = response.Content.ReadAsStringAsync().Result;
            var lists = content.JsonToObject<GetListResponse.Response>().value;

            return lists.FirstOrDefault(p => p.displayName.IsEquals(listName));
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

            var response = RestApi.GetAsync(url).GetAwaiter().GetResult();
            CheckStatusCode(response, RestApi.RetryResults);

            string content = response.Content.ReadAsStringAsync().Result;
            return content.JsonToObject<GetItemResponse.Response>().value;
        }

        /// <summary>
        /// https://learn.microsoft.com/en-us/graph/api/listitem-list?view=graph-rest-1.0&tabs=http
        /// <para></para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="siteId"></param>
        /// <param name="listId"></param>
        /// <param name="expression">the expression to filter, e.g. title eq 'xxx'</param>
        /// <returns></returns>
        public async Task<List<T>> GetItemsAsync<T>(string siteId, string listId, string expression) where T : class
        {
            // GET https://graph.microsoft.com/v1.0/sites/{site-id}/lists/{list-id}/items?$expand=fields($select=Name,Color,Quantity)

            var props = typeof(T).GetProperties();

            string fields = string.Join(",", props.Select(p => p.Name));

            string url = $"{GraphApiRootUrl}/sites/{siteId}/lists/{listId}/items?expand=fields($select={fields})";

            if (!string.IsNullOrEmpty(expression))
            {
                url += $"&$filter={EscapeExpression(expression)}";

                AddDefaultRequestHeaders(this.HttpClient, "Prefer", "HonorNonIndexedQueriesWarningMayFailRandomly");
            }

            var response = await RestApi.GetAsync(url);
            CheckStatusCode(response, RestApi.RetryResults);

            string content = await response.Content.ReadAsStringAsync();

            var items = content.JsonToObject<GetItemResponse.Response>().value;

            if (items.Count == 0)
            {
                return null;
            }

            List<T> list = new List<T>();

            for (int i = 0; i < items.Count; i++)
            {
                var item = content.GetJsonValue<T>($"value[{i}]:fields");

                list.Add(item);
            }

            return list;
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

            var response = RestApi.GetAsync(url).GetAwaiter().GetResult();
            CheckStatusCode(response, RestApi.RetryResults);

            string content = response.Content.ReadAsStringAsync().Result;
            return content.GetJsonValue<T>("fields");
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

            HttpContent httpContent = new StringContent(jsonBody);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = RestApi.PostAsync(url, httpContent).GetAwaiter().GetResult();
            CheckStatusCode(response, RestApi.RetryResults);

            string content = response.Content.ReadAsStringAsync().Result;
            return content.JsonToObject<GetItemResponse.Item>();
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

            HttpContent httpContent = new StringContent(jsonBody);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = RestApi.PatchAsync(url, httpContent).GetAwaiter().GetResult();
            CheckStatusCode(response, RestApi.RetryResults);

            string content = response.Content.ReadAsStringAsync().Result;
            return content.JsonToObject<GetItemResponse.Fields>();
        }
    }
}
