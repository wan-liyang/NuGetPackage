using NUnit.Framework;
using System;
using TryIT.TableauApi;

namespace NUnitTest02.TryIT_TableauApi
{
    class ServiceUtility_UnitTest
    {
        [Test]
        public async Task GetUser_Test()
        {
            using(TableauConnector connector = new TableauConnector(new TryIT.RestApi.Models.HttpClientConfig { }, GetRequestModel()))
            {
                await connector.SignIn();

                await connector.GetUsers();
            }
        }

        private TryIT.TableauApi.SiteModel.ApiRequestModel GetRequestModel()
        {
            return new TryIT.TableauApi.SiteModel.ApiRequestModel
            {
                HostUrl = "",
                ApiVersion = "",
                Sitename = "",
                TokenName = "",
                TokenSecret = "",
            };
        }
    }
}
