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
            string hostUrl = "";
            string apiVersion = "";
            string tokenName = "";
            string tokenSecret = "";
            string sitename = "";
            using(TableauConnector connector = new TableauConnector(hostUrl, apiVersion, tokenName, tokenSecret, sitename))
            {
                connector.GetAllUsers();
            }
        }
    }
}
