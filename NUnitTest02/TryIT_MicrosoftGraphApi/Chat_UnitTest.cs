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
    internal class Chat_UnitTest
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
            ChatApi api = new ChatApi(config);

            var response = await api.SendMessageAsync(
                "abc@abc.com",
                new TryIT.MicrosoftGraphApi.HttpModel.Chat.SendMessageRequest.Request
                {
                    body = new TryIT.MicrosoftGraphApi.HttpModel.Chat.SendMessageRequest.Body
                    {
                        content = "Hello from UnitTest",
                    }
                });

            Assert.IsTrue(response.id != null);
        }
    }
}
