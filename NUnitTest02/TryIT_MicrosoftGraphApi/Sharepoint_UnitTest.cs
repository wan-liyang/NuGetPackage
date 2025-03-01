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
        public void Rename_Test()
        {
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

        [Test]
        public void CreateFolder()
        {
            SharepointApi api = new SharepointApi(config);

            string folderPath = @"a\b\c\d";

            string hostname = "";
            string sitename = "";
            SiteApi siteApi = new SiteApi(config, hostname);
            var site = siteApi.GetSite(sitename);
            var drive = siteApi.GetDrive(site.id);

            var item = api.GetItemByPath(drive.id, "");

            var response = api.CreateFolder(drive.id, item.id, folderPath);
        }

        [Test]
        public void CreateFolderStopInherit()
        {
            SharepointApi api = new SharepointApi(config);

            string hostname = "";
            string sitename = "";
            SiteApi siteApi = new SiteApi(config, hostname);
            var site = siteApi.GetSite(sitename);
            var drive = siteApi.GetDrive(site.id);

            string folderPath = @"";
            var item = api.GetItemByPath(drive.id, folderPath);

            string newFolderPath = @"";
            var response = api.CreateFolder(drive.id, item.id, newFolderPath, true);
        }

        [Test]
        public void DownloadFolder()
        {
            SharepointApi api = new SharepointApi(config);

            string hostname = "";
            string sitename = "";
            SiteApi siteApi = new SiteApi(config, hostname);
            var site = siteApi.GetSite(sitename);
            var drive = siteApi.GetDrive(site.id);

            var item = api.GetItemByPath(drive.id, @"sgfinance\Project_Office\013-CIS\01 - To Generate (GB)");

            var response = api.DownloadItems(drive.id, item.id, @"D:\01_NCS_Project\New folder");
        }

        [Test]
        public void GetFolder()
        {
            SharepointApi api = new SharepointApi(config);

            string url = "";


            var item = api.GetFolder(url);
        }

        [Test]
        public void GetChildFilter()
        {
            SharepointApi api = new SharepointApi(config);

            string driveId = "";
            string itemId = "";
            string filterExpress = "startsWith(name, 'abc')";
            api.GetChildren(driveId, itemId, filterExpress);

        }

        [Test]
        public void GetListItems()
        {
        }

        public class ListItem
        {
            public string Id { get; set; }
            public string Title { get; set; }
            public string Name { get; set; }
            public string EmployeeId { get; set; }
            public string CaseId { get; set; }
            public DateTime? CaseDate { get; set; }
        }
    }
}
