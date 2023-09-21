using TryIT.MicrosoftGraphApi.Helper;
using TryIT.MicrosoftGraphApi.HttpClientHelper;
using TryIT.MicrosoftGraphApi.Model;
using TryIT.MicrosoftGraphApi.Model.Token;
using TryIT.MicrosoftGraphApi.Response.Token;

namespace TryIT.MicrosoftGraphApi.MsGraphApi
{
    /// <summary>
    /// token api
    /// </summary>
    public class TokenApi
    {
        private AppTokenHelper _helper;

        /// <summary>
        /// init Token api with configuration, for obtain token, the <see cref="MsGraphApiConfig.Token"/> put 'NA'
        /// </summary>
        /// <param name="config"></param>
        public TokenApi(MsGraphApiConfig config)
        {
            MsGraphHelper graphHelper = new MsGraphHelper(config);
            _helper = new AppTokenHelper(graphHelper.GetHttpClient());
        }

        /// <summary>
        /// get access token
        /// </summary>
        /// <param name="tokenModel"></param>
        /// <returns></returns>
        public GetAppTokenResponse.Response GetToken(GetTokenModel tokenModel)
        {
            return _helper.GetToken(tokenModel);
        }
    }
}
