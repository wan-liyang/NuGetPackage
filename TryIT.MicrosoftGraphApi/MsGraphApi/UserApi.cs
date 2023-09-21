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
    }
}