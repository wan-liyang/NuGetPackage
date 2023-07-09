using TryIT.MicrosoftGraphService.ApiModel.User;
using TryIT.MicrosoftGraphService.Helper;
using TryIT.MicrosoftGraphService.HttpClientHelper;
using TryIT.MicrosoftGraphService.Model;
using TryIT.MicrosoftGraphService.Model.User;

namespace TryIT.MicrosoftGraphService.MsGraphApi
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
        /// get user by email address
        /// </summary>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        public UserModel GetUser(string userEmail)
        {
            var response = _helper.GetUserInfo(userEmail);

            return response.ToUserModel();
        }

        /// <summary>
        /// get user by employeeId
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        public EmployeeModel GetEmployee(string employeeId)
        {
            var response = _helper.GetUserByEmployeeId(employeeId);

            return response.ToEmployeeModel();
        }
    }
}