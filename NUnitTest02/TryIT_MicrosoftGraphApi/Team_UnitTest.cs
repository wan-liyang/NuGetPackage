using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TryIT.MicrosoftGraphApi.Model;
using TryIT.MicrosoftGraphApi.Model.Team;
using TryIT.MicrosoftGraphApi.MsGraphApi;

namespace NUnitTest02.TryIT_MicrosoftGraphApi
{
    internal class Team_UnitTest
    {
        [Test]
        public void Test()
        {
            MsGraphApiConfig apiConfig = new MsGraphApiConfig
            {
                //Proxy_Url = AppConfig.ProxyUrl,
                Token = ""
            };

            TeamApi teamApi = new TeamApi(apiConfig, "O365-{teamName}-NCS");

            teamApi.CreateTeam(new CreateTeamModel { DisplayName = "Test", Description = "Test" });
        }
    }
}
