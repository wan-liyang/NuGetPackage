using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TryIT.MicrosoftGraphApi.Helper;
using TryIT.MicrosoftGraphApi.Model;
using TryIT.MicrosoftGraphApi.Model.Outlook;
using TryIT.MicrosoftGraphApi.MsGraphApi;

namespace NUnitTest02.TryIT_MicrosoftGraphApi
{
    internal class Outlook_UnitTest
    {

        OutlookApi api;

        [SetUp]
        public void Setup()
        {
            MsGraphApiConfig config = new MsGraphApiConfig
            {
                Token = "",
            };

           api = new OutlookApi(config);
        }

        [Test]
        public void Group_Test()
        {
        }
    }
}
