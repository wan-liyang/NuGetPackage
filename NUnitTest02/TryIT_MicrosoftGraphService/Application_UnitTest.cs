using System.Diagnostics;
using TryIT.MicrosoftGraphService.Model;
using TryIT.MicrosoftGraphService.MsGraphApi;

namespace NUnitTest02.TryIT_MicrosoftGraphService
{
    public class Application_UnitTest
	{
        [Test]
        public void Jwt_Test()
        {
            MsGraphApiConfig config = new MsGraphApiConfig
            {
                Token = "",
            };

            ApplicationApi applicationApi = new ApplicationApi(config);

            var app = applicationApi.GetApplication("ncsit-etl-uat-app");

            Debug.WriteLine(TryIT.ObjectExtension.ObjExtension.ObjectToJson(app));
        }
    }
}

