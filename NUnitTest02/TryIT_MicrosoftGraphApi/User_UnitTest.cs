﻿using System;
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
        [Test]
        public void Group_Test()
        {
            MsGraphApiConfig config = new MsGraphApiConfig
            {
                Token = "",
            };

            UserApi api = new UserApi(config);

            var result = api.GetUserByMail("liyang.wan2@ncs.co");
            var result2 = api.GetUserByAttribute("employeeId", "1207563");
            var result3 = api.GetUserByMailWithAdditionalAttribute("liyang.wan2@ncs.co", "employeeId");

            Assert.True(1 == 1);
        }
    }
}
