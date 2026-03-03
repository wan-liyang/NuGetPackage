using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TryIT.TableauApi.ApiResponse;

namespace TryIT.TableauApi
{
    public partial class TableauConnector
    {
        /// <summary>
        /// get all users for current site
        /// </summary>
        /// <returns></returns>
        public async Task<List<SiteModel.User>> GetUsers()
        {
            List<SiteModel.User> users = new List<SiteModel.User>();

            string url = $"{_requestModel.HostUrl}/api/{_requestModel.ApiVersion}/sites/{siteId}/users";
            var responseMessage = await RestApiInstance.GetAsync(url);
            CheckResponseStatus(responseMessage);

            var content = await responseMessage.Content.ReadAsStringAsync();
            var result = content.JsonToObject<GetUsersResponse.Response>();
            if (result.users.user != null)
            {
                users.AddRange(result.users.user.Select(p => p.ToUser()).ToList());
            }

            int pageNumber = Convert.ToInt32(result.pagination.pageNumber);
            int totalAvailable = Convert.ToInt32(result.pagination.totalAvailable);
            int pageSize = Convert.ToInt32(result.pagination.pageSize);
            int totalPage = GetTotalPages(pageSize, totalAvailable);

            if (totalPage > pageNumber)
            {
                for (int i = pageNumber + 1; i <= totalPage; i++)
                {
                    var pageUser = await GetUsers(i, pageSize);
                    if (pageUser != null)
                    {
                        users.AddRange(pageUser);
                    }
                }
            }
            return users;
        }

        /// <summary>
        /// get users by pages
        /// </summary>
        /// <returns></returns>
        private async Task<List<SiteModel.User>> GetUsers(int pageNumber, int pageSize)
        {
            // GET /api/api-version/sites/site-id/users?pageSize=page-size&pageNumber=page-number

            string url = $"{_requestModel.HostUrl}/api/{_requestModel.ApiVersion}/sites/{siteId}/users?pageSize={pageSize}&pageNumber={pageNumber}";
            var responseMessage = await RestApiInstance.GetAsync(url);
            CheckResponseStatus(responseMessage);

            var content = await responseMessage.Content.ReadAsStringAsync();
            var result = content.JsonToObject<GetUsersResponse.Response>();
            if (result.users.user != null)
            {
                return result.users.user.Select(p => p.ToUser()).ToList();
            }
            return null;
        }

        /// <summary>
        /// find an user, return null if not found
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<SiteModel.User> GetUser(string username)
        {
            string url = $"{_requestModel.HostUrl}/api/{_requestModel.ApiVersion}/sites/{siteId}/users?filter=name:eq:{username}";
            var responseMessage = await RestApiInstance.GetAsync(url);
            CheckResponseStatus(responseMessage);

            var content = await responseMessage.Content.ReadAsStringAsync();
            var result = content.JsonToObject<GetUsersResponse.Response>();

            if (result.users.user != null)
            {
                return result.users.user.FirstOrDefault().ToUser();
            }
            return null;
        }

        /// <summary>
        /// get user based on assigned id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<SiteModel.User> GetUserById(string userId)
        {
            string url = $"{_requestModel.HostUrl}/api/{_requestModel.ApiVersion}/sites/{siteId}/users/{userId}";
            var responseMessage = await RestApiInstance.GetAsync(url);
            CheckResponseStatus(responseMessage);

            var content = await responseMessage.Content.ReadAsStringAsync();
            var result = content.JsonToObject<GetUserResponse.Response>();

            if (result.user != null)
            {
                return result.user.ToUser();
            }
            return null;
        }

        /// <summary>
        /// add new user to current site
        /// </summary>
        /// <param name="username"></param>
        /// <param name="siterole"></param>
        /// <returns></returns>
        public async Task<SiteModel.User> AddUserToSite(string username, string siterole)
        {
            string url = $"{_requestModel.HostUrl}/api/{_requestModel.ApiVersion}/sites/{siteId}/users";
            string request = $"<tsRequest><user name=\"{username}\" siteRole=\"{siterole}\"/></tsRequest>";
            StringContent requestContent = new StringContent(request, System.Text.Encoding.UTF8, "application/xml");
            var responseMessage = await RestApiInstance.PostAsync(url, requestContent);
            CheckResponseStatus(responseMessage);

            var content = await responseMessage.Content.ReadAsStringAsync();
            var result = content.JsonToObject<AddUserResponse.Response>();
            return result.user.ToUser();
        }

        /// <summary>
        /// update user displayName, email, siterole based on <paramref name="userId"/>
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="displayName"></param>
        /// <param name="email"></param>
        /// <param name="siterole"></param>
        /// <returns></returns>
        public async Task<SiteModel.User> UpdateUser(string userId, string displayName, string email, string siterole)
        {
            string url = $"{_requestModel.HostUrl}/api/{_requestModel.ApiVersion}/sites/{siteId}/users/{userId}";
            string request = $"<tsRequest><user fullName=\"{displayName}\" email=\"{email}\" siteRole=\"{siterole}\"/></tsRequest>";
            StringContent requestContent = new StringContent(request, System.Text.Encoding.UTF8, "application/xml");
            var responseMessage = await RestApiInstance.PutAsync(url, requestContent);
            CheckResponseStatus(responseMessage);

            return await GetUserById(userId);
        }

        /// <summary>
        /// delete an user from site, the content under this user will assign to <paramref name="mapAssetsToId"/>, if <paramref name="mapAssetsToId"/> is empty, will assign to current user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="mapAssetsToId"></param>
        public async Task DeleteUser(string userId, string mapAssetsToId)
        {
            if (string.IsNullOrEmpty(mapAssetsToId))
            {
                mapAssetsToId = myId;
            }
            string url = $"{_requestModel.HostUrl}/api/{_requestModel.ApiVersion}/sites/{siteId}/users/{userId}?mapAssetsTo={mapAssetsToId}";
            var responseMessage = await RestApiInstance.DeleteAsync(url);
            CheckResponseStatus(responseMessage);
        }

        /// <summary>
        /// create group with name <paramref name="groupName"/> into site
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public async Task<SiteModel.Group> CreateGroup(string groupName)
        {
            string url = $"{_requestModel.HostUrl}/api/{_requestModel.ApiVersion}/sites/{siteId}/groups";
            string request = $"<tsRequest><group name=\"{groupName}\" /></tsRequest>";
            StringContent requestContent = new StringContent(request, System.Text.Encoding.UTF8, "application/xml");
            var responseMessage = await RestApiInstance.PostAsync(url, requestContent);
            CheckResponseStatus(responseMessage);
            return await GetGroup(groupName);
        }

        /// <summary>
        /// get group by <paramref name="groupName"/>
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public async Task<SiteModel.Group> GetGroup(string groupName)
        {
            groupName = groupName.Replace(" ", "+");

            string url = $"{_requestModel.HostUrl}/api/{_requestModel.ApiVersion}/sites/{siteId}/groups?filter=name:eq:{groupName}";
            var responseMessage = await RestApiInstance.GetAsync(url);
            CheckResponseStatus(responseMessage);

            var content = await responseMessage.Content.ReadAsStringAsync();
            var result = content.JsonToObject<GetGroupResponse.Response>();
            if (result.groups.group == null)
            {
                return null;
            }
            return result.groups.group[0].ToGroup();
        }

        /// <summary>
        /// get all user from a group
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public async Task<List<SiteModel.User>> GetGroupUser(string groupId)
        {
            List<SiteModel.User> users = new List<SiteModel.User>();

            string url = $"{_requestModel.HostUrl}/api/{_requestModel.ApiVersion}/sites/{siteId}/groups/{groupId}/users";
            var responseMessage = await RestApiInstance.GetAsync(url);
            CheckResponseStatus(responseMessage);

            var content = await responseMessage.Content.ReadAsStringAsync();
            var result = content.JsonToObject<GetGroupUserResponse.Response>();
            if (result.users.user != null)
            {
                users.AddRange(result.users.user.Select(p => p.ToUser()).ToList());
            }

            int pageNumber = Convert.ToInt32(result.pagination.pageNumber);
            int totalAvailable = Convert.ToInt32(result.pagination.totalAvailable);
            int pageSize = Convert.ToInt32(result.pagination.pageSize);
            int totalPage = GetTotalPages(pageSize, totalAvailable);

            if (totalPage > pageNumber)
            {
                for (int i = pageNumber + 1; i <= totalPage; i++)
                {
                    var pageUser = await GetGroupUser(groupId, i, pageSize);
                    if (pageUser != null)
                    {
                        users.AddRange(pageUser);
                    }
                }
            }
            return users;
        }

        /// <summary>
        /// get user by page
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        private async Task<List<SiteModel.User>> GetGroupUser(string groupId, int pageNumber, int pageSize)
        {
            // /api/api-version/sites/site-id/groups/group-id/users?pageSize=page-size&pageNumber=page-number

            string url = $"{_requestModel.HostUrl}/api/{_requestModel.ApiVersion}/sites/{siteId}/groups/{groupId}/users?pageSize={pageSize}&pageNumber={pageNumber}";
            var responseMessage = await RestApiInstance.GetAsync(url);
            CheckResponseStatus(responseMessage);

            var content = await responseMessage.Content.ReadAsStringAsync();
            var result = content.JsonToObject<GetGroupUserResponse.Response>();
            if (result.users.user == null)
            {
                return null;
            }
            return result.users.user.Select(p => p.ToUser()).ToList();
        }

        /// <summary>
        /// add an user into group
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="userId"></param>
        public async Task AddUserToGroup(string groupId, string userId)
        {
            string url = $"{_requestModel.HostUrl}/api/{_requestModel.ApiVersion}/sites/{siteId}/groups/{groupId}/users";
            string request = $"<tsRequest><user id=\"{userId}\" /></tsRequest>";
            StringContent requestContent = new StringContent(request, System.Text.Encoding.UTF8, "application/xml");
            var responseMessage = await RestApiInstance.PostAsync(url, requestContent);
            CheckResponseStatus(responseMessage);
        }

        /// <summary>
        /// remove an user from group
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="userId"></param>
        public async Task RemoveUserFromGroup(string groupId, string userId)
        {
            string url = $"{_requestModel.HostUrl}/api/{_requestModel.ApiVersion}/sites/{siteId}/groups/{groupId}/users/{userId}";
            var responseMessage = await RestApiInstance.DeleteAsync(url);
            CheckResponseStatus(responseMessage);
        }
    }
}
