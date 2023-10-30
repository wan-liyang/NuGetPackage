using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using TryIT.TableauApi.ApiResponse;
using TryIT.TableauApi.Model;

namespace TryIT.TableauApi
{
    public partial class TableauConnector
    {
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
    }
}
