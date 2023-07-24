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
        [Test]
        public void Group_Test()
        {
            MsGraphApiConfig config = new MsGraphApiConfig
            {
                Token = "",
            };

            OutlookApi api = new OutlookApi(config);

            api.SendMessage(new SendMessageModel
            {
                Subject = $"Test Email {DateTime.Now}",
                Body = "Test",
                BodyContentType = BodyContentType.Html,
                ToRecipients = "liyang.wan2@ncs.co".Split(','),
                CcRecipients = null,
                Attachments = null
            }); ;

            Assert.True(1 == 1);
        }
    }
}
