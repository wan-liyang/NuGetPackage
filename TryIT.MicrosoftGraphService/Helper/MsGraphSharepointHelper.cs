using TryIT.MicrosoftGraphService.HttpClientHelper;
using TryIT.MicrosoftGraphService.ApiModel;
using TryIT.MicrosoftGraphService.Model;
using System.Collections.Generic;
using System.Linq;

namespace TryIT.MicrosoftGraphService.Helper
{
    internal class MsGraphSharePointHelper
    {
        private static SharepointHelper _helper;
        public MsGraphSharePointHelper(MsGraphApiConfig config)
        {
            MsGraphHelper graphHelper = new MsGraphHelper(config);
            _helper = new SharepointHelper(graphHelper.GetHttpClient());
        }

        public SharepointModel.Site GetSite(string hostName, string siteName)
        {
            var site = _helper.GetSite(hostName, siteName);
            return site.ToSiteModule();
        }

        public SharepointModel.SiteDriveItemModel GetFolderByUrl(string absoluteUrl)
        {
            var folder = _helper.GetFolderByUrl(absoluteUrl);
            return folder.ToSiteDriveItem();
        }

        public List<SharepointModel.SiteDriveItemModel> CreateFolder(string hostName, string siteName, string parentFolderAbsUrl, string folderNameOrPath)
        {
            var site = this.GetSite(hostName, siteName);
            var parentFolder = this.GetFolderByUrl(parentFolderAbsUrl);

            List<SharepointModel.SiteDriveItemModel> listNewFolders = new List<SharepointModel.SiteDriveItemModel>();

            string[] folders = folderNameOrPath.Split('\\');

            for (int i = 0; i < folders.Length; i++)
            {
                SharepointModel.SiteDriveItemModel siteDriveItem = new SharepointModel.SiteDriveItemModel
                {
                    ParentDriveItem = new SharepointModel.SiteDriveItemModel
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

        private static SharepointModel.SiteDriveItemModel CreateFolder(string siteId, string parentFolderId, string folderName)
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

        public SharepointModel.SiteDriveItemModel UploadFileByFolderUrl(string hostName, string siteName, string absoluteUrl, string fileName, byte[] fileContent)
        {
            var site = this.GetSite(hostName, siteName);
            var folder = this.GetFolderByUrl(absoluteUrl);

            SharepointModel.SiteUploadFileModel module = new SharepointModel.SiteUploadFileModel
            {
                FileName = fileName,
                FileContent = fileContent,
                SiteId = site.Id,
                DriveItemId = folder.Id
            };

            var item = _helper.UploadFile(module);
            return item.ToSiteDriveItem();
        }

        public SharepointModel.SiteDriveItemModel UploadFileByFolderId(string hostName, string siteName, string folderId, string fileName, byte[] fileContent)
        {
            var site = this.GetSite(hostName, siteName);

            SharepointModel.SiteUploadFileModel module = new SharepointModel.SiteUploadFileModel
            {
                FileName = fileName,
                FileContent = fileContent,
                SiteId = site.Id,
                DriveItemId = folderId
            };

            var item = _helper.UploadFile(module);
            return item.ToSiteDriveItem();
        }

        public SharepointModel.SiteDriveItemModel GetFileByName(string hostName, string siteName, string folderAbsUrl, string fileName)
        {
            var site = this.GetSite(hostName, siteName);
            var folder = this.GetFolderByUrl(folderAbsUrl);

            var file = _helper.GetFileByName(site.Id, folder.Id, fileName);
            return file.ToSiteDriveItem();
        }

        public SharepointModel.SiteDriveItemModel GetFileById(string hostName, string siteName, string fileId)
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
        public List<SharepointModel.SiteDriveItemModel> GetFiles(string folderAbsUrl)
        {
            var files = _helper.GetItemsByUrl(folderAbsUrl);
            return files.value.Select(p => p.ToSiteDriveItem()).ToList();
        }

        public SharepointModel.SiteDriveItemPreviewModule CreateItemPreviewLink(string hostName, string siteName, string fileId)
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
