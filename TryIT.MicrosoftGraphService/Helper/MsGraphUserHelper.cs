using TryIT.MicrosoftGraphService.HttpClientHelper;
using TryIT.MicrosoftGraphService.ApiModel;
using TryIT.MicrosoftGraphService.Model;

namespace TryIT.MicrosoftGraphService.Helper
{
    internal class MsGraphUserHelper
    {
        private static UserHelper _helper;
        public MsGraphUserHelper(MsGraphApiConfig config)
        {
            MsGraphHelper graphHelper = new MsGraphHelper(config);
            _helper = new UserHelper(graphHelper.GetHttpClient());
        }

        /// <summary>
        /// get user info, if <paramref name="userEmail"/> is empty, get me
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <returns></returns>
        public UserModel GetUserInfo(string emailAddress = "")
        {
            var user = _helper.GetUserInfo(emailAddress);

            return user.ToUserModule();
        }
    }
}
