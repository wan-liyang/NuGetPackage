using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using TryIT.TableauApi.ApiResponse;

namespace TryIT.TableauApi
{
    public partial class TableauConnector
    {
        /// <summary>
        /// get all users for current site
        /// </summary>
        /// <returns></returns>
        public List<SiteModel.User> GetUsers()
        {
            List<SiteModel.User> users = new List<SiteModel.User>();

            string url = $"/api/{apiVersion}/sites/{siteId}/users";
            var responseMessage = httpClient.GetAsync(url).GetAwaiter().GetResult();
            CheckResponseStatus(responseMessage);

            var content = responseMessage.Content.ReadAsStringAsync().Result;
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
                    var pageUser = GetUsers(i, pageSize);
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
        private List<SiteModel.User> GetUsers(int pageNumber, int pageSize)
        {
            // GET /api/api-version/sites/site-id/users?pageSize=page-size&pageNumber=page-number

            string url = $"/api/{apiVersion}/sites/{siteId}/users?pageSize={pageSize}&pageNumber={pageNumber}";
            var responseMessage = httpClient.GetAsync(url).GetAwaiter().GetResult();
            CheckResponseStatus(responseMessage);

            var content = responseMessage.Content.ReadAsStringAsync().Result;
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
        public SiteModel.User GetUser(string username)
        {
            string url = $"/api/{apiVersion}/sites/{siteId}/users?filter=name:eq:{username}";
            var responseMessage = httpClient.GetAsync(url).GetAwaiter().GetResult();
            CheckResponseStatus(responseMessage);

            var content = responseMessage.Content.ReadAsStringAsync().Result;
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
        public SiteModel.User GetUserById(string userId)
        {
            string url = $"/api/{apiVersion}/sites/{siteId}/users/{userId}";
            var responseMessage = httpClient.GetAsync(url).GetAwaiter().GetResult();
            CheckResponseStatus(responseMessage);

            var content = responseMessage.Content.ReadAsStringAsync().Result;
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
        public SiteModel.User AddUserToSite(string username, string siterole)
        {
            string url = $"/api/{apiVersion}/sites/{siteId}/users";
            string request = $"<tsRequest><user name=\"{username}\" siteRole=\"{siterole}\"/></tsRequest>";
            StringContent requestContent = new StringContent(request, System.Text.Encoding.UTF8, "application/xml");
            var responseMessage = httpClient.PostAsync(url, requestContent).GetAwaiter().GetResult();
            CheckResponseStatus(responseMessage);

            var content = responseMessage.Content.ReadAsStringAsync().Result;
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
        public SiteModel.User UpdateUser(string userId, string displayName, string email, string siterole)
        {
            string url = $"/api/{apiVersion}/sites/{siteId}/users/{userId}";
            string request = $"<tsRequest><user fullName=\"{displayName}\" email=\"{email}\" siteRole=\"{siterole}\"/></tsRequest>";
            StringContent requestContent = new StringContent(request, System.Text.Encoding.UTF8, "application/xml");
            var responseMessage = httpClient.PutAsync(url, requestContent).GetAwaiter().GetResult();
            CheckResponseStatus(responseMessage);

            return GetUserById(userId);
        }

        /// <summary>
        /// delete an user from site, the content under this user will assign to <paramref name="mapAssetsToId"/>, if <paramref name="mapAssetsToId"/> is empty, will assign to current user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="mapAssetsToId"></param>
        public void DeleteUser(string userId, string mapAssetsToId)
        {
            if (string.IsNullOrEmpty(mapAssetsToId))
            {
                mapAssetsToId = myId;
            }
            string url = $"/api/{apiVersion}/sites/{siteId}/users/{userId}?mapAssetsTo={mapAssetsToId}";
            var responseMessage = httpClient.DeleteAsync(url).GetAwaiter().GetResult();
            CheckResponseStatus(responseMessage);
        }

        /// <summary>
        /// create group with name <paramref name="groupName"/> into site
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public SiteModel.Group CreateGroup(string groupName)
        {
            string url = $"/api/{apiVersion}/sites/{siteId}/groups";
            string request = $"<tsRequest><group name=\"{groupName}\" /></tsRequest>";
            StringContent requestContent = new StringContent(request, System.Text.Encoding.UTF8, "application/xml");
            var responseMessage = httpClient.PostAsync(url, requestContent).GetAwaiter().GetResult();
            CheckResponseStatus(responseMessage);
            return GetGroup(groupName);
        }

        /// <summary>
        /// get group by <paramref name="groupName"/>
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public SiteModel.Group GetGroup(string groupName)
        {
            groupName = groupName.Replace(" ", "+");

            string url = $"/api/{apiVersion}/sites/{siteId}/groups?filter=name:eq:{groupName}";
            var responseMessage = httpClient.GetAsync(url).GetAwaiter().GetResult();
            CheckResponseStatus(responseMessage);

            var content = responseMessage.Content.ReadAsStringAsync().Result;
            var result = content.JsonToObject<GetGroupResponse.Response>();
            if (result.groups.group == null)
            {
                return null;
            }
            return result.groups.group.First().ToGroup();
        }

        /// <summary>
        /// get all user from a group
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public List<SiteModel.User> GetGroupUser(string groupId)
        {
            List<SiteModel.User> users = new List<SiteModel.User>();

            string url = $"/api/{apiVersion}/sites/{siteId}/groups/{groupId}/users";
            var responseMessage = httpClient.GetAsync(url).GetAwaiter().GetResult();
            CheckResponseStatus(responseMessage);

            var content = responseMessage.Content.ReadAsStringAsync().Result;
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
                    var pageUser = GetGroupUser(groupId, i, pageSize);
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
        private List<SiteModel.User> GetGroupUser(string groupId, int pageNumber, int pageSize)
        {
            // /api/api-version/sites/site-id/groups/group-id/users?pageSize=page-size&pageNumber=page-number

            string url = $"/api/{apiVersion}/sites/{siteId}/groups/{groupId}/users?pageSize={pageSize}&pageNumber={pageNumber}";
            var responseMessage = httpClient.GetAsync(url).GetAwaiter().GetResult();
            CheckResponseStatus(responseMessage);

            var content = responseMessage.Content.ReadAsStringAsync().Result;
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
        public void AddUserToGroup(string groupId, string userId)
        {
            string url = $"/api/{apiVersion}/sites/{siteId}/groups/{groupId}/users";
            string request = $"<tsRequest><user id=\"{userId}\" /></tsRequest>";
            StringContent requestContent = new StringContent(request, System.Text.Encoding.UTF8, "application/xml");
            var responseMessage = httpClient.PostAsync(url, requestContent).GetAwaiter().GetResult();
            CheckResponseStatus(responseMessage);
        }

        /// <summary>
        /// remove an user from group
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="userId"></param>
        public void RemoveUserFromGroup(string groupId, string userId)
        {
            string url = $"/api/{apiVersion}/sites/{siteId}/groups/{groupId}/users/{userId}";
            var responseMessage = httpClient.DeleteAsync(url).GetAwaiter().GetResult();
            CheckResponseStatus(responseMessage);
        }
    }
}
