using System.Threading.Tasks;
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
            _helper = new AppTokenHelper(config);
        }

        /// <summary>
        /// get access token
        /// </summary>
        /// <param name="tokenModel"></param>
        /// <returns></returns>
        public async Task<GetAppTokenResponse.Response> GetTokenAsync(GetTokenModel tokenModel)
        {
            return await _helper.GetTokenAsync(tokenModel);
        }
    }
}
