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
                Url = "http://localhost:18080"
            };


            ApiRequest api = new ApiRequest();
            var result = api.GetAsync(apiRequest).GetAwaiter().GetResult();

            var result2 = api.PostAsync(apiRequest).GetAwaiter().GetResult();

            api.RetryLog.Count();
        }
    }
}
