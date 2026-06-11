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
            SiteApi siteApi = new SiteApi(config);
            var site = siteApi.GetSite(sitename, hostname);
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
            SiteApi siteApi = new SiteApi(config);
            var site = siteApi.GetSite(sitename, hostname);
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
            SiteApi siteApi = new SiteApi(config);
            var site = siteApi.GetSite(sitename, hostname);
            var drive = siteApi.GetDrive(site.id);

            var item = api.GetItemByPath(drive.id, @"");

            var response = api.DownloadItems(drive.id, item.id, @"");
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
        public void AddPermission()
        {
            SharepointApi api = new SharepointApi(config);

            string driveId = "";
            string itemId = "";

            api.AddPermissionsAsync(driveId, itemId, new TryIT.MicrosoftGraphApi.Request.Sharepoint.AddPermissionRequest.Body
            {
                recipients = new List<TryIT.MicrosoftGraphApi.Request.Sharepoint.AddPermissionRequest.Recipient>
                {
                    new TryIT.MicrosoftGraphApi.Request.Sharepoint.AddPermissionRequest.Recipient{ email = "" },
                    new TryIT.MicrosoftGraphApi.Request.Sharepoint.AddPermissionRequest.Recipient{ alias = "" }
                },
                roles = new List<TryIT.MicrosoftGraphApi.Request.Sharepoint.AddPermissionRequest.PermissionRole>
                {
                    TryIT.MicrosoftGraphApi.Request.Sharepoint.AddPermissionRequest.PermissionRole.read
                },
            });
        }

        [Test]
        public async Task CreateLinkTest()
        {
            SharepointApi api = new SharepointApi(config);
            string driveId = "";
            string itemId = "";
            var response = await api.CreateLinkAsync(driveId, itemId, new TryIT.MicrosoftGraphApi.Request.Sharepoint.CreateLinkRequest.Body { });

            Assert.IsNotEmpty(response.link.webUrl);
        }

        [Test]
        public async Task GetItemByUrl_Test()
        {
            SharepointApi api = new SharepointApi(config);

            var result = await api.GetItemByUrlAsync("https://xxx/_layouts/15/Doc.aspx?sourcedoc={7D3D8F1C-0AEE-476F-85A2-1BB49FD3D3A5}");

            var result2 = await api.GetItemByUrlAsync("https://xxx/Shared Documents/Forms/DispForm.aspx?ID=1234");

            Assert.That(result != null);
            Assert.That(result2 != null);
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
