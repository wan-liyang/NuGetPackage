using System;
using System.Collections;
using System.Collections.Generic;
using TryIT.MicrosoftGraphApi.Helper;
using TryIT.MicrosoftGraphApi.HttpClientHelper;
using TryIT.MicrosoftGraphApi.Model;
using TryIT.MicrosoftGraphApi.Model.User;
using TryIT.MicrosoftGraphApi.Response.User;

namespace TryIT.MicrosoftGraphApi.MsGraphApi
{
    public class UserApi
    {
        private UserHelper _helper;

        /// <summary>
        /// init Teams api with configuration
        /// </summary>
        /// <param name="config"></param>
        public UserApi(MsGraphApiConfig config)
        {
            MsGraphHelper graphHelper = new MsGraphHelper(config);
            _helper = new UserHelper(graphHelper.GetHttpClient());
        }

        /// <summary>
        /// get user by user principal name 
        /// </summary>
        /// <returns></returns>
        public GetUserResponse.User GetMe()
        {
            return _helper.GetMe();
        }

        /// <summary>
        /// get user by email address (email may different with principal name)
        /// </summary>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        public GetUserResponse.User GetUserByMail(string userEmail)
        {
            return _helper.GetUserByMail(userEmail);
        }

        /// <summary>
        /// filter user with <paramref name="expression"/>, return list of <typeparamref name="T"/> which contains expected attributes, https://learn.microsoft.com/en-us/graph/aad-advanced-queries?tabs=http
        /// </summary>
        /// <typeparam name="T">expected response type contains expected attributes</typeparam>
        /// <param name="expression">the expression to filter, e.g. employeeId eq 'xxx'</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public List<T> FilterUser<T>(string expression) where T : class
        {
            return _helper.FilterUser<T>(expression);
        }

        /// <summary>
        /// get user by specific attribute name and value
        /// </summary>
        /// <param name="attrKey"></param>
        /// <param name="attrValue"></param>
        /// <returns></returns>
        public GetUserResponse.User GetUserByAttribute(string attrKey, string attrValue)
        {
            return _helper.GetUserByAttribute(attrKey, attrValue);
        }

        /// <summary>
        /// get user by mail, with additional attribute
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="additionalAttribute"></param>
        /// <returns></returns>
        public GetUserResponse.User GetUserByMailWithAdditionalAttribute(string userEmail, params string[] additionalAttribute)
        {
            return _helper.GetUserByMail(userEmail, additionalAttribute);
        }

        /// <summary>
        /// Use this API to create a new invitation. Invitation adds an external user to the organization
        /// </summary>
        /// <param name="invitationModel"></param>
        /// <returns></returns>
        public CreateInvitationResponse.Response CreateInvitation(CreateInvitationModel invitationModel)
        {
            return _helper.CreateInvitation(invitationModel);
        }

        /// <summary>
        /// Delete user.
        /// <para>When deleted, user resources are moved to a temporary container and can be restored within 30 days. After that time, they are permanently deleted. To learn more, see deletedItems.</para>
        /// <para>https://learn.microsoft.com/en-us/graph/api/user-delete</para>
        /// </summary>
        /// <param name="userEmail"></param>
        /// <returns>true: delete success, false: user not exists</returns>
        public bool DeleteUserByEmail(string userEmail)
        {
            return _helper.DeleteUserByEmail(userEmail);
        }

        /// <summary>
        /// https://learn.microsoft.com/en-us/graph/api/profilephoto-get?view=graph-rest-1.0&tabs=http
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public byte[] GetPhoth(string email)
        {
            return _helper.GetPhoto(email);
        }
    }
}