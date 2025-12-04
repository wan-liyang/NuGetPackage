using System.Collections.Generic;
using System.Threading.Tasks;
using TryIT.MicrosoftGraphApi.HttpClientHelper;
using TryIT.MicrosoftGraphApi.Model;
using TryIT.MicrosoftGraphApi.Response.SiteList;

namespace TryIT.MicrosoftGraphApi.MsGraphApi
{
    /// <summary>
    /// sharepoint list api, use to opoerate on sharepoint list
    /// </summary>
    public class SiteListApi
    {
        private readonly SiteListHelper _helper;

        /// <summary>
        /// init Teams api with configuration
        /// </summary>
        /// <param name="config">configuration for api request, e.g token, timeout, proxy etc</param>
        /// <param name="hostName">the host(domain) of the site, use for api request to get site under specific host</param>
        /// <param name="siteName">SharePoint site name</param>
        public SiteListApi(MsGraphApiConfig config, string hostName, string siteName)
        {
            _helper = new SiteListHelper(config, hostName, siteName);
        }

        /// <summary>
        /// get all list under a site
        /// </summary>
        /// <returns></returns>
        public async Task<List<GetListResponse.SiteList>> GetAllListAsync()
        {
            return await _helper.GetAllListAsync();
        }


        /// <summary>
        /// get a specific list under a site
        /// </summary>
        /// <param name="listName"></param>
        /// <returns></returns>
        public async Task<GetListResponse.SiteList> GetListAsync(string listName)
        {
            return await _helper.GetListAsync(listName);
        }

        /// <summary>
        /// get items with default columns, no other customize column
        /// </summary>
        /// <param name="listName"></param>
        /// <returns></returns>
        public async Task<List<GetItemResponse.Item>> GetItemsAsync(string listName)
        {
            return await _helper.GetItemsAsync(listName);
        }

        /// <summary>
        /// get items as a entity list from a sharepoint list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listId"></param>
        /// <param name="expression">the expression to filter, e.g. title eq 'xxx', give null if do not apply filter</param>
        /// <returns></returns>
        public async Task<List<T>> GetItemsAsync<T>(string listId, string expression) where T : class
        {
            return await _helper.GetItemsAsync<T>(listId, expression);
        }

        /// <summary>
        /// get specific item from list, convert result as expected object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<T> GetItem<T>(string listName, string id)
        {
            return await _helper.GetItemByIdAsync<T>(listName, id);
        }

        /// <summary>
        /// create a item into list, refer to https://learn.microsoft.com/en-us/graph/api/listitem-create?view=graph-rest-1.0&tabs=http
        /// </summary>
        /// <param name="listName"></param>
        /// <param name="jsonBody">json body with necessary columns in list</param>
        /// <returns></returns>
        public async Task<GetItemResponse.Item> CreateItemAsync(string listName, string jsonBody)
        {
            return await _helper.CreateItemAsync(listName, jsonBody);
        }

        /// <summary>
        /// update a item, refer to https://learn.microsoft.com/en-us/graph/api/listitem-update
        /// </summary>
        /// <param name="listName"></param>
        /// <param name="itemId"></param>
        /// <param name="jsonBody">json body with necessary columns in list</param>
        /// <returns></returns>
        public async Task<GetItemResponse.Fields> UpdateItemAsync(string listName, string itemId, string jsonBody)
        {
            return await _helper.UpdateItemAsync(listName, itemId, jsonBody);
        }
    }
}
