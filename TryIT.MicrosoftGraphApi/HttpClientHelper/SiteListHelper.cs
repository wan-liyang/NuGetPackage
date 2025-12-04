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
        private readonly string _siteId;
        public SiteListHelper(MsGraphApiConfig config, string hostName, string siteName) : base(config) 
        {
            var _siteHelper = new SiteHelper(config);
            _siteId = _siteHelper.GetSite(siteName, hostName).id;
        }

        /// <summary>
        /// get all list under the site
        /// </summary>
        /// <returns></returns>
        public async Task<List<GetListResponse.SiteList>> GetAllListAsync()
        {
            string url = $"{GraphApiRootUrl}/sites/{_siteId}/lists";

            var response = await RestApi.GetAsync(url);
            CheckStatusCode(response, RestApi.RetryResults);

            string content = await response.Content.ReadAsStringAsync();
            return content.JsonToObject<GetListResponse.Response>().value;
        }

        /// <summary>
        /// get a specific list under a site
        /// </summary>
        /// <param name="listName"></param>
        /// <returns></returns>
        public async Task<GetListResponse.SiteList> GetListAsync(string listName)
        {
            string url = $"{GraphApiRootUrl}/sites/{_siteId}/lists?$filter={EscapeExpression($"DisplayName eq '{listName}'")}";

            var response = await RestApi.GetAsync(url);
            CheckStatusCode(response, RestApi.RetryResults);

            string content = await response.Content.ReadAsStringAsync();
            var lists = content.JsonToObject<GetListResponse.Response>().value;

            return lists.FirstOrDefault(p => p.displayName.IsEquals(listName));
        }

        /// <summary>
        /// get items with default columns, no other customize column
        /// </summary>
        /// <param name="listName"></param>
        /// <returns></returns>
        public async Task<List<GetItemResponse.Item>> GetItemsAsync(string listName)
        {
            var list = await GetListAsync(listName);

            string url = $"{GraphApiRootUrl}/sites/{_siteId}/lists/{list.id}/items?expand=fields";

            var response = await RestApi.GetAsync(url);
            CheckStatusCode(response, RestApi.RetryResults);

            string content = await response.Content.ReadAsStringAsync();
            return content.JsonToObject<GetItemResponse.Response>().value;
        }

        /// <summary>
        /// https://learn.microsoft.com/en-us/graph/api/listitem-list?view=graph-rest-1.0&tabs=http
        /// <para></para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listId"></param>
        /// <param name="expression">the expression to filter, e.g. title eq 'xxx'</param>
        /// <returns></returns>
        public async Task<List<T>> GetItemsAsync<T>(string listId, string expression) where T : class
        {
            // GET https://graph.microsoft.com/v1.0/sites/{site-id}/lists/{list-id}/items?$expand=fields($select=Name,Color,Quantity)

            var props = typeof(T).GetProperties();

            string fields = string.Join(",", props.Select(p => p.Name));

            string url = $"{GraphApiRootUrl}/sites/{_siteId}/lists/{listId}/items?expand=fields($select={fields})";

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
        /// <param name="listName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<T> GetItemByIdAsync<T>(string listName, string id)
        {
            var list = await GetListAsync(listName);

            string url = $"{GraphApiRootUrl}/sites/{_siteId}/lists/{list.id}/items/{id}";

            var response = await RestApi.GetAsync(url);
            CheckStatusCode(response, RestApi.RetryResults);

            string content = await response.Content.ReadAsStringAsync();
            return content.GetJsonValue<T>("fields");
        }

        /// <summary>
        /// create a item into list, refer to https://learn.microsoft.com/en-us/graph/api/listitem-create?view=graph-rest-1.0&tabs=http
        /// </summary>
        /// <param name="listName"></param>
        /// <param name="jsonBody">json body with necessary columns in list</param>
        /// <returns></returns>
        public async Task<GetItemResponse.Item> CreateItemAsync(string listName, string jsonBody)
        {
            var list = await GetListAsync(listName);

            string url = $"{GraphApiRootUrl}/sites/{_siteId}/lists/{list.id}/items/";

            HttpContent httpContent = new StringContent(jsonBody);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await RestApi.PostAsync(url, httpContent);
            CheckStatusCode(response, RestApi.RetryResults);

            string content = await response.Content.ReadAsStringAsync();
            return content.JsonToObject<GetItemResponse.Item>();
        }

        /// <summary>
        /// update a item, return the default fields, no customize fields, refer to https://learn.microsoft.com/en-us/graph/api/listitem-update
        /// </summary>
        /// <param name="listName"></param>
        /// <param name="itemId"></param>
        /// <param name="jsonBody">json body with necessary columns in list</param>
        /// <returns></returns>
        public async Task<GetItemResponse.Fields> UpdateItemAsync(string listName, string itemId, string jsonBody)
        {
            var list = await GetListAsync(listName);

            string url = $"{GraphApiRootUrl}/sites/{_siteId}/lists/{list.id}/items/{itemId}/fields";

            HttpContent httpContent = new StringContent(jsonBody);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await RestApi.PatchAsync(url, httpContent);
            CheckStatusCode(response, RestApi.RetryResults);

            string content = await response.Content.ReadAsStringAsync();
            return content.JsonToObject<GetItemResponse.Fields>();
        }
    }
}
