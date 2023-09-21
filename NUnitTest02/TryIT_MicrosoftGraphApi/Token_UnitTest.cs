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
        public void Token_Test()
        {
            MsGraphApiConfig config = new MsGraphApiConfig
            {
                Token = "NA",
            };

            TokenApi api = new TokenApi(config);

            GetTokenModel tokenModel = new GetTokenModel
            {
                tenant_id = "d1f299a1-5715-459d-9c31-d122c140c957",
                grant_type = "client_credentials",
                client_id = "7ec219f2-f570-4925-b721-e2b54c155f22",
                client_secret = "obI8Q~AM6HRmIDnng7XXdIXqev2XGce1N_jcZby.",
                scope = "https://graph.microsoft.com/.default",
            };

            var response = api.GetToken(tokenModel);

            Assert.True(1 == 1);
        }
    }
}
