using System.Collections.Generic;
using System.Linq;
using TryIT.MicrosoftGraphApi.Helper;
using TryIT.MicrosoftGraphApi.HttpClientHelper;
using TryIT.MicrosoftGraphApi.Model;
using TryIT.MicrosoftGraphApi.Response.Sharepoint;

namespace TryIT.MicrosoftGraphApi.MsGraphApi
{
    /// <summary>
    /// api with /drives/{drive-id}
    /// </summary>
    public class SharepointApi
    {
        private SharepointHelper _helper;

        /// <summary>
        /// init Teams api with configuration
        /// </summary>
        /// <param name="config"></param>
        public SharepointApi(MsGraphApiConfig config)
        {
            MsGraphHelper graphHelper = new MsGraphHelper(config);
            _helper = new SharepointHelper(graphHelper.GetHttpClient());
        }

        /// <summary>
        /// upload file into Sharepoint by folder absolute url
        /// </summary>
        /// <param name="folderUrl"></param>
        /// <param name="fileName"></param>
        /// <param name="fileContent"></param>
        /// <returns>created Sharepoint item (Id and Name)</returns>
        public GetDriveItemResponse.Item UploadFile(string folderUrl, string fileName, byte[] fileContent)
        {
            return _helper.UploadFile(folderUrl, fileName, fileContent);
        }

        /// <summary>
        /// get file item only from a folder
        /// </summary>
        /// <param name="folderUrl"></param>
        /// <returns></returns>
        public List<GetDriveItemResponse.Item> GetFiles(string folderUrl)
        {
            return _helper.GetChildren(folderUrl).Where(p => p.file != null).ToList();
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
        /// create subfolder under <paramref name="parentFolderUrl"/>, the subfolder <paramref name="folderNameOrPath"/> could be subfolder name or subfolder path, e.g A\B\C
        /// </summary>
        /// <param name="parentFolderUrl"></param>
        /// <param name="folderNameOrPath"></param>
        /// <returns>created subfolder list</returns>
        public List<GetDriveItemResponse.Item> CreateFolder(string parentFolderUrl, string folderNameOrPath)
        {
            List<GetDriveItemResponse.Item> listNewFolders = new List<GetDriveItemResponse.Item>();
            var parentFolder = _helper.GetFolder(parentFolderUrl);

            string[] folders = folderNameOrPath.Split('\\');

            for (int i = 0; i < folders.Length; i++)
            {
                string folderName = folders[i];
                parentFolder = CreateSubFolder(parentFolder.webUrl, folderName);
                listNewFolders.Add(parentFolder);
            }

            return listNewFolders;
        }

        private GetDriveItemResponse.Item CreateSubFolder(string parentFolderUrl, string subFolderName)
        {
            return _helper.CreateFolder(parentFolderUrl, subFolderName);
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
            var file = this.GetFiles(sourceFolderUrl).Where(p => p.name.IsEquals(sourceFileName)).First();
            if (string.IsNullOrEmpty(targetFileName))
            {
                targetFileName = sourceFileName;
            }
            return _helper.MoveFile(file.id, targetFolderUrl, targetFileName);
        }
    }
}