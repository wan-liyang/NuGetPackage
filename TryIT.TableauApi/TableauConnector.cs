using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using TryIT.TableauApi.ApiResponse;
using TryIT.TableauApi.Model;
using TryIT.TableauApi.SiteModel;

namespace TryIT.TableauApi
{
    /// <summary>
    /// Tableau Connector
    /// </summary>
    public class TableauConnector : IDisposable
    {
        string siteId;
        string myId;
        string apiVersion;

        HttpClient httpClient;

        private static WebProxy GetProxy(WebProxyModel proxyModel)
        {
            WebProxy proxy = null;

            if (proxyModel != null && !string.IsNullOrEmpty(proxyModel.Url))
            {
                proxy = new WebProxy(proxyModel.Url);

                if (!string.IsNullOrEmpty(proxyModel.Username))
                {
                    proxy.UseDefaultCredentials = false;
                    proxy.Credentials = new NetworkCredential(proxyModel.Username, proxyModel.Password);
                }
                proxy.BypassProxyOnLocal = true;
            }

            return proxy;
        }

        private static void ValidateInfo(ApiRequestModel requestModel)
        {
            if (requestModel == null)
            {
                throw new ArgumentNullException(nameof(requestModel));
            }
            if (string.IsNullOrEmpty(requestModel.HostUrl))
            {
                throw new ArgumentNullException(nameof(requestModel.HostUrl));
            }
            if (string.IsNullOrEmpty(requestModel.Sitename))
            {
                throw new ArgumentNullException(nameof(requestModel.Sitename));
            }
            if (string.IsNullOrEmpty(requestModel.ApiVersion))
            {
                throw new ArgumentNullException(nameof(requestModel.ApiVersion));
            }
            if (string.IsNullOrEmpty(requestModel.TokenName))
            {
                throw new ArgumentNullException(nameof(requestModel.TokenName));
            }
            if (string.IsNullOrEmpty(requestModel.TokenSecret))
            {
                throw new ArgumentNullException(nameof(requestModel.TokenSecret));
            }
        }

        /// <summary>
        /// init Tableau Connector
        /// </summary>
        /// <param name="requestModel"></param>
        public TableauConnector(ApiRequestModel requestModel)
        {
            ValidateInfo(requestModel);

            this.apiVersion = requestModel.ApiVersion;

            WebProxy proxy = GetProxy(requestModel.Proxy);
            HttpClientHandler clientHandler = new HttpClientHandler()
            {
                AllowAutoRedirect = true,
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
            };
            if (proxy != null)
            {
                clientHandler.Proxy = proxy;
            }

            httpClient = new HttpClient(clientHandler);
            httpClient.BaseAddress = new Uri(requestModel.HostUrl);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            string url = $"/api/{apiVersion}/auth/signin";
            string request = $"<tsRequest><credentials personalAccessTokenName=\"{requestModel.TokenName}\" personalAccessTokenSecret=\"{requestModel.TokenSecret}\"><site contentUrl=\"{requestModel.Sitename}\"/></credentials></tsRequest>";
            StringContent requestContent = new StringContent(request, System.Text.Encoding.UTF8, "application/xml");
            var responseMessage = httpClient.PostAsync(url, requestContent).GetAwaiter().GetResult();
            CheckResponseStatus(responseMessage);

            var content = responseMessage.Content.ReadAsStringAsync().Result;
            var signinResponse = content.JsonToObject<SigninResponse.Response>();
            this.siteId = signinResponse.credentials.site.id;
            this.myId = signinResponse.credentials.user.id;

            // add the token into httpClient
            string token = signinResponse.credentials.token;
            httpClient.DefaultRequestHeaders.Remove("X-Tableau-Auth");
            httpClient.DefaultRequestHeaders.Add("X-Tableau-Auth", token);
        }

        #region User and Group method
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
                for(int i = pageNumber + 1; i <= totalPage; i++)
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
                for(int i = pageNumber + 1; i <= totalPage; i++)
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
        #endregion

        #region Project method
        /// <summary>
        /// get project by name and parent project id, if <paramref name="parentProjectId"/> is null or empty, means get top level project
        /// </summary>
        /// <param name="parentProjectId"></param>
        /// <param name="projectName">values are case sensitive, refer to https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_concepts_filtering_and_sorting.htm#filter-expression-notes</param>
        /// <returns></returns>
        public SiteModel.Project GetProject(string parentProjectId, string projectName)
        {
            projectName = projectName.Replace(" ", "+");
            string url = string.Empty;
            if (string.IsNullOrEmpty(parentProjectId))
            {
                url = $"/api/{apiVersion}/sites/{siteId}/projects?filter=topLevelProject:eq:true,name:eq:{projectName}";
            }
            else
            {
                url = $"/api/{apiVersion}/sites/{siteId}/projects?filter=parentProjectId:eq:{parentProjectId},name:eq:{projectName}";
            }
            var responseMessage = httpClient.GetAsync(url).GetAwaiter().GetResult();
            CheckResponseStatus(responseMessage);

            var content = responseMessage.Content.ReadAsStringAsync().Result;
            var result = content.JsonToObject<GetProjectResponse.Response>();
            if (result.projects.project == null)
            {
                return null;
            }
            return result.projects.project.First().ToProject();
        }

        /// <summary>
        /// create a project under <paramref name="parentProjectId"/>, if <paramref name="parentProjectId"/> is null or empty, will create as top level project
        /// </summary>
        /// <param name="parentProjectId"></param>
        /// <param name="projectName"></param>
        /// <param name="projectDescription"></param>
        /// <param name="projectContentPermission">This value controls user permissions in a project, default is ManagedByOwner</param>
        /// <returns></returns>
        public SiteModel.Project CreateProject(string parentProjectId, string projectName, string projectDescription, ProjectContentPermission projectContentPermission = ProjectContentPermission.ManagedByOwner)
        {
            string url = $"/api/{apiVersion}/sites/{siteId}/projects";
            string request = $"<tsRequest><project parentProjectId=\"{parentProjectId}\" name=\"{projectName}\" description=\"{projectDescription}\" contentPermissions=\"{projectContentPermission}\" /></tsRequest>";
            StringContent requestContent = new StringContent(request, System.Text.Encoding.UTF8, "application/xml");
            var responseMessage = httpClient.PostAsync(url, requestContent).GetAwaiter().GetResult();
            CheckResponseStatus(responseMessage);

            var content = responseMessage.Content.ReadAsStringAsync().Result;
            var result = content.JsonToObject<CreateProjectResponse.Response>();

            if (result.project == null)
            {
                return null;
            }

            return result.project.ToProject();
        }

        /// <summary>
        /// Updates the name, description, or project hierarchy of the specified project. You can create or update project hierarchies by specifying a parent project for the project that you are updating using this method.
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="newParentProjectId"></param>
        /// <param name="newProjectName"></param>
        /// <param name="newProjectDescription"></param>
        /// <param name="newProjectContentPermission"></param>
        /// <returns></returns>
        public SiteModel.Project UpdateProject(string projectId, string newParentProjectId, string newProjectName, string newProjectDescription, ProjectContentPermission newProjectContentPermission = ProjectContentPermission.ManagedByOwner)
        {
            string url = $"/api/{apiVersion}/sites/{siteId}/projects/{projectId}";
            string request = $"<tsRequest><project parentProjectId=\"{newParentProjectId}\" name=\"{newProjectName}\" description=\"{newProjectDescription}\" contentPermissions=\"{newProjectContentPermission}\" /></tsRequest>";
            StringContent requestContent = new StringContent(request, System.Text.Encoding.UTF8, "application/xml");
            var responseMessage = httpClient.PutAsync(url, requestContent).GetAwaiter().GetResult();
            CheckResponseStatus(responseMessage);

            var content = responseMessage.Content.ReadAsStringAsync().Result;
            var result = content.JsonToObject<CreateProjectResponse.Response>();

            if (result.project == null)
            {
                return null;
            }

            return result.project.ToProject();
        }
        #endregion

        #region Permission method

        /// <summary>
        /// get project permission, or other default permission
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="defaultPermissionType">determine get project perssion, or other defualt permission (e.g. Workbooks, Datasources, etc)</param>
        /// <returns></returns>
        public SiteModel.Permission GetProjectPermission(string projectId, DefaultPermissionTypeEnum defaultPermissionType)
        {
            string url = $"/api/{apiVersion}/sites/{siteId}/projects/{projectId}/permissions";
            if (!defaultPermissionType.Equals(DefaultPermissionTypeEnum.project))
            {
                url = $"/api/{apiVersion}/sites/{siteId}/projects/{projectId}/default-permissions/{defaultPermissionType}";
            }
            var responseMessage = httpClient.GetAsync(url).GetAwaiter().GetResult();
            CheckResponseStatus(responseMessage);

            var content = responseMessage.Content.ReadAsStringAsync().Result;
            var result = content.JsonToObject<GetProjectPermssionResponse.Response>();

            return result.permissions.ToPermission();
        }

        /// <summary>
        /// add project permission for group
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="groupId"></param>
        /// <param name="defaultPermissionType"></param>
        /// <param name="capabilities"></param>
        /// <returns></returns>
        public SiteModel.Permission AddProjectGroupPermission(string projectId, string groupId, DefaultPermissionTypeEnum defaultPermissionType, List<Capability> capabilities)
        {
            if (capabilities == null || capabilities.Count == 0)
            {
                throw new ArgumentNullException(nameof(capabilities), "no capability provided");
            }

            string url = $"/api/{apiVersion}/sites/{siteId}/projects/{projectId}/permissions";
            if (!defaultPermissionType.Equals(DefaultPermissionTypeEnum.project))
            {
                url = $"/api/{apiVersion}/sites/{siteId}/projects/{projectId}/default-permissions/{defaultPermissionType}";
            }
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var item in capabilities)
            {
                stringBuilder.Append($"<capability name=\"{item.Type}\" mode=\"{item.Mode}\" />");
            }

