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
        public void Test()
        {
            TeamApi teamApi = new TeamApi(config, "");

            teamApi.CreateTeam(new CreateTeamModel { DisplayName = "Test", Description = "Test" });
        }

        [Test]
        public void Test2()
        {
            TeamApi teamApi = new TeamApi(config);

            var list = teamApi.GetMembers("");
        }
    }
}
