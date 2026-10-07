using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TryIT.MicrosoftGraphApi.DatalakeApi;
using TryIT.MicrosoftGraphApi.Model;
using TryIT.MicrosoftGraphApi.MsGraphApi;

namespace NUnitTest02.TryIT_MicrosoftGraphApi
{
    internal class Datalake_UnitTest
    {
        MsGraphApiConfig config;

        [SetUp]
        public void Setup()
        {
            config = new MsGraphApiConfig
            {
                Token = "",
            };
        }

        [Test]
        public async Task Test()
        {
            DatalakeApi api = new DatalakeApi(new ApiConfig
            {
                TokenRequestInfo = new TryIT.MicrosoftGraphApi.Model.Token.GetTokenModel
                {
                    client_id = "",
                    client_secret = "",
                    grant_type = "client_credentials",
                    scope = "",
                    tenant_id = ""
                }
            });

            var response = await api.GetFileAsync(new TryIT.MicrosoftGraphApi.Model.Datalake.GetFileRequest
            {
                InstanceName = "",
                FilePath = ""
            });

            Assert.IsTrue(response != null);
        }
    }
}
