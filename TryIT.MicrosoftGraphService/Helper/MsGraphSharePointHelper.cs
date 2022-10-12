using TryIT.MicrosoftGraphService.Config;
using TryIT.MicrosoftGraphService.HttpClientHelper;
using TryIT.MicrosoftGraphService.Model;
using System.Collections.Generic;
using System.Linq;

namespace TryIT.MicrosoftGraphService.Helper
{
    public class MsGraphSharePointHelper
    {
        private static SharePointHelper _helper;
        public MsGraphSharePointHelper(MsGraphApiConfig config)
        {
            MsGraphHelper graphHelper = new MsGraphHelper(config);
            _helper = new SharePointHelper(graphHelper.GetHttpClient());
        }

        public SharePointModel.Site GetSite(string hostName, string siteName)
        {
            var site = _helper.GetSite(hostName, siteName);
            return site.ToSiteModule();
        }

        public SharePointModel.SiteDriveItemModel GetFolderByUrl(string absoluteUrl)
        {
            var folder = _helper.GetFolderByUrl(absoluteUrl);
            return folder.ToSiteDriveItem();
        }

        public List<SharePointModel.SiteDriveItemModel> CreateFolder(string hostName, string siteName, string parentFolderAbsUrl, string folderNameOrPath)
        {
            var site = this.GetSite(hostName, siteName);
            var parentFolder = this.GetFolderByUrl(parentFolderAbsUrl);

            List<SharePointModel.SiteDriveItemModel> listNewFolders = new List<SharePointModel.SiteDriveItemModel>();

            string[] folders = folderNameOrPath.Split('\\');

            for (int i = 0; i < folders.Length; i++)
            {
                SharePointModel.SiteDriveItemModel siteDriveItem = new SharePointModel.SiteDriveItemModel
                {
                    ParentDriveItem = new SharePointModel.SiteDriveItemModel
                    {
                        Id = parentFolder.Id,
                        Name = parentFolder.Name,
                    }
                };

                string folderName = folders[i];
                parentFolder = CreateFolder(site.Id, parentFolder.Id, folderName);

                siteDriveItem.Id = parentFolder.Id;
                siteDriveItem.Name = parentFolder.Name;

                listNewFolders.Add(siteDriveItem);
            }

            return listNewFolders;
        }

        private static SharePointModel.SiteDriveItemModel CreateFolder(string siteId, string parentFolderId, string folderName)
        {
            SharePointCreateFolderModel model = new SharePointCreateFolderModel
            {
                Name = folderName,
                ConflictBehavior = ConflictBehavior.replace.ToString(),
                Folder = new SharePointResponse.GetDriveItemResponse.Folder()
            };

            var item = _helper.CreateFolder(siteId, parentFolderId, model);
            return item.ToSiteDriveItem();
        }

        public SharePointModel.SiteDriveItemModel UploadFileByFolderUrl(string hostName, string siteName, string absoluteUrl, string fileName, byte[] fileContent)
        {
            var site = this.GetSite(hostName, siteName);
            var folder = this.GetFolderByUrl(absoluteUrl);

            SharePointModel.SiteUploadFileModel module = new SharePointModel.SiteUploadFileModel
            {
                FileName = fileName,
                FileContent = fileContent,
                SiteId = site.Id,
                DriveItemId = folder.Id
            };

            var item = _helper.UploadFile(module);
            return item.ToSiteDriveItem();
        }

        public SharePointModel.SiteDriveItemModel UploadFileByFolderId(string hostName, string siteName, string folderId, string fileName, byte[] fileContent)
        {
            var site = this.GetSite(hostName, siteName);

            SharePointModel.SiteUploadFileModel module = new SharePointModel.SiteUploadFileModel
            {
                FileName = fileName,
                FileContent = fileContent,
                SiteId = site.Id,
                DriveItemId = folderId
            };

            var item = _helper.UploadFile(module);
            return item.ToSiteDriveItem();
        }

        public SharePointModel.SiteDriveItemModel GetFileByName(string hostName, string siteName, string folderAbsUrl, string fileName)
        {
            var site = this.GetSite(hostName, siteName);
            var folder = this.GetFolderByUrl(folderAbsUrl);

            var file = _helper.GetFileByName(site.Id, folder.Id, fileName);
            return file.ToSiteDriveItem();
        }

        public SharePointModel.SiteDriveItemModel GetFileById(string hostName, string siteName, string fileId)
        {
            var site = this.GetSite(hostName, siteName);
            var file = _helper.GetItemById(site.Id, fileId);
            return file.ToSiteDriveItem();
        }

        /// <summary>
        /// get files from a absolute url of the folder (the browser display url after access inside the folder)
        /// </summary>
        /// <param name="folderAbsUrl"></param>
        /// <returns></returns>
        public List<SharePointModel.SiteDriveItemModel> GetFiles(string folderAbsUrl)
        {
            var files = _helper.GetItemsByUrl(folderAbsUrl);
            return files.Select(p => p.ToSiteDriveItem()).ToList();
        }

        public SharePointModel.SiteDriveItemPreviewModule CreateItemPreviewLink(string hostName, string siteName, string fileId)
        {
            var site = this.GetSite(hostName, siteName);
            var file = _helper.CreateItemPreviewLink(site.Id, fileId);
            return file.ToSiteDriveItemPreviewModule();
        }

        public bool DeleteFileById(string hostName, string siteName, string fileId, bool isDeleteFolderIfEmpty)
        {
            var site = this.GetSite(hostName, siteName);
            var file = _helper.GetItemById(site.Id, fileId);
            string folderId = file.parentReference.id;

            bool isSuccess = _helper.DeleteItemById(site.Id, fileId);

            if (isSuccess)
            {
                if (isDeleteFolderIfEmpty)
                {
                    var folder = _helper.GetItemById(site.Id, folderId);
                    if (folder != null && folder.folder != null && folder.folder.childCount == 0)
                    {
                        isSuccess = _helper.DeleteItemById(site.Id, folderId);
                    }
                }
            }
            return isSuccess;
        }

        public byte[] GetFileContentByFileId(string hostName, string siteName, string fileId)
        {
            var site = this.GetSite(hostName, siteName);
            byte[] _byte = _helper.GetFileContent(site.Id, fileId);

            return _byte;
        }
    }
}
