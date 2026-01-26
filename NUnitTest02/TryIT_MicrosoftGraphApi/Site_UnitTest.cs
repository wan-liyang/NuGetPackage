using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TryIT.MicrosoftGraphApi.Model;
using TryIT.MicrosoftGraphApi.Model.Token;
using TryIT.MicrosoftGraphApi.MsGraphApi;

namespace NUnitTest02.TryIT_MicrosoftGraphApi
{
    internal class Site_UnitTest
    {
        [Test]
        public async Task Test()
        {
            MsGraphApiConfig config = new MsGraphApiConfig
            {
                Token = "",
            };

            SiteApi api = new SiteApi(config);

            var site = await api.CreateSiteAsync(new TryIT.MicrosoftGraphApi.Request.Site.CreateSiteRequest.Request
            {
                displayName = "",
                description = "",
            });

            Assert.True(1 == 1);
        }
        [Test]
        public async Task GetSiteListItemTest()
        {
            MsGraphApiConfig config = new MsGraphApiConfig
            {
                Token = "",
            };
            SiteListApi api = new SiteListApi(config, "", "");
            var items = await api.GetItemsAsync("");
            Assert.True(items != null && items.Count > 0);
        }

        [Test]
        public async Task GetSiteListItemAsObjectTest()
        {
            MsGraphApiConfig config = new MsGraphApiConfig
            {
                Token = "",
            };
            SiteListApi api = new SiteListApi(config, "", "");
            var items = await api.GetItemsAsync<ItemObject>("", "");
            Assert.True(items != null && items.Count > 0);
        }

        class ItemObject
        {
            public string Title { get; set; }
        }
    }
}
