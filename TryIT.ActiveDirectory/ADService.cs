using TryIT.ActiveDirectory.Entities;
using TryIT.ActiveDirectory.Helper;
using TryIT.ActiveDirectory.Models;
using System.Collections.Generic;

namespace TryIT.ActiveDirectory
{
    /// <summary>
    /// service to operate with Active Directory
    /// </summary>
    public class ADService
    {
        /// <summary>
        /// User related service
        /// </summary>
        public class User
        {
            private static AdUser GetAdUserEntity(AdUserModel model)
            {
                AdUser user = null;

                if (model != null)
                {
                    user = new AdUser();

                    user.DistinguishedName = model.DistinguishedName;
                    user.DisplayName = model.DisplayName;
                    user.GivenName = model.GivenName;
                    user.LastName = model.LastName;
                    user.UserLogin = model.UserLogin;
                    user.EmployeeId = model.EmployeeId;
                    user.EmailAddress = model.EmailAddress;
                    user.AccountActive = model.AccountActive ? "Yes" : "No";

                    //if (model.Manager != null)
                    //{
                    //    user.Manager = new AdUserManager
                    //    {
                    //        DistinguishedName = model.Manager.DistinguishedName,
                    //        DisplayName = model.Manager.DisplayName,
                    //        UserLogin = model.Manager.UserLogin,
                    //        EmployeeId = model.Manager.EmployeeId,
                    //        EmailAddress = model.Manager.EmailAddress
                    //    };
                    //}

                    //if (model.MemberOf_Group != null && model.MemberOf_Group.Count > 0)
                    //{
                    //    user.MemberOf_Group = new List<AdUserGroup>();

                    //    model.MemberOf_Group.ForEach(p => {
                    //        user.MemberOf_Group.Add(new AdUserGroup
                    //        {
                    //            DistinguishedName = p.DistinguishedName,
                    //            Name = p.Name,
                    //            Description = p.Description,
                    //            Mail = p.Mail
                    //        });
                    //    });
                    //}

                    user.ExpirationDate = model.ExpirationDate;
                    user.AccountLockoutTime = model.AccountLockoutTime;
                    user.LastPasswordSet = model.LastPasswordSet;
                    user.PasswordExpiryOn = model.PasswordExpiryOn;
                    user.CreationDate = model.CreationDate;
                }

                return user;
            }

            /// <summary>
            /// find user by Employee Id
            /// </summary>
            /// <param name="employeeId"></param>
            /// <returns></returns>
            public static AdUser FindUserByEmployeeId(string employeeId)
            {
                AdUserHelper adUserHelper = new AdUserHelper();
                AdUserModel model = adUserHelper.FindUserByEmployeeId(employeeId);

                return GetAdUserEntity(model);
            }

            /// <summary>
            /// find user by Email Address
            /// </summary>
            /// <param name="emailAddress"></param>
            /// <returns></returns>
            public static AdUser FindUserByEmailAddress(string emailAddress)
            {
                AdUserHelper adUserHelper = new AdUserHelper();
                AdUserModel model = adUserHelper.FindUserByEmailAddress(emailAddress);

                return GetAdUserEntity(model);
            }

            /// <summary>
            /// find user by Email Address
            /// </summary>
            /// <param name="userLogin"></param>
            /// <returns></returns>
            public static AdUser FindUserByUserLogin(string userLogin)
            {
                AdUserHelper adUserHelper = new AdUserHelper();
                AdUserModel model = adUserHelper.FindUserByLogin(userLogin);

                return GetAdUserEntity(model);
            }

            /// <summary>
            /// find user by EmployeeId / UserLogin / EmailAddress
            /// </summary>
            /// <param name="keyword"></param>
            /// <returns></returns>
            public static AdUser FindUser(string keyword)
            {
                AdUserHelper adUserHelper = new AdUserHelper();
                AdUserModel model = adUserHelper.FindUserByKeyword(keyword);

                return GetAdUserEntity(model);
            }

            /// <summary>
            /// find user's manager
            /// </summary>
            /// <param name="userDistinguishedName"></param>
            /// <returns></returns>
            public static AdUserManager FindUserManager(string userDistinguishedName)
            {
                AdUserHelper adUserHelper = new AdUserHelper();
                AdUserModel model = adUserHelper.FindUserManager(userDistinguishedName);
                AdUserManager manager = null;

                if (model != null)
                {
                    manager = new AdUserManager();
                    manager.DistinguishedName = model.DistinguishedName;
                    manager.DisplayName = model.DisplayName;
                    manager.EmployeeId = model.EmployeeId;
                    manager.EmailAddress = model.EmailAddress;
                }
                return manager;
            }

            /// <summary>
            /// get groups user belong to
            /// </summary>
            /// <param name="userDistinguishedName"></param>
            /// <returns></returns>
            public static List<AdUserGroup> FindUserGroup(string userDistinguishedName)
            {
                AdUserHelper adUserHelper = new AdUserHelper();

                var userGroupInfo = adUserHelper.FindUserGroup(userDistinguishedName);

                List<AdUserGroup> groups = null;
                if (userGroupInfo != null)
                {
                    groups = new List<AdUserGroup>();

                    if (userGroupInfo.MemberOf_Group != null && userGroupInfo.MemberOf_Group.Count > 0)
                    {
                        userGroupInfo.MemberOf_Group.ForEach(p => {
                            groups.Add(new AdUserGroup
                            {
                                DistinguishedName = p.DistinguishedName,
                                Name = p.Name,
                                Description = p.Description,
                                Mail = p.Mail
                            });
                        });
                    }
                }

                return groups;
            }
        }
    }
}
