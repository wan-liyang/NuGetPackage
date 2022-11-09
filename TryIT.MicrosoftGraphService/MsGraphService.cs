using TryIT.MicrosoftGraphService.Helper;
using TryIT.MicrosoftGraphService.Model;
using System.Collections.Generic;

namespace TryIT.MicrosoftGraphService
{
    public class MsGraphService
    {
        private const string ParamName_HostName = "SharePoint Host Name";
        private const string ParamName_SiteName = "SharePoint Site Name";
        private const string ParamName_FolderID = "SharePoint Folder Unique ID";
        private const string ParamName_FolderAbsoluteUrl = "SharePoint Folder Absolute Url";
        private const string ParamName_FileID = "SharePoint File Unique ID";
        private const string ParamName_FileName = "SharePoint File Name";
        private const string ParamName_FileContent = "SharePoint File Content";
        private const string ParamName_FolderName = "SharePoint Folder Name or Path";

        public class SharePoint
        {
            private MsGraphApiConfig _config;
            public SharePoint(MsGraphApiConfig config)
            {
                _config = config;
            }

            /// <summary>
            /// upload file into SharePoint by folder absolute url
            /// </summary>
            /// <param name="hostName"></param>
            /// <param name="siteName"></param>
            /// <param name="folderAbsoluteUrl"></param>
            /// <param name="fileName"></param>
            /// <param name="fileContent"></param>
            /// <returns>created SharePoint item (Id and Name)</returns>
            public SharePointModel.SiteDriveItemModel UploadFileByFolderUrl(string hostName, string siteName, string folderAbsoluteUrl, string fileName, byte[] fileContent)
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
            /// upload file into SharePoint by folder unique Id
            /// </summary>
            /// <param name="hostName"></param>
            /// <param name="siteName"></param>
            /// <param name="folderId"></param>
            /// <param name="fileName"></param>
            /// <param name="fileContent"></param>
            /// <returns>created SharePoint item (Id and Name)</returns>
            public SharePointModel.SiteDriveItemModel UploadFileByFolderId(string hostName, string siteName, string folderId, string fileName, byte[] fileContent)
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
            /// get file content from SharePoint by fileId
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
            /// get file from SharePoint by specific file name, e.g. fileName = "test.xlsx"
            /// </summary>
            /// <param name="hostName"></param>
            /// <param name="siteName"></param>
            /// <param name="folderAbsUrl"></param>
            /// <param name="fileName"></param>
            /// <returns></returns>
            public SharePointModel.SiteDriveItemModel GetFileByName(string hostName, string siteName, string folderAbsUrl, string fileName)
            {
                NotEmptyParameter(ParamName_HostName, hostName);
                NotEmptyParameter(ParamName_SiteName, siteName);
                NotEmptyParameter(ParamName_FolderAbsoluteUrl, folderAbsUrl);
                NotEmptyParameter(ParamName_FileName, fileName);

                MsGraphSharePointHelper helper = new MsGraphSharePointHelper(_config);
                return helper.GetFileByName(hostName, siteName, folderAbsUrl, fileName);
            }

            public List<SharePointModel.SiteDriveItemModel> GetFiles(string hostName, string siteName, string folderAbsUrl)
            {
                NotEmptyParameter(ParamName_HostName, hostName);
                NotEmptyParameter(ParamName_SiteName, siteName);
                NotEmptyParameter(ParamName_FolderAbsoluteUrl, folderAbsUrl);
                
                MsGraphSharePointHelper helper = new MsGraphSharePointHelper(_config);
                return helper.GetFiles(folderAbsUrl);
            }

            /// <summary>
            /// get file from SharePoint by the sharepoint unique id
            /// </summary>
            /// <param name="hostName"></param>
            /// <param name="siteName"></param>
            /// <param name="fileId"></param>
            /// <returns></returns>
            public SharePointModel.SiteDriveItemModel GetFileById(string hostName, string siteName, string fileId)
            {
                NotEmptyParameter(ParamName_HostName, hostName);
                NotEmptyParameter(ParamName_SiteName, siteName);
                NotEmptyParameter(ParamName_FileID, fileId);

                MsGraphSharePointHelper helper = new MsGraphSharePointHelper(_config);
                return helper.GetFileById(hostName, siteName, fileId);
            }

            public SharePointModel.SiteDriveItemPreviewModule CreateItemPreviewLink(string hostName, string siteName, string fileId)
            {
                NotEmptyParameter(ParamName_HostName, hostName);
                NotEmptyParameter(ParamName_SiteName, siteName);
                NotEmptyParameter(ParamName_FileID, fileId);

                MsGraphSharePointHelper helper = new MsGraphSharePointHelper(_config);
                return helper.CreateItemPreviewLink(hostName, siteName, fileId);
            }

            /// <summary>
            /// delete file from SharePoint, return true if delete success, otherwise return false
            /// </summary>
            /// <param name="hostName"></param>
            /// <param name="siteName"></param>
            /// <param name="fileId">the File SharePoint Unique Id</param>
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
            /// create new SharePoint folder under <paramref name="parentFolderAbsUrl"/>, 
            /// <para>set <paramref name="folderNameOrPath"/> to "NewFolderName" to create a new folder</para>
            /// <para>set <paramref name="folderNameOrPath"/> to "NewFolderName\NewSubFolderName" to create a new folder and sub folder</para>
            /// </summary>
            /// <param name="hostName"></param>
            /// <param name="siteName"></param>
            /// <param name="parentFolderAbsUrl"></param>
            /// <param name="folderNameOrPath"></param>
            /// <returns></returns>
            public List<SharePointModel.SiteDriveItemModel> CreateFolder(string hostName, string siteName, string parentFolderAbsUrl, string folderNameOrPath)
            {
                NotEmptyParameter(ParamName_HostName, hostName);
                NotEmptyParameter(ParamName_SiteName, siteName);
                NotEmptyParameter(ParamName_FolderAbsoluteUrl, parentFolderAbsUrl);
                NotEmptyParameter(ParamName_FolderName, folderNameOrPath);

                MsGraphSharePointHelper helper = new MsGraphSharePointHelper(_config);
                return helper.CreateFolder(hostName, siteName, parentFolderAbsUrl, folderNameOrPath);
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
