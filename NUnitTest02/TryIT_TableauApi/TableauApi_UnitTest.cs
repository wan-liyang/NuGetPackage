using NUnit.Framework;
using System;
using TryIT.TableauApi;

namespace NUnitTest02.TryIT_TableauApi
{
    class ServiceUtility_UnitTest
    {
        [Test]
        public void GetUser_Test()
        {
            using(TableauConnector connector = new TableauConnector(GetRequestModel()))
            {
                connector.GetUsers();
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
                Proxy = new TryIT.TableauApi.SiteModel.WebProxyModel
                {
                    Url = ""
                }
            };
        }
    }
}
