using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TryIT.RestApi;

namespace NUnitTest02.TryIT_RestApi
{
    internal class TryIT_RestApi_UnitTest
    {

        [Test]
        public void Test1()
        {
            var apiRequest = new RequestModel
            {
                Url = "http://localhost:18080"
            };


            ApiRequest api = new ApiRequest();
            var result = api.GetAsync(apiRequest).GetAwaiter().GetResult();

            var result2 = api.PostAsync(apiRequest).GetAwaiter().GetResult();

            api.RetryLog.Count();
        }

        [Test]
        public async Task Test2()
        {
            Api api = new Api(new ApiConfig
            {
                HttpClient = new HttpClient()
                {
                    Timeout = TimeSpan.FromSeconds(10)
                },
                EnableRetry = true
            });


            var response = await api.GetAsync("https://localhost:7279/weatherforecast");

            var a = response;
        }

        [Test]
        public async Task Test3()
        {
            //Api api = new Api(new HttpClientConfig
            //{
            //    TimeoutSecond = 10 * 60
            //});


            //var response = await api.GetAsync("https://localhost:7279/weatherforecast");

            //var a = response;
        }
    }
}
