using System.Collections.Generic;
using System.IO;
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
        /// get all children item from a folder, include file and folder
        /// </summary>
        /// <param name="folderUrl"></param>
        /// <returns></returns>
        public List<GetDriveItemResponse.Item> GetChildren(string folderUrl)
        {
            return _helper.GetChildren(folderUrl);
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

        /// <summary>
        /// delete item from sharepoint folder
        /// </summary>
        /// <param name="folderUrl"></param>
        /// <param name="itemName"></param>
        /// <returns></returns>
        public bool DeleteItem(string folderUrl, string itemName)
        {
            return _helper.DeleteItem(folderUrl, itemName);
        }

        List<string> _listFiles;
        #region Download sharepoint folder to local
        /// <summary>
        /// download sharepoint folder to local, recursive subfolder
        /// </summary>
        /// <param name="folderAbsUrl"></param>
        /// <param name="localRootFolder"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public List<string> DownloadFolder(string folderAbsUrl, string localRootFolder)
        {
            if (string.IsNullOrEmpty(localRootFolder))
            {
                throw new System.ArgumentNullException(nameof(localRootFolder));
            }

            _listFiles = new List<string>();

            var driveItems = GetChildren(folderAbsUrl);

            foreach (var item in driveItems)
            {
                _DownloadFile(item, localRootFolder);
            }

            return _listFiles;
        }

        private void _DownloadFile(GetDriveItemResponse.Item item, string folderPath)
        {
            if (item.file != null)
            {
                var _byte = _helper.GetFileContent(item.MicrosoftGraphDownloadUrl);

                string localFileNameAndPath = System.IO.Path.Combine(folderPath, item.name);

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                File.WriteAllBytes(localFileNameAndPath, _byte);
                _listFiles.Add(localFileNameAndPath);
            }
            else
            {
                string parentPath = Path.Combine(folderPath, item.name);
                if (!Directory.Exists(parentPath))
                {
                    Directory.CreateDirectory(parentPath);
                }

                var subItems = GetChildren(item.webUrl);
                if (subItems.Count > 0)
                {
                    foreach (var subItem in subItems)
                    {
                        _DownloadFile(subItem, parentPath);
                    }
                }
            }
        }
        #endregion

        #region Upload local folder to sharepoint
        /// <summary>
        /// upload local folder to sharepoint
        /// </summary>
        /// <param name="folderAbsUrl"></param>
        /// <param name="localRootFolder"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public List<string> UploadFolder(string folderAbsUrl, string localRootFolder)
        {
            if (string.IsNullOrEmpty(localRootFolder))
            {
                throw new System.ArgumentNullException(nameof(localRootFolder));
            }

            if (!Directory.Exists(localRootFolder))
            {
                throw new DirectoryNotFoundException($"Source directory not found: {localRootFolder}");
            }

            _listFiles = new List<string>();

            _UploadFile(localRootFolder, folderAbsUrl);
            return _listFiles;
        }
        private void _UploadFile(string sourceDir, string sharepointFolderUrl)
        {
            var dir = new DirectoryInfo(sourceDir);

            DirectoryInfo[] dirs = dir.GetDirectories();

            foreach (FileInfo file in dir.GetFiles())
            {
                UploadFile(sharepointFolderUrl, file.Name, File.ReadAllBytes(file.FullName));
                _listFiles.Add(Path.Combine(sourceDir, file.FullName));
            }

            foreach (DirectoryInfo subDir in dirs)
            {
                var items = GetChildren(sharepointFolderUrl);
                var subfolder = items.Where(p => p.folder != null && p.name.IsEquals(subDir.Name)).FirstOrDefault();
                string subFolderUrl = string.Empty;
                if (subfolder == null)
                {
                    var newfolder = CreateFolder(sharepointFolderUrl, subDir.Name);
                    subFolderUrl = newfolder.First().webUrl;
                }
                else
                {
                    subFolderUrl = subfolder.webUrl;
                }
                _UploadFile(subDir.FullName, subFolderUrl);
            }
        }
        #endregion
    }
}