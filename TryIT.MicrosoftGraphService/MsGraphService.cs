using TryIT.MicrosoftGraphService.Helper;
using TryIT.MicrosoftGraphService.Model;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TryIT.MicrosoftGraphService
{
    public class MsGraphService
    {
        private const string ParamName_HostName = "Sharepoint Host Name";
        private const string ParamName_SiteName = "Sharepoint Site Name";
        private const string ParamName_FolderID = "Sharepoint Folder Unique ID";
        private const string ParamName_FolderAbsoluteUrl = "Sharepoint Folder Absolute Url";
        private const string ParamName_FileID = "Sharepoint File Unique ID";
        private const string ParamName_FileName = "Sharepoint File Name";
        private const string ParamName_FileContent = "Sharepoint File Content";
        private const string ParamName_FolderName = "Sharepoint Folder Name or Path";

        public class Sharepoint
        {
            private MsGraphApiConfig _config;
            public Sharepoint(MsGraphApiConfig config)
            {
                _config = config;
            }

            /// <summary>
            /// upload file into Sharepoint by folder absolute url
            /// </summary>
            /// <param name="hostName"></param>
            /// <param name="siteName"></param>
            /// <param name="folderAbsoluteUrl"></param>
            /// <param name="fileName"></param>
            /// <param name="fileContent"></param>
            /// <returns>created Sharepoint item (Id and Name)</returns>
            public SharepointModel.SiteDriveItemModel UploadFileByFolderUrl(string hostName, string siteName, string folderAbsoluteUrl, string fileName, byte[] fileContent)
            {
                NotEmptyParameter(ParamName_HostName, hostName);
                NotEmptyParameter(ParamName_SiteName, siteName);
                NotEmptyParameter(ParamName_FolderAbsoluteUrl, folderAbsoluteUrl);
                NotEmptyParameter(ParamName_FileName, fileName);
                NotNullParameter(ParamName_FileContent, fileContent);

                MsGraphSharePointHelper helper = new MsGraphSharePointHelper(_config);
                return helper.UploadFileByFolderUrl(hostName, siteName, folderAbsoluteUrl, fileName, fileContent);
            }

            /// <summary>
            /// upload file into Sharepoint by folder unique Id
            /// </summary>
            /// <param name="hostName"></param>
            /// <param name="siteName"></param>
            /// <param name="folderId"></param>
            /// <param name="fileName"></param>
            /// <param name="fileContent"></param>
            /// <returns>created Sharepoint item (Id and Name)</returns>
            public SharepointModel.SiteDriveItemModel UploadFileByFolderId(string hostName, string siteName, string folderId, string fileName, byte[] fileContent)
            {
                NotEmptyParameter(ParamName_HostName, hostName);
                NotEmptyParameter(ParamName_SiteName, siteName);
                NotEmptyParameter(ParamName_FolderID, folderId);
                NotEmptyParameter(ParamName_FileName, fileName);
                NotNullParameter(ParamName_FileContent, fileContent);

                MsGraphSharePointHelper helper = new MsGraphSharePointHelper(_config);
                return helper.UploadFileByFolderId(hostName, siteName, folderId, fileName, fileContent);
            }

            /// <summary>
            /// get file content from Sharepoint by fileId
            /// </summary>
            /// <param name="hostName"></param>
            /// <param name="siteName"></param>
            /// <param name="fileId"></param>
            /// <returns></returns>
            public byte[] GetFileContentByFileId(string hostName, string siteName, string fileId)
            {
                NotEmptyParameter(ParamName_HostName, hostName);
                NotEmptyParameter(ParamName_SiteName, siteName);
                NotEmptyParameter(ParamName_FileID, fileId);

                MsGraphSharePointHelper helper = new MsGraphSharePointHelper(_config);
                return helper.GetFileContentByFileId(hostName, siteName, fileId);
            }

            /// <summary>
            /// get file from Sharepoint by specific file name, e.g. fileName = "test.xlsx"
            /// </summary>
            /// <param name="hostName"></param>
            /// <param name="siteName"></param>
            /// <param name="folderAbsUrl"></param>
            /// <param name="fileName"></param>
            /// <returns></returns>
            public SharepointModel.SiteDriveItemModel GetFileByName(string hostName, string siteName, string folderAbsUrl, string fileName)
            {
                NotEmptyParameter(ParamName_HostName, hostName);
                NotEmptyParameter(ParamName_SiteName, siteName);
                NotEmptyParameter(ParamName_FolderAbsoluteUrl, folderAbsUrl);
                NotEmptyParameter(ParamName_FileName, fileName);

                MsGraphSharePointHelper helper = new MsGraphSharePointHelper(_config);
                return helper.GetFileByName(hostName, siteName, folderAbsUrl, fileName);
            }

            public List<SharepointModel.SiteDriveItemModel> GetFiles(string hostName, string siteName, string folderAbsUrl)
            {
                NotEmptyParameter(ParamName_HostName, hostName);
                NotEmptyParameter(ParamName_SiteName, siteName);
                NotEmptyParameter(ParamName_FolderAbsoluteUrl, folderAbsUrl);
                
                MsGraphSharePointHelper helper = new MsGraphSharePointHelper(_config);
                return helper.GetFiles(folderAbsUrl);
            }

            /// <summary>
            /// get file from Sharepoint by the sharepoint unique id
            /// </summary>
            /// <param name="hostName"></param>
            /// <param name="siteName"></param>
            /// <param name="fileId"></param>
            /// <returns></returns>
            public SharepointModel.SiteDriveItemModel GetFileById(string hostName, string siteName, string fileId)
            {
                NotEmptyParameter(ParamName_HostName, hostName);
                NotEmptyParameter(ParamName_SiteName, siteName);
                NotEmptyParameter(ParamName_FileID, fileId);

                MsGraphSharePointHelper helper = new MsGraphSharePointHelper(_config);
                return helper.GetFileById(hostName, siteName, fileId);
            }

            public SharepointModel.SiteDriveItemPreviewModule CreateItemPreviewLink(string hostName, string siteName, string fileId)
            {
                NotEmptyParameter(ParamName_HostName, hostName);
                NotEmptyParameter(ParamName_SiteName, siteName);
                NotEmptyParameter(ParamName_FileID, fileId);

                MsGraphSharePointHelper helper = new MsGraphSharePointHelper(_config);
                return helper.CreateItemPreviewLink(hostName, siteName, fileId);
            }

            /// <summary>
            /// delete file from Sharepoint, return true if delete success, otherwise return false
            /// </summary>
            /// <param name="hostName"></param>
            /// <param name="siteName"></param>
            /// <param name="fileId">the File Sharepoint Unique Id</param>
            /// <param name="isDeleteFolderIfEmpty">indicator to remove folder if it become empty after file deleted</param>
            /// <returns></returns>
            public bool DeleteFileByFileId(string hostName, string siteName, string fileId, bool isDeleteFolderIfEmpty)
            {
                NotEmptyParameter(ParamName_HostName, hostName);
                NotEmptyParameter(ParamName_SiteName, siteName);
                NotEmptyParameter(ParamName_FileID, fileId);

                MsGraphSharePointHelper helper = new MsGraphSharePointHelper(_config);
                return helper.DeleteFileById(hostName, siteName, fileId, isDeleteFolderIfEmpty);
            }

            /// <summary>
            /// create new Sharepoint folder under <paramref name="parentFolderAbsUrl"/>, 
            /// <para>set <paramref name="folderNameOrPath"/> to "NewFolderName" to create a new folder</para>
            /// <para>set <paramref name="folderNameOrPath"/> to "NewFolderName\NewSubFolderName" to create a new folder and sub folder</para>
            /// </summary>
            /// <param name="hostName"></param>
            /// <param name="siteName"></param>
            /// <param name="parentFolderAbsUrl"></param>
            /// <param name="folderNameOrPath"></param>
            /// <returns></returns>
            public List<SharepointModel.SiteDriveItemModel> CreateFolder(string hostName, string siteName, string parentFolderAbsUrl, string folderNameOrPath)
            {
                NotEmptyParameter(ParamName_HostName, hostName);
                NotEmptyParameter(ParamName_SiteName, siteName);
                NotEmptyParameter(ParamName_FolderAbsoluteUrl, parentFolderAbsUrl);
                NotEmptyParameter(ParamName_FolderName, folderNameOrPath);

                MsGraphSharePointHelper helper = new MsGraphSharePointHelper(_config);
                return helper.CreateFolder(hostName, siteName, parentFolderAbsUrl, folderNameOrPath);
            }

            /// <summary>
            /// get items inside a folder, either is file or is a sub folder
            /// </summary>
            /// <param name="hostName"></param>
            /// <param name="siteName"></param>
            /// <param name="folderAbsUrl"></param>
            /// <returns></returns>
            public List<SharepointModel.SiteDriveItemModel> GetItems(string hostName, string siteName, string folderAbsUrl)
            {
                NotEmptyParameter(ParamName_HostName, hostName);
                NotEmptyParameter(ParamName_SiteName, siteName);
                NotEmptyParameter(ParamName_FolderAbsoluteUrl, folderAbsUrl);

                MsGraphSharePointHelper helper = new MsGraphSharePointHelper(_config);
                return helper.GetItems(folderAbsUrl);
            }

            string _hostName;
            string _siteName;
            List<string> _listFiles;
            /// <summary>
            /// download Sharepoint folder into <paramref name="localRootFolder"/>, this method will create same folder structure
            /// </summary>
            /// <param name="hostName"></param>
            /// <param name="siteName"></param>
            /// <param name="folderAbsUrl"></param>
            /// <param name="localRootFolder"></param>
            /// <returns>download file list</returns>
            /// <exception cref="System.ArgumentNullException"></exception>
            public List<string> DownloadFolder(string hostName, string siteName, string folderAbsUrl, string localRootFolder)
            {
                NotEmptyParameter(ParamName_HostName, hostName);
                NotEmptyParameter(ParamName_SiteName, siteName);
                NotEmptyParameter(ParamName_FolderAbsoluteUrl, folderAbsUrl);

                if (string.IsNullOrEmpty(localRootFolder))
                {
                    throw new System.ArgumentNullException(nameof(localRootFolder));
                }

                _hostName = hostName;
                _siteName = siteName;
                _listFiles = new List<string>();

                MsGraphSharePointHelper helper = new MsGraphSharePointHelper(_config);
                var driveItems = helper.GetItems(folderAbsUrl);

                foreach (var item in driveItems)
                {
                    DownloadFile(item, localRootFolder);
                }

                return _listFiles;
            }

            private void DownloadFile(SharepointModel.SiteDriveItemModel item, string folderPath)
            {
                if (item.IsFile)
                {
                    var _byte = GetFileContentByFileId(_hostName, _siteName, item.Id);
                    string localFileNameAndPath = System.IO.Path.Combine(folderPath, item.Name);

                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    File.WriteAllBytes(localFileNameAndPath, _byte);
                    _listFiles.Add(localFileNameAndPath);
                }
                else
                {
                    string parentPath = Path.Combine(folderPath, item.Name);
                    if (!Directory.Exists(parentPath))
                    {
                        Directory.CreateDirectory(parentPath);
                    }

                    var subItems = GetItems(_hostName, _siteName, item.WebUrl);
                    if (subItems.Count > 0)
                    {
                        foreach (var subItem in subItems)
                        {
                            DownloadFile(subItem, parentPath);
                        }
                    }
                }
            }


            /// <summary>
            /// upload local folder to Sharepoint folder
            /// </summary>
            /// <param name="hostName"></param>
            /// <param name="siteName"></param>
            /// <param name="folderAbsUrl"></param>
            /// <param name="localRootFolder"></param>
            /// <returns></returns>
            /// <exception cref="System.ArgumentNullException"></exception>
            /// <exception cref="DirectoryNotFoundException"></exception>
            public List<string> UploadFolder(string hostName, string siteName, string folderAbsUrl, string localRootFolder)
            {
                NotEmptyParameter(ParamName_HostName, hostName);
                NotEmptyParameter(ParamName_SiteName, siteName);
                NotEmptyParameter(ParamName_FolderAbsoluteUrl, folderAbsUrl);

                if (string.IsNullOrEmpty(localRootFolder))
                {
                    throw new System.ArgumentNullException(nameof(localRootFolder));
                }

                if (!Directory.Exists(localRootFolder))
                {
                    throw new DirectoryNotFoundException($"Source directory not found: {localRootFolder}");
                }

                _hostName = hostName;
                _siteName = siteName;
                _listFiles = new List<string>();

                UploadFile(localRootFolder, folderAbsUrl);
                return _listFiles;
            }
            private void UploadFile(string sourceDir, string sharepointFolderUrl)
            {
                var dir = new DirectoryInfo(sourceDir);

                DirectoryInfo[] dirs = dir.GetDirectories();

                foreach (FileInfo file in dir.GetFiles())
                {
                    UploadFileByFolderUrl(_hostName, _siteName, sharepointFolderUrl, file.Name, File.ReadAllBytes(file.FullName));

                    _listFiles.Add(Path.Combine(sourceDir, file.FullName));
                }

                foreach (DirectoryInfo subDir in dirs)
                {
                    var items = GetItems(_hostName, _siteName, sharepointFolderUrl);
                    var subfolder = items.Where(p => !p.IsFile && p.Name.Equals(subDir.Name, System.StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                    string subFolderUrl = string.Empty;
                    if (subfolder == null)
                    {
                        var newfolder = CreateFolder(_hostName, _siteName, sharepointFolderUrl, subDir.Name);
                        subFolderUrl = newfolder.First().WebUrl;
                    }
                    else
                    {
                        subFolderUrl = subfolder.WebUrl;
                    }

                    UploadFile(subDir.FullName, subFolderUrl);
                }
            }
            
            /// <summary>
            /// delete item from Sharepoint
            /// </summary>
            /// <param name="hostName"></param>
            /// <param name="siteName"></param>
            /// <param name="itemId"></param>
            /// <returns></returns>
            /// <exception cref="System.ArgumentNullException"></exception>
            public bool DeleteItem(string hostName, string siteName, string itemId)
            {
                NotEmptyParameter(ParamName_HostName, hostName);
                NotEmptyParameter(ParamName_SiteName, siteName);
                
                if (string.IsNullOrEmpty(itemId))
                {
                    throw new System.ArgumentNullException(nameof(itemId));
                }

                MsGraphSharePointHelper helper = new MsGraphSharePointHelper(_config);
                return helper.DeleteItem(hostName, siteName, itemId);
            }

            /// <summary>
            /// move item into another folder
            /// </summary>
            /// <param name="hostName"></param>
            /// <param name="siteName"></param>
            /// <param name="itemId"></param>
            /// <param name="newParentId"></param>
            /// <returns></returns>
            /// <exception cref="System.ArgumentNullException"></exception>
            public bool MoveItem(string hostName, string siteName, MoveSharepointItemModel moveItem)
            {
                NotEmptyParameter(ParamName_HostName, hostName);
                NotEmptyParameter(ParamName_SiteName, siteName);
                
                if (moveItem == null)
                {
                    throw new System.ArgumentNullException(nameof(moveItem));
                }
                if (string.IsNullOrEmpty(moveItem.itemId))
                {
                    throw new System.ArgumentNullException(nameof(moveItem.itemId));
                }
                if (string.IsNullOrEmpty(moveItem.parentId))
                {
                    throw new System.ArgumentNullException(nameof(moveItem.parentId));
                }

                MsGraphSharePointHelper helper = new MsGraphSharePointHelper(_config);
                return helper.MoveItem(hostName, siteName, moveItem);
            }
        }

        public class User
        {
            private MsGraphApiConfig _config;
            public User(MsGraphApiConfig config)
            {
                _config = config;
            }

            /// <summary>
            /// get user info, if <paramref name="userEmail"/> is empty, get me
            /// </summary>
            /// <param name="emailAddress"></param>
            /// <returns></returns>
            public UserModel GetUser(string emailAddress = "")
            {
                MsGraphUserHelper helper = new MsGraphUserHelper(_config);

                return helper.GetUserInfo(emailAddress);
            }
        }

        public class Team
        {
            private MsGraphApiConfig _config;
            public Team(MsGraphApiConfig config)
            {
                _config = config;
            }
            /// <summary>
            /// get list members of team, the <paramref name="userEmail"/> need be in the team, so that have permission to get the list
            /// </summary>
            /// <param name="teamName"></param>
            /// <param name="userEmail"></param>
            /// <returns></returns>
            public List<TeamModel.Member> GetMembers(string teamName, string userEmail)
            {
                MsGraphTeamHelper helper = new MsGraphTeamHelper(_config);
                return helper.GetMembers(teamName, userEmail);
            }

            public void AddMember(string teamName, string userEmail)
            {
                MsGraphTeamHelper helper = new MsGraphTeamHelper(_config);
                helper.AddMember(teamName, userEmail);
            }
            public void RemoveMember(string teamName, string userEmail, string membershipId)
            {
                MsGraphTeamHelper helper = new MsGraphTeamHelper(_config);
                helper.RemoveMember(teamName, userEmail, membershipId);
            }
        }

        public class Outlook
        {
            private MsGraphApiConfig _config;
            public Outlook(MsGraphApiConfig config)
            {
                _config = config;
            }

            /// <summary>
            /// get message of user
            /// </summary>
            /// <param name="user">'Me' or specific email address, default is Me if pass empty</param>
            /// <param name="folder">'Index' or specific folder, default is Index if pass empty</param>
            /// <returns></returns>
            public List<MailboxModel.Message> GetMessages(string userEmail, string folder)
            {
                MsGraphOutlookHelper helper = new MsGraphOutlookHelper(_config);
                return helper.GetMessages(userEmail, folder);
            }

            /// <summary>
            /// send message as current user
            /// </summary>
            /// <param name="message"></param>
            public void SendMessage(SendMessageModel message)
            {
                MsGraphOutlookHelper helper = new MsGraphOutlookHelper(_config);
                helper.SendMessage(message);
            }
        }

        #region Parameter Validation
        private static void NotEmptyParameter(string paramName, string paramValue)
        {
            if (!string.IsNullOrEmpty(paramName) && string.IsNullOrEmpty(paramValue))
            {
                throw new System.ArgumentNullException($"'{paramName}'", "invalid parameter value, must be not null or empty");
            }
        }
        private static void NotNullParameter(string paramName, object paramValue)
        {
            if (!string.IsNullOrEmpty(paramName) && paramName == null)
            {
                throw new System.ArgumentNullException($"'{paramName}'", "invalid parameter value, must be not null");
            }
        } 
        #endregion
    }
}
