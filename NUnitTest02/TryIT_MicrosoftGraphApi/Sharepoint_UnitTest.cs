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
    internal class Sharepoint_UnitTest
    {
        [Test]
        public void Rename_Test()
        {
            MsGraphApiConfig config = new MsGraphApiConfig
            {
                Token = "",
            };

            SharepointApi api = new SharepointApi(config);

            string folderUrl = "";

            var items = api.GetChildren(folderUrl);

            var items2 = api.GetFiles(folderUrl);

            string oldName = "";
            string newName = "";

            //var item = api.RenameItem(folderUrl, oldName, newName);
            //Assert.True(item.name == newName);

            //item = api.RenameItem(folderUrl, newName, oldName);
            //Assert.True(item.name == oldName);
        }
    }
}
