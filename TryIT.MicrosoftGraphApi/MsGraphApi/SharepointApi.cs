using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TryIT.MicrosoftGraphApi.Helper;
using TryIT.MicrosoftGraphApi.HttpClientHelper;
using TryIT.MicrosoftGraphApi.Model;
using TryIT.MicrosoftGraphApi.Model.Sharepoint;
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
        /// get folder information with folder url
        /// </summary>
        /// <param name="folderUrl"></param>
        /// <returns></returns>
        public GetDriveItemResponse.Item GetFolder(string folderUrl)
        {
            return _helper.GetFolder(folderUrl);
        }

        /// <summary>
        /// get item by item path, return null if not found https://learn.microsoft.com/en-us/graph/api/driveitem-get?view=graph-rest-1.0&tabs=http
        /// </summary>
        /// <param name="driveId">get from <see cref="GetFolder(string)"/></param>
        /// <param name="itemPath">e.g. /folder/folder or /folder/folder/file.txt</param>
        /// <returns></returns>
        public GetDriveItemResponse.Item GetItemByPath(string driveId, string itemPath)
        {
            return _helper.GetItemByPath(driveId, itemPath);
        }

        /// <summary>
        /// upload file into Sharepoint folder
        /// </summary>
        /// <param name="driveId"></param>
        /// <param name="folderItemId"></param>
        /// <param name="fileName"></param>
        /// <param name="fileContent"></param>
        /// <returns>created Sharepoint item (Id and Name)</returns>
        public GetDriveItemResponse.Item UploadFile(string driveId, string folderItemId, string fileName, byte[] fileContent)
        {
            return _helper.UploadFile(driveId, folderItemId, fileName, fileContent);
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
        /// List children of a driveItem, https://learn.microsoft.com/en-us/graph/api/driveitem-list-children?view=graph-rest-1.0&tabs=http
        /// </summary>
        /// <param name="folderUrl"></param>
        /// <returns></returns>
        public List<GetDriveItemResponse.Item> GetChildren(string driveId, string itemId)
        {
            return _helper.GetChildren(driveId, itemId);
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
        /// create subfolder under <paramref name="folderItemId"/>, the subfolder <paramref name="newFolderNameOrPath"/> could be subfolder name or subfolder path, e.g A\B\C
        /// </summary>
        /// <param name="driveId"></param>
        /// <param name="folderItemId"></param>
        /// <param name="newFolderNameOrPath"></param>
        /// <param name="stopInherit">indicate whether stop the newly created folder to stop inherit permission from it's parent folder, if yes, program will delete existing permission (by do this delete permission the folder will become stop inherit)</param>
        /// <returns>created subfolder list</returns>
        public List<GetDriveItemResponse.Item> CreateFolder(string driveId, string folderItemId, string newFolderNameOrPath, bool stopInherit = false)
        {
            List<GetDriveItemResponse.Item> listNewFolders = new List<GetDriveItemResponse.Item>();

            var pathList = newFolderNameOrPath.ConvertPathToList();

            string parentFolderId = folderItemId;

            for (int i = 0; i < pathList.Count; i++)
            {
                string subFolderPath = pathList[i];
                string subFolderName = pathList[i].Split('\\').Last();
                var subFolder = _helper.GetItemByPath(driveId, subFolderPath);

                if (subFolder == null)
                {
                    subFolder = _helper.CreateFolder(driveId, parentFolderId, subFolderName);
                    listNewFolders.Add(subFolder);

                    if (stopInherit)
                    {
                        // for new created folder, do remove permission, so that it will stop inherit permission
                        var permissions = _helper.ListPermissions(driveId, subFolder.id);
                        if (permissions != null && permissions.Count > 0)
                        {
                            foreach (var permission in permissions)
                            {
                                _helper.DeletePermission(driveId, subFolder.id, permission.id);
                            }
                        }
                    }
                }
                parentFolderId = subFolder.id;
            }
            return listNewFolders;
        }

        /// <summary>
        /// move item into another folder, if source file not exists, no action will be perform
        /// </summary>
        /// <param name="sourceFolderUrl"></param>
        /// <param name="sourceFileName"></param>
        /// <param name="targetFolderUrl"></param>
        /// <param name="targetFileName"></param>
        /// <returns></returns>
        public bool MoveFile(string sourceFolderUrl, string sourceFileName, string targetFolderUrl, string targetFileName)
        {
            var file = this.GetFiles(sourceFolderUrl).Where(p => p.name.IsEquals(sourceFileName)).FirstOrDefault();
            if (file == null)
            {
                return true;
            }

            if (string.IsNullOrEmpty(targetFileName))
            {
                targetFileName = sourceFileName;
            }
            return _helper.MoveFile(file.id, targetFolderUrl, targetFileName);
        }


        [Obsolete("change to use DeleteDriveItem", true)]
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

        /// <summary>
        /// Delete a DriveItem, https://learn.microsoft.com/en-us/graph/api/driveitem-delete?view=graph-rest-1.0&tabs=http
        /// </summary>
        /// <param name="folderUrl"></param>
        /// <param name="itemName"></param>
        /// <returns></returns>
        public bool DeleteDriveItem(string driveId, string itemId)
        {
            return _helper.DeleteDriveItem(driveId, itemId);
        }

        /// <summary>
        /// rename sharepoint item
        /// </summary>
        /// <param name="driveId"></param>
        /// <param name="itemId"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        public GetDriveItemResponse.Item RenameItem(string driveId, string itemId, string newName)
        {
            return _helper.RenameItem(driveId, itemId, newName);
        }

        /// <summary>
        /// download sharepoint entire folder or individual item to local, recursive subfolder
        /// </summary>
        /// <param name="driveId"></param>
        /// <param name="itemId">folder item id, or content item id</param>
        /// <param name="localRootFolder"></param>
        /// <returns></returns>
        public List<DownloadFolderResult> DownloadItems(string driveId, string itemId, string localRootFolder)
        {
            List<DownloadFolderResult> results = new List<DownloadFolderResult>();

            if (string.IsNullOrEmpty(localRootFolder))
            {
                throw new System.ArgumentNullException(nameof(localRootFolder));
            }

            var items = GetChildren(driveId, itemId);

            foreach (var item in items)
            {
                _DownloadItems(item, localRootFolder, results);
            }

            return results;
        }
        private void _DownloadItems(GetDriveItemResponse.Item item, string folderPath, List<DownloadFolderResult> results)
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

                results.Add(new DownloadFolderResult
                {
                    FileOrFolder = "file",
                    DriveItem = item,
                    LocalItem = localFileNameAndPath,
                });
            }
            else
            {
                string parentPath = Path.Combine(folderPath, item.name);
                if (!Directory.Exists(parentPath))
                {
                    Directory.CreateDirectory(parentPath);
                }

                results.Add(new DownloadFolderResult
                {
                    FileOrFolder = "folder",
                    DriveItem = item,
                    LocalItem = folderPath,
                });

                var subItems = GetChildren(item.parentReference.driveId, item.id);
                if (subItems.Count > 0)
                {
                    foreach (var subItem in subItems)
                    {
                        _DownloadItems(subItem, parentPath, results);
                    }
                }
            }
        }

        #region Upload local folder to sharepoint

        List<FileUploadResult> _uploadFolderStatus;

        /// <summary>
        /// upload local folder to sharepoint
        /// </summary>
        /// <param name="driveId"></param>
        /// <param name="folderItemId"></param>
        /// <param name="localRootFolder"></param>
        /// <param name="allowPartialSuccess">indicator allow partial upload success, true(default): if one file failed, will continue rest, false: if one file failed, will stop and throw exception</param>
        /// <returns>upload status for each file, either success or failure with exception</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public List<FileUploadResult> UploadFolder(string driveId, string folderItemId, string localRootFolder, bool allowPartialSuccess = true)
        {
            if (string.IsNullOrEmpty(localRootFolder))
            {
                throw new System.ArgumentNullException(nameof(localRootFolder));
            }

            if (!Directory.Exists(localRootFolder))
            {
                throw new DirectoryNotFoundException($"Source directory not found: {localRootFolder}");
            }

            _uploadFolderStatus = new List<FileUploadResult>();

            _UploadFile(driveId, folderItemId, localRootFolder, allowPartialSuccess);
            return _uploadFolderStatus;
        }
        private void _UploadFile(string driveId, string folderItemId, string sourceDir, bool allowPartialSuccess)
        {
            var dir = new DirectoryInfo(sourceDir);

            DirectoryInfo[] dirs = dir.GetDirectories();

            foreach (FileInfo file in dir.GetFiles())
            {
                try
                {
                    UploadFile(driveId, folderItemId, file.Name, File.ReadAllBytes(file.FullName));
                    _uploadFolderStatus.Add(new FileUploadResult
                    {
                        FileName = file.Name,
                        FileFullName = file.FullName,
                        Success = true
                    });
                }
                catch (System.Exception ex)
                {
                    _uploadFolderStatus.Add(new FileUploadResult
                    {
                        FileName = file.Name,
                        FileFullName = file.FullName,
                        Success = false,
                        Exception = ex
                    });

                    if (!allowPartialSuccess)
                    {
                        throw;
                    }
                }
            }

            foreach (DirectoryInfo subDir in dirs)
            {
                var items = GetChildren(driveId, folderItemId);
                var subfolder = items.Where(p => p.folder != null && p.name.IsEquals(subDir.Name)).FirstOrDefault();
                string subFolderItemId = string.Empty;
                if (subfolder == null)
                {
                    var newfolder = _helper.CreateFolder(driveId, folderItemId, subDir.Name);
                    subFolderItemId = newfolder.id;
                }
                else
                {
                    subFolderItemId = subfolder.id;
                }
                _UploadFile(driveId, subFolderItemId, subDir.FullName, allowPartialSuccess);
            }
        }
        #endregion

        #region Permissions

        /// <summary>
        /// list permission of item
        /// </summary>
        /// <param name="driveId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public List<ListPermissionsResponse.Value> ListPermissions(string driveId, string itemId)
        {
            return _helper.ListPermissions(driveId, itemId);
        }

        /// <summary>
        /// add permission
        /// </summary>
        /// <param name="driveId"></param>
        /// <param name="itemId"></param>
        /// <param name="addPermissionModel"></param>
        /// <returns></returns>
        public List<AddPermissionResponse.Value> AddPermissions(string driveId, string itemId, AddPermissionModel addPermissionModel)
        {
            return _helper.AddPermissions(driveId, itemId, addPermissionModel);
        }
        /// <summary>
        /// delete permission
        /// </summary>
        /// <param name="driveId"></param>
        /// <param name="itemId"></param>
        /// <param name="permissionId">the id of permission object, can get from <see cref="ListPermissions(string, string)"/></param>
        /// <returns></returns>
        public bool DeletePermission(string driveId, string itemId, string permissionId)
        {
            return _helper.DeletePermission(driveId, itemId, permissionId);
        }
        #endregion
    }
}