using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TryIT.MicrosoftGraphApi.Model;
using TryIT.MicrosoftGraphApi.Model.Token;
using TryIT.MicrosoftGraphApi.MsGraphApi;

namespace NUnitTest02.TryIT_MicrosoftGraphApi
{
    internal class Token_UnitTest
    {
        [Test]
        public async Task Token_Test()
        {
            MsGraphApiConfig config = new MsGraphApiConfig
            {
                Token = "NA",
            };

            TokenApi api = new TokenApi(config);

            GetTokenModel tokenModel = new GetTokenModel
            {
                tenant_id = "",
                grant_type = "client_credentials",
                client_id = "",
                client_secret = "",
                scope = "https://graph.microsoft.com/.default",
            };

            var response = await api.GetTokenAsync(tokenModel);

            Assert.True(1 == 1);
        }
    }
}
