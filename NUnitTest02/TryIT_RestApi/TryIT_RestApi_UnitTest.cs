using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TryIT.RestApi;
using TryIT.RestApi.Models;

namespace NUnitTest02.TryIT_RestApi
{
    internal class TryIT_RestApi_UnitTest
    {

        [Test]
        public async Task Test1()
        {
            TryIT.RestApi.Api api = new Api(new HttpClientConfig
            {
                
            });

            var response = await api.GetAsync("https://www.baidu.com");
        }
    }
}
