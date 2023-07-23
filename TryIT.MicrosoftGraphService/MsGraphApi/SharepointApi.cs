using System.Collections.Generic;
using System.Linq;
using TryIT.MicrosoftGraphService.ApiModel;
using TryIT.MicrosoftGraphService.Helper;
using TryIT.MicrosoftGraphService.HttpClientHelper;
using TryIT.MicrosoftGraphService.Model;

namespace TryIT.MicrosoftGraphService.MsGraphApi
{
    /// <summary>
    /// api with /drives/{drive-id}
    /// </summary>
    public class SharepointApi
    {
        private SharepointDriveHelper _helper;

        /// <summary>
        /// init Teams api with configuration
        /// </summary>
        /// <param name="config"></param>
        public SharepointApi(MsGraphApiConfig config)
        {
            MsGraphHelper graphHelper = new MsGraphHelper(config);
            _helper = new SharepointDriveHelper(graphHelper.GetHttpClient());
        }

        /// <summary>
        /// upload file into Sharepoint by folder absolute url
        /// </summary>
        /// <param name="folderUrl"></param>
        /// <param name="fileName"></param>
        /// <param name="fileContent"></param>
        /// <returns>created Sharepoint item (Id and Name)</returns>
        public SharepointModel.SiteDriveItemModel UploadFile(string folderUrl, string fileName, byte[] fileContent)
        {
            var item = _helper.UploadFile(folderUrl, fileName, fileContent);

            return item.ToSiteDriveItem();
        }

        /// <summary>
        /// get files only from folder
        /// </summary>
        /// <param name="folderUrl"></param>
        /// <returns></returns>
        public List<SharepointModel.SiteDriveItemModel> GetFiles(string folderUrl)
        {
            return _helper.GetChildren(folderUrl).value?.Where(p => p.file != null)?.Select(p => p.ToSiteDriveItem()).ToList();
        }

        /// <summary>
        /// get file content (aka download file)
        /// </summary>
        /// <param name="folderAbsoluteUrl"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public byte[] GetFileContent(string folderAbsoluteUrl, string fileName)
        {
            return _helper.GetFileContent(folderAbsoluteUrl, fileName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentFolderUrl"></param>
        /// <param name="folderNameOrPath"></param>
        /// <returns></returns>
        public List<SharepointModel.SiteDriveItemModel> CreateFolder(string parentFolderUrl, string folderNameOrPath)
        {
            List<SharepointModel.SiteDriveItemModel> listNewFolders = new List<SharepointModel.SiteDriveItemModel>();
            var parentFolder = _helper.GetFolder(parentFolderUrl).ToSiteDriveItem();

            string[] folders = folderNameOrPath.Split('\\');

            for (int i = 0; i < folders.Length; i++)
            {
                SharepointModel.SiteDriveItemModel siteDriveItem = new SharepointModel.SiteDriveItemModel
                {
                    ParentDriveItem = new SharepointModel.SiteDriveItemModel
                    {
                        Id = parentFolder.Id,
                        Name = parentFolder.Name,
                        WebUrl = parentFolder.WebUrl
                    }
                };

                string folderName = folders[i];
                parentFolder = CreateSubFolder(parentFolder.WebUrl, folderName);

                siteDriveItem.Id = parentFolder.Id;
                siteDriveItem.Name = parentFolder.Name;
                siteDriveItem.WebUrl = parentFolder.WebUrl;

                listNewFolders.Add(siteDriveItem);
            }

            return listNewFolders;
        }

        private SharepointModel.SiteDriveItemModel CreateSubFolder(string parentFolderUrl, string subFolderName)
        {
            var item = _helper.CreateFolder(parentFolderUrl, subFolderName);
            return item.ToSiteDriveItem();
        }

        /// <summary>
        /// move item into another folder
        /// </summary>
        /// <param name="sourceFolderUrl"></param>
        /// <param name="sourceFileName"></param>
        /// <param name="targetFolderUrl"></param>
        /// <param name="targetFileName"></param>
        /// <returns></returns>
        public bool MoveFile(string sourceFolderUrl, string sourceFileName, string targetFolderUrl, string targetFileName)
        {
            var children = _helper.GetChildren(sourceFolderUrl).value;
            var file = children.Where(p => p.file != null && p.name.Equals(sourceFileName, System.StringComparison.CurrentCultureIgnoreCase)).First();

            if (string.IsNullOrEmpty(targetFileName))
            {
                targetFileName = sourceFileName;
            }

            return _helper.MoveFile(file.id, targetFolderUrl, targetFileName);
        }
    }
}