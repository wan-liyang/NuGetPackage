using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            api.SendMessage(new SendMessageModel
            {
                From = "noreply@noreply.com",
                Subject = $"Test Email {DateTime.Now}",
                Body = "Test",
                BodyContentType = BodyContentType.Html,
                ToRecipients = "".Split(','),
                CcRecipients = null,
                Attachments = null
            });

            Assert.True(1 == 1);
        }

        [Test]
        public void Group_Test2()
        {
            var result = api.GetMessages(new GetMessageModel
            {

            });

            Assert.True(1 == 1);
        }
    }
}
