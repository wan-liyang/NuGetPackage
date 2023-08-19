using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TryIT.RestApi;

namespace NUnitTest02
{
    internal class TryIT_RestApi_UnitTest
    {

        [Test]
        public void Test1()
        {
            var apiRequest = new TryIT.RestApi.RequestModel
            {

            };

            TryIT.RestApi.ApiRequest.Post(apiRequest);

            ApiRequest.Post(apiRequest);
        }
    }
}
