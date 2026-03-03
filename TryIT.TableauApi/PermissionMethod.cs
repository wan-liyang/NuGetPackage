using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
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
        public async Task<SiteModel.Permission> GetProjectPermission(string projectId, DefaultPermissionTypeEnum defaultPermissionType)
        {
            string url = $"{_requestModel.HostUrl}/api/{_requestModel.ApiVersion}/sites/{siteId}/projects/{projectId}/permissions";
            if (!defaultPermissionType.Equals(DefaultPermissionTypeEnum.project))
            {
                url = $"{_requestModel.HostUrl}/api/{_requestModel.ApiVersion}/sites/{siteId}/projects/{projectId}/default-permissions/{defaultPermissionType}";
            }
            var responseMessage = await RestApiInstance.GetAsync(url);
            CheckResponseStatus(responseMessage);

            var content = await responseMessage.Content.ReadAsStringAsync();
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
        public async Task <SiteModel.Permission> AddProjectGroupPermission(string projectId, string groupId, DefaultPermissionTypeEnum defaultPermissionType, List<Capability> capabilities)
        {
            if (capabilities == null || capabilities.Count == 0)
            {
                throw new ArgumentNullException(nameof(capabilities), "no capability provided");
            }

            string url = $"{_requestModel.HostUrl} /api/ {_requestModel.ApiVersion}/sites/{siteId}/projects/{projectId}/permissions";
            if (!defaultPermissionType.Equals(DefaultPermissionTypeEnum.project))
            {
                url = $"{_requestModel.HostUrl}/api/{_requestModel.ApiVersion}/sites/{siteId}/projects/{projectId}/default-permissions/{defaultPermissionType}";
            }
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var item in capabilities)
            {
                stringBuilder.Append($"<capability name=\"{item.Type}\" mode=\"{item.Mode}\" />");
            }

            string request = $"<tsRequest><permissions><granteeCapabilities><group id=\"{groupId}\" /><capabilities>{stringBuilder.ToString()}</capabilities></granteeCapabilities></permissions></tsRequest>";
            StringContent requestContent = new StringContent(request, System.Text.Encoding.UTF8, "application/xml");
            var responseMessage = await RestApiInstance.PutAsync(url, requestContent);
            CheckResponseStatus(responseMessage);

            return await GetProjectPermission(projectId, defaultPermissionType);
        }

        /// <summary>
        /// delete permission for "All User" group
        /// </summary>
        /// <param name="projectId"></param>
        public async Task DeletePermissionForAllUserGroup(string projectId)
        {
            var group = await GetGroup("All Users");
            await DeleteProjectGroupPermission(projectId, group.id);
        }

        /// <summary>
        /// delete project permission by <paramref name="groupId"/>
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="groupId"></param>
        public async Task DeleteProjectGroupPermission(string projectId, string groupId)
        {
            // 1. delete project permission
            // 2. delete project default permission

            // DELETE /api/api-version/sites/site-id/projects/project-id/permissions/groups/group-id/capability-name/capability-mode
            var permission = await GetProjectPermission(projectId, DefaultPermissionTypeEnum.project);
            if (permission != null)
            {
                foreach (var item in permission.granteeCapabilities
                                    .Where(p => !string.IsNullOrEmpty(p.groupId)
                                            && p.groupId.Equals(groupId, StringComparison.CurrentCultureIgnoreCase)))
                {
                    foreach (var cap in item.capabilities)
                    {
                        string url = $"{_requestModel.HostUrl}/api/{_requestModel.ApiVersion}/sites/{siteId}/projects/{projectId}/permissions/groups/{groupId}/{cap.name}/{cap.mode}";
                        var responseMessage = await RestApiInstance.DeleteAsync(url);
                        CheckResponseStatus(responseMessage);
                    }
                }
            }

            // DELETE /api/api-version/sites/site-luid/projects/project-luid/default-permissions/workbooks/groups/group-luid/capability-name/capability-mode
            await DeleteDefaultPermission(projectId, groupId, null, DefaultPermissionTypeEnum.workbooks);
            await DeleteDefaultPermission(projectId, groupId, null, DefaultPermissionTypeEnum.datasources);
            await DeleteDefaultPermission(projectId, groupId, null, DefaultPermissionTypeEnum.dataroles);
            await DeleteDefaultPermission(projectId, groupId, null, DefaultPermissionTypeEnum.flows);
            await DeleteDefaultPermission(projectId, groupId, null, DefaultPermissionTypeEnum.lenses);
            await DeleteDefaultPermission(projectId, groupId, null, DefaultPermissionTypeEnum.metrics);
        }
        /// <summary>
        /// delete default permission by groupId or userId
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="groupId"></param>
        /// <param name="userId"></param>
        /// <param name="defaultPermissionType"></param>
        private async Task DeleteDefaultPermission(string projectId, string groupId, string userId, DefaultPermissionTypeEnum defaultPermissionType)
        {
            var permission = await GetProjectPermission(projectId, defaultPermissionType);
            if (permission != null)
            {
                foreach (var item in permission.granteeCapabilities.Where(
                    item => (!string.IsNullOrEmpty(item.groupId) && item.groupId.Equals(groupId, StringComparison.CurrentCultureIgnoreCase))
                            || (!string.IsNullOrEmpty(item.userId) && item.userId.Equals(userId, StringComparison.CurrentCultureIgnoreCase)
                    )))
                {
                    foreach (var cap in item.capabilities)
                    {
                        string url = $"{_requestModel.HostUrl}/api/{_requestModel.ApiVersion}/sites/{siteId}/projects/{projectId}/default-permissions/{defaultPermissionType}/groups/{groupId}/{cap.name}/{cap.mode}";
                        var responseMessage = await RestApiInstance.DeleteAsync(url);
                        CheckResponseStatus(responseMessage);
                    }
                }
            }
        }
    }
}
