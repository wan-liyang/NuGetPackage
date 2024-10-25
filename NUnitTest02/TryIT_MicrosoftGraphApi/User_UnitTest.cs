using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TryIT.MicrosoftGraphApi.Model;
using TryIT.MicrosoftGraphApi.MsGraphApi;

namespace NUnitTest02.TryIT_MicrosoftGraphApi
{
    internal class User_UnitTest
    {
        public MsGraphApiConfig _config;
        [SetUp]
        public void Setup()
        {
            _config = new MsGraphApiConfig
            {
                Token = "",
            };
        }

        [Test]
        public void Group_Test()
        {
            UserApi api = new UserApi(_config);

            var result = api.GetUserByMail("liyang.wan2@ncs.co");
            var result2 = api.GetUserByAttribute("employeeId", "1207563");
            var result3 = api.GetUserByMailWithAdditionalAttribute("liyang.wan2@ncs.co", "employeeId");

            Assert.True(1 == 1);
        }

        [Test]
        public void FilterUser_Test()
        {
            UserApi api = new UserApi(_config);
            var result = api.FilterUser<User>("startsWith(employeeID, '135')");

            Assert.True(1 == 1);
        }

        [Test]
        public void FilterUser2_Test()
        {
            UserApi api = new UserApi(_config);
            //var result = api.FilterUser<User>("accountEnabled eq true and NOT(userType eq 'guest') and NOT(endsWith(userPrincipalName,'@groupncs.onmicrosoft.com') )");
            var result = api.FilterUser<User>("startsWith(employeeId,'1201')");

            Assert.True(1 == 1);
        }

        public class User
        {
            public string id { get; set; }
            public string displayName { get; set; }
            public bool accountEnabled { get; set; }
            public string userPrincipalName { get; set; }
            public string mail { get; set; }
        }
    }
}