            string request = $"<tsRequest><permissions><granteeCapabilities><group id=\"{groupId}\" /><capabilities>{stringBuilder.ToString()}</capabilities></granteeCapabilities></permissions></tsRequest>";
            StringContent requestContent = new StringContent(request, System.Text.Encoding.UTF8, "application/xml");
            var responseMessage = httpClient.PutAsync(url, requestContent).GetAwaiter().GetResult();
            CheckResponseStatus(responseMessage);

            return GetProjectPermission(projectId, defaultPermissionType);
        }

        /// <summary>
        /// delete permission for "All User" group
        /// </summary>
        /// <param name="token"></param>
        /// <param name="siteId"></param>
        /// <param name="projectId"></param>
        public void DeletePermissionForAllUserGroup(string projectId)
        {
            var group = GetGroup("All Users");
            DeleteProjectGroupPermission(projectId, group.id);
        }

        /// <summary>
        /// delete project permission by <paramref name="groupId"/>
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="groupId"></param>
        public void DeleteProjectGroupPermission(string projectId, string groupId)
        {
            // 1. delete project permission
            // 2. delete project default permission

            // DELETE /api/api-version/sites/site-id/projects/project-id/permissions/groups/group-id/capability-name/capability-mode
            {
                var permission = GetProjectPermission(projectId, DefaultPermissionTypeEnum.project);

                if (permission != null)
                {
                    foreach (var item in permission.granteeCapabilities)
                    {
                        if (!string.IsNullOrEmpty(item.groupId) && item.groupId.Equals(groupId, StringComparison.CurrentCultureIgnoreCase))
                        {
                            foreach (var cap in item.capabilities)
                            {
                                string url = $"/api/{apiVersion}/sites/{siteId}/projects/{projectId}/permissions/groups/{groupId}/{cap.name}/{cap.mode}";
                                var responseMessage = httpClient.DeleteAsync(url).GetAwaiter().GetResult();
                                CheckResponseStatus(responseMessage);
                            }
                        }
                    }
                }
            }

            // DELETE /api/api-version/sites/site-luid/projects/project-luid/default-permissions/workbooks/groups/group-luid/capability-name/capability-mode
            {
                DeleteDefaultPermission(projectId, groupId, null, DefaultPermissionTypeEnum.workbooks);
                DeleteDefaultPermission(projectId, groupId, null, DefaultPermissionTypeEnum.datasources);
                DeleteDefaultPermission(projectId, groupId, null, DefaultPermissionTypeEnum.dataroles);
                DeleteDefaultPermission(projectId, groupId, null, DefaultPermissionTypeEnum.flows);
                DeleteDefaultPermission(projectId, groupId, null, DefaultPermissionTypeEnum.lenses);
                DeleteDefaultPermission(projectId, groupId, null, DefaultPermissionTypeEnum.metrics);
            }
        }
        /// <summary>
        /// delete default permission by groupId or userId
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="groupId"></param>
        /// <param name="userId"></param>
        /// <param name="defaultPermissionType"></param>
        private void DeleteDefaultPermission(string projectId, string groupId, string userId, DefaultPermissionTypeEnum defaultPermissionType)
        {
            var permission = GetProjectPermission(projectId, defaultPermissionType);
            if (permission != null)
            {
                foreach (var item in permission.granteeCapabilities)
                {
                    if (!string.IsNullOrEmpty(item.groupId) && item.groupId.Equals(groupId, StringComparison.CurrentCultureIgnoreCase)
                        || !string.IsNullOrEmpty(item.userId) && item.userId.Equals(userId, StringComparison.CurrentCultureIgnoreCase))
                    {
                        foreach (var cap in item.capabilities)
                        {
                            string url = $"/api/{apiVersion}/sites/{siteId}/projects/{projectId}/default-permissions/{defaultPermissionType}/groups/{groupId}/{cap.name}/{cap.mode}";
                            var responseMessage = httpClient.DeleteAsync(url).GetAwaiter().GetResult();
                            CheckResponseStatus(responseMessage);
                        }
                    }
                }
            }
        }
        #endregion

        public void Dispose()
        {
            siteId = null;
            myId = null;
            httpClient = null;
        }

        /// <summary>
        /// check API response status, if failed, throw exception
        /// </summary>
        /// <param name="responseMessage"></param>
        /// <exception cref="Exception"></exception>
        private void CheckResponseStatus(HttpResponseMessage responseMessage)
        {
            if (!responseMessage.IsSuccessStatusCode)
            {
                throw new Exception($"operation failed: {responseMessage.Content.ReadAsStringAsync().Result}");
            }
        }

        /// <summary>
        /// get total pages based on PageSize and TotalAvailable items
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="totalAvailable"></param>
        private int GetTotalPages(int pageSize, int totalAvailable)
        {
            int pages = totalAvailable / pageSize;

            if (totalAvailable % pageSize > 0)
            {
                pages++;
            }

            return pages;
        }
    }
}

