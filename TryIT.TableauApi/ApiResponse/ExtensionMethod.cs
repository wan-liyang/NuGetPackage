using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace TryIT.TableauApi.ApiResponse
{
    internal static class ExtensionMethod
    {
        #region Object <=> Json
        /// <summary>
        /// covnert Json string to specific object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static T JsonToObject<T>(this string jsonString)
        {
            return JsonConvert.DeserializeObject<T>(jsonString);
        }

        /// <summary>
        /// convert object to Json string
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ObjectToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
        #endregion

        public static SiteModel.User ToUser(this GetUsersResponse.User user)
        {
            if (user == null)
            {
                return null;
            }

            return new SiteModel.User
            {
                externalAuthUserId = user.externalAuthUserId,
                id = user.id,
                lastLogin = user.lastLogin,
                name = user.name,
                siteRole = user.siteRole,
                locale = user.locale,
                language = user.language
            };
        }
        
        public static SiteModel.User ToUser(this GetUserResponse.User user)
        {
            if (user == null)
            {
                return null;
            }

            return new SiteModel.User
            {
                externalAuthUserId = user.externalAuthUserId,
                id = user.id,
                lastLogin = user.lastLogin,
                name = user.name,
                siteRole = user.siteRole,
                locale = user.locale,
                language = user.language
            };
        }
        
        public static SiteModel.User ToUser(this AddUserResponse.User user)
        {
            if (user == null)
            {
                return null;
            }

            return new SiteModel.User
            {
                externalAuthUserId = user.externalAuthUserId,
                id = user.id,
                name = user.name,
                siteRole = user.siteRole
            };
        }

        public static SiteModel.User ToUser(this GetGroupUserResponse.User user)
        {
            if (user == null)
            {
                return null;
            }

            return new SiteModel.User
            {
                externalAuthUserId = user.externalAuthUserId,
                id = user.id,
                lastLogin = user.lastLogin,
                name = user.name,
                siteRole = user.siteRole,
                locale = user.locale,
                language = user.language
            };
        }

        public static SiteModel.Group ToGroup(this GetGroupResponse.Group group)
        {
            if (group == null)
            {
                return null;
            }

            return new SiteModel.Group
            {
                id = group.id,
                name = group.name
            };
        }

        public static SiteModel.Project ToProject(this GetProjectResponse.Project project)
        {
            if (project == null)
            {
                return null;
            }
            return new SiteModel.Project
            {
                ownerId = project.owner.id,
                id = project.id,
                name = project.name,
                description = project.description,
                createdAt = project.createdAt,
                updatedAt = project.updatedAt,
                contentPermissions = project.contentPermissions
            };
        }
        public static SiteModel.Project ToProject(this CreateProjectResponse.Project project)
        {
            if (project == null)
            {
                return null;
            }
            return new SiteModel.Project
            {
                ownerId = project.owner?.id,
                id = project.id,
                name = project.name,
                description = project.description,
                createdAt = project.createdAt,
                updatedAt = project.updatedAt,
                contentPermissions = project.contentPermissions
            };
        }

        public static SiteModel.Permission ToPermission(this GetProjectPermssionResponse.Permissions permissions)
        {
            if (permissions.project == null || permissions.granteeCapabilities == null)
            {
                return null;
            }

            var model = new SiteModel.Permission
            {
                project = new SiteModel.Permission.Project
                {
                    ownerId = permissions.project.owner.id,
                    id = permissions.project.id,
                    name = permissions.project.name
                },
                granteeCapabilities = permissions.granteeCapabilities?.Select(p => p.ToGranteeCapability()).ToList()
            };
            return model;
        }

        private static SiteModel.Permission.GranteeCapability ToGranteeCapability(this GetProjectPermssionResponse.GranteeCapability granteeCapability)
        {
            if (granteeCapability == null)
            {
                return null;
            }

            return new SiteModel.Permission.GranteeCapability
            {
                groupId = granteeCapability.group?.id,
                userId = granteeCapability.user?.id,
                capabilities = granteeCapability.capabilities.capability?.Select(p => p.ToCapability()).ToList()
            };
        }

        private static SiteModel.Permission.Capability ToCapability(this GetProjectPermssionResponse.Capability capability)
        {
            if (capability == null)
            {
                return null;
            }
            return new SiteModel.Permission.Capability
            {
                name = capability.name,
                mode = capability.mode
            };
        }
    }
}

