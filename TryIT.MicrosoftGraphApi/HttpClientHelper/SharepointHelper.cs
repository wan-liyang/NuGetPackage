using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using TryIT.MicrosoftGraphApi.Helper;
using TryIT.MicrosoftGraphApi.Model;
using TryIT.MicrosoftGraphApi.Model.Sharepoint;
using TryIT.MicrosoftGraphApi.Request.Sharepoint;
using TryIT.MicrosoftGraphApi.Response.Sharepoint;

namespace TryIT.MicrosoftGraphApi.HttpClientHelper
{
    internal class SharepointHelper : BaseHelper
    {
        private readonly SiteHelper _siteHelper;
        public SharepointHelper(MsGraphApiConfig config) : base(config)
        {
            _siteHelper = new SiteHelper(config, null);
        }

        /// <summary>
        /// get folder information with folder url
        /// </summary>
        /// <param name="folderUrl"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public GetDriveItemResponse.Item GetFolder(string folderUrl)
        {
            string encodedUrl = Base64EncodeUrl(folderUrl);

            GetDriveItemResponse.Item item = null;

            string url = $"https://graph.microsoft.com/v1.0/shares/{encodedUrl}/driveItem";

            var response = DoGetAsync(async () =>
            {
                return await RestApi.GetAsync(url);

            }).GetAwaiter().GetResult();

            CheckStatusCode(response, RestApi.RetryResults);

            string content = response.Content.ReadAsStringAsync().Result;
            item = content.JsonToObject<GetDriveItemResponse.Item>();

            if (item == null)
            {
                throw new ArgumentException($"item not found: {folderUrl}");
            }

            if (string.IsNullOrEmpty(item.parentReference.siteId))
            {
                var site = _siteHelper.GetSiteByUrl(folderUrl);
                item.parentReference.siteId = site.id;
            }

            return item;
        }

        /// <summary>
        /// get item by item path, return null if not found https://learn.microsoft.com/en-us/graph/api/driveitem-get?view=graph-rest-1.0&tabs=http
        /// </summary>
        /// <param name="driveId">get from <see cref="GetFolder(string)"/></param>
        /// <param name="itemPath">e.g. /folder/folder or /folder/folder/file.txt</param>
        /// <returns></returns>
        public GetDriveItemResponse.Item GetItemByPath(string driveId, string itemPath)
        {
            string url = $"{GraphApiRootUrl}/drives/{driveId}/root:/{itemPath}";

            var response = RestApi.GetAsync(url).GetAwaiter().GetResult();

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            CheckStatusCode(response, RestApi.RetryResults);

            string content = response.Content.ReadAsStringAsync().Result;
            var responseObj = content.JsonToObject<GetDriveItemResponse.Item>();
            return responseObj;
        }

        public GetDriveItemResponse.Item UploadFile(string driveId, string folderItemId, string fileName, byte[] fileContent)
        {
            fileName = UtilityHelper.CleanItemName(fileName);

            // if file size < 4 MB, use normal upload, otherwise use upload session
            if (fileContent.Length < 4 * 1024 * 1024)
            {
                var children = GetChildren(driveId, folderItemId, null);

                string fileId = children.FirstOrDefault(p => p.name.Equals(fileName, StringComparison.CurrentCultureIgnoreCase))?.id;

                UploadSmallFileModel smallFileModel = new UploadSmallFileModel
                {
                    DriveId = driveId,
                    ParentId = folderItemId,
                    ItemId = fileId,
                    FileName = fileName,
                    FileContent = fileContent
                };

                return UploadSmallFile(smallFileModel);
            }
            else
            {
                UploadLargeFileModel largeFileModel = new UploadLargeFileModel
                {
                    DriveId = driveId,
                    ItemId = folderItemId,
                    FileName = fileName,
                    FileContent = fileContent
                };

                return UploadLargeFile(largeFileModel);
            }
        }

        private GetDriveItemResponse.Item UploadSmallFile(UploadSmallFileModel fileModel)
        {
            string fileName = UtilityHelper.CleanItemName(fileModel.FileName);
            byte[] fileContent = fileModel.FileContent;

            /*
                //upload new: /sites/{siteId}/drive/items/{driveItemId}:/{fileName}:/content
                //replace existing: /sites/{siteId}/drive/items/{driveItemId}/content

                replace: PUT /drives/{drive-id}/items/{item-id}/content
                new: PUT /drives/{drive-id}/items/{parent-id}:/{filename}:/content
                
             */
            string url = $"{GraphApiRootUrl}/drives/{fileModel.DriveId}/items";

            if (!string.IsNullOrEmpty(fileModel.ItemId))
            {
                url += $"/{fileModel.ItemId}/content";
            }
            else
            {
                url += $"/{fileModel.ParentId}:/{fileName}:/content";
            }

            HttpContent httpContent = new ByteArrayContent(fileContent);
            string contentType = MIMEType.GetContentType(fileName);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);

            var response = RestApi.PutAsync(url, httpContent).GetAwaiter().GetResult();

            // catch error to response with detail file information
            try
            {
                CheckStatusCode(response, RestApi.RetryResults);
            }
            catch (Exception ex)
            {
                throw new Exception($"Upload failed, fileName: {fileName}, Url: {url}", ex);
            }
            string content = response.Content.ReadAsStringAsync().Result;

            return content.JsonToObject<GetDriveItemResponse.Item>();
        }

        private GetDriveItemResponse.Item UploadLargeFile(UploadLargeFileModel fileModel)
        {
            CreateUploadSessionResponse.Response uploadSession = CreateUploadSession(fileModel);

            string url = uploadSession.uploadUrl;
            try
            {
                HttpContent httpContent = new ByteArrayContent(fileModel.FileContent);
                string contentType = MIMEType.GetContentType(UtilityHelper.CleanItemName(fileModel.FileName));
                httpContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                httpContent.Headers.ContentRange = new ContentRangeHeaderValue(0, fileModel.FileContent.Length - 1, fileModel.FileContent.Length);

                var response = RestApi.PutAsync(url, httpContent).GetAwaiter().GetResult();
                CheckStatusCode(response, RestApi.RetryResults);

                string content = response.Content.ReadAsStringAsync().Result;
                return content.JsonToObject<GetDriveItemResponse.Item>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Uploaded large file failed, file: '{fileModel.FileName}' ", ex);
            }
        }

        private CreateUploadSessionResponse.Response CreateUploadSession(UploadLargeFileModel fileModel)
        {
            string url = $"{GraphApiRootUrl}/drives/{fileModel.DriveId}/items/{fileModel.ItemId}:/{fileModel.FileName}:/createUploadSession";
            try
            {
                CreateUploadSessionRequest.Body requestBody = new CreateUploadSessionRequest.Body
                {
                    ConflictBehavior = ConflictBehaviorEnum.replace.ToString(),
                    Name = fileModel.FileName
                };

                HttpContent httpContent = GetJsonHttpContent(requestBody);
                var response = RestApi.PostAsync(url, httpContent).GetAwaiter().GetResult();
                CheckStatusCode(response, RestApi.RetryResults);

                string content = response.Content.ReadAsStringAsync().Result;
                return content.JsonToObject<CreateUploadSessionResponse.Response>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Create Upload Session faild, file '{fileModel.FileName}' ", ex);
            }
        }

        /// <summary>
        /// get children of <paramref name="itemId"/>, filter with <paramref name="filterExpression"/>
        /// </summary>
        /// <param name="driveId"></param>
        /// <param name="itemId"></param>
        /// <param name="filterExpression"></param>
        /// <returns></returns>
        public List<GetDriveItemResponse.Item> GetChildren(string driveId, string itemId, string filterExpression)
        {
            string url = $"{GraphApiRootUrl}/drives/{driveId}/items/{itemId}/children";

            if (!string.IsNullOrEmpty(filterExpression))
            {
                url = $"{url}?$filter={EscapeExpression(filterExpression)}";
            }

            List<GetDriveItemResponse.Item> childrenItems = new List<GetDriveItemResponse.Item>();

            var response = RestApi.GetAsync(url).GetAwaiter().GetResult();
            CheckStatusCode(response, RestApi.RetryResults);

            string content = response.Content.ReadAsStringAsync().Result;
            var responseObj = content.JsonToObject<GetDriveItemResponse.Response>();

            childrenItems = new List<GetDriveItemResponse.Item>();
            childrenItems.AddRange(responseObj.value);

            if (!string.IsNullOrEmpty(responseObj.odatanextLink))
            {
                _getnextlink(responseObj.odatanextLink, childrenItems);
            }

            return childrenItems;
        }

        private void _getnextlink(string nextLink, List<GetDriveItemResponse.Item> list)
        {
            var response = RestApi.GetAsync(nextLink).GetAwaiter().GetResult();
            CheckStatusCode(response, RestApi.RetryResults);

            string content = response.Content.ReadAsStringAsync().Result;
            var responseObj = content.JsonToObject<GetDriveItemResponse.Response>();

            list.AddRange(responseObj.value);

            if (!string.IsNullOrEmpty(responseObj.odatanextLink))
            {
                _getnextlink(responseObj.odatanextLink, list);
            }
        }

        public List<GetDriveItemResponse.Item> GetChildren(string folderAbsoluteUrl)
        {
            var folder = GetFolder(folderAbsoluteUrl);

            return GetChildren(folder.parentReference.driveId, folder.id, null);
        }

        /// <summary>
        /// get file content in byte[] by fileId
        /// </summary>
        /// <param name="folderAbsoluteUrl"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public byte[] GetFileContent(string folderAbsoluteUrl, string fileName)
        {
            var folder = GetFolder(folderAbsoluteUrl);
            var files = GetChildren(folderAbsoluteUrl);

            var file = files.FirstOrDefault(p => p.name.Equals(fileName, StringComparison.CurrentCultureIgnoreCase));

            string url = $"{GraphApiRootUrl}/drives/{folder.parentReference.driveId}/items/{file.id}/content";

            var response = RestApi.GetAsync(url).GetAwaiter().GetResult();
            CheckStatusCode(response, RestApi.RetryResults);

            byte[] content = response.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();

            return content;
        }

        public byte[] GetFileContent(string graphdownloadUrl)
        {
            var response = RestApi.GetAsync(graphdownloadUrl).GetAwaiter().GetResult();
            CheckStatusCode(response, RestApi.RetryResults);

            byte[] content = response.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();

            return content;
        }

        /// <summary>
        /// create subfolder
        /// </summary>
        /// <param name="driveId"></param>
        /// <param name="folderItemId">create new subfolder inside this folder</param>
        /// <param name="subFolderName">new subfolder name</param>
        /// <returns></returns>
        public GetDriveItemResponse.Item CreateFolder(string driveId, string folderItemId, string subFolderName)
        {
            CreateFolderRequest.Body model = new CreateFolderRequest.Body
            {
                Name = UtilityHelper.CleanItemName(subFolderName),
                ConflictBehavior = ConflictBehaviorEnum.replace.ToString(),
                Folder = new GetDriveItemResponse.Item.Folder()
            };

            string url = $"{GraphApiRootUrl}/drives/{driveId}/items/{folderItemId}/children";

            string jsonContent = model.ObjectToJson();

            HttpContent httpContent = new StringContent(jsonContent);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = RestApi.PostAsync(url, httpContent).GetAwaiter().GetResult();
            CheckStatusCode(response, RestApi.RetryResults);

            string content = response.Content.ReadAsStringAsync().Result;
            var item = content.JsonToObject<GetDriveItemResponse.Item>();
            return item;
        }

        /// <summary>
        /// move item into another folder
        /// </summary>
        /// <param name="sourceFileId"></param>
        /// <param name="targerFolderUrl"></param>
        /// <param name="targetFileName"></param>
        /// <returns></returns>
        public bool MoveFile(string sourceFileId, string targerFolderUrl, string targetFileName)
        {
            var folder = GetFolder(targerFolderUrl);

            string url = $"{GraphApiRootUrl}/drives/{folder.parentReference.driveId}/items/{sourceFileId}";

            MoveItemRequest.Body requestBody = new MoveItemRequest.Body
            {
                parentReference = new MoveItemRequest.ParentReference
                {
                    id = folder.id
                },
                name = targetFileName
            };

            string jsonContent = requestBody.ObjectToJson();

            HttpContent httpContent = new StringContent(jsonContent);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = RestApi.PutAsync(url, httpContent).GetAwaiter().GetResult();

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return true;
            }

            CheckStatusCode(response, RestApi.RetryResults);
            return false;
        }

        public bool DeleteItem(string folderUrl, string itemName)
        {
            var folder = GetFolder(folderUrl);
            var children = GetChildren(folderUrl).Where(p => p.name.IsEquals(itemName)).FirstOrDefault();

            if (children == null)
            {
                throw new Exception($"'{itemName}' not found");
            }

            string url = $"{GraphApiRootUrl}/drives/{folder.parentReference.driveId}/items/{children.id}";

            var response = RestApi.DeleteAsync(url).GetAwaiter().GetResult();
            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return true;
            }
            CheckStatusCode(response, RestApi.RetryResults);
            return false;
        }

        /// <summary>
        /// https://learn.microsoft.com/en-us/graph/api/driveitem-delete?view=graph-rest-1.0&tabs=http
        /// </summary>
        /// <param name="driveId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public bool DeleteDriveItem(string driveId, string itemId)
        {
            string url = $"{GraphApiRootUrl}/drives/{driveId}/items/{itemId}";

            var response = RestApi.DeleteAsync(url).GetAwaiter().GetResult();
            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return true;
            }
            CheckStatusCode(response, RestApi.RetryResults);
            return false;
        }



        /// <summary>
        /// rename sharepoint item
        /// </summary>
        /// <param name="folderUrl"></param>
        /// <param name="itemOldName"></param>
        /// <param name="itemNewName"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public GetDriveItemResponse.Item Rename(string folderUrl, string itemOldName, string itemNewName)
        {
            var folder = GetFolder(folderUrl);
            var children = GetChildren(folderUrl).FirstOrDefault(p => p.name.IsEquals(itemOldName));
            if (children == null)
            {
                throw new Exception($"'{itemOldName}' not found");
            }

            string url = $"{GraphApiRootUrl}/drives/{folder.parentReference.driveId}/items/{children.id}";

            string cleanName = UtilityHelper.CleanItemName(itemNewName);
            RenameItemRequest.Body requestBody = new RenameItemRequest.Body
            {
                Name = cleanName
            };
            string jsonContent = requestBody.ObjectToJson();
            HttpContent httpContent = new StringContent(jsonContent);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = RestApi.PutAsync(url, httpContent).GetAwaiter().GetResult();
            CheckStatusCode(response, RestApi.RetryResults);

            string content = response.Content.ReadAsStringAsync().Result;
            var item = content.JsonToObject<GetDriveItemResponse.Item>();

            return item;
        }

        /// <summary>
        /// rename sharepoint item
        /// </summary>
        /// <param name="driveId"></param>
        /// <param name="itemId"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public GetDriveItemResponse.Item RenameItem(string driveId, string itemId, string newName)
        {
            string url = $"{GraphApiRootUrl}/drives/{driveId}/items/{itemId}";

            string cleanName = UtilityHelper.CleanItemName(newName);
            RenameItemRequest.Body requestBody = new RenameItemRequest.Body
            {
                Name = cleanName
            };
            string jsonContent = requestBody.ObjectToJson();
            HttpContent httpContent = new StringContent(jsonContent);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = RestApi.PutAsync(url, httpContent).GetAwaiter().GetResult();
            CheckStatusCode(response, RestApi.RetryResults);

            string content = response.Content.ReadAsStringAsync().Result;
            var item = content.JsonToObject<GetDriveItemResponse.Item>();

            return item;
        }

        private static string Base64EncodeUrl(string url)
        {
            string base64Value = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(url));
            string encodedUrl = "u!" + base64Value.TrimEnd('=').Replace('/', '_').Replace('+', '-');
            return encodedUrl;
        }

        #region Permissions

        /// <summary>
        /// list permission of item
        /// </summary>
        /// <param name="driveId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public List<ListPermissionsResponse.Value> ListPermissions(string driveId, string itemId)
        {
            string url = $"{GraphApiRootUrl}/drives/{driveId}/items/{itemId}/permissions";

            var response = RestApi.GetAsync(url).GetAwaiter().GetResult();
            CheckStatusCode(response, RestApi.RetryResults);

            string content = response.Content.ReadAsStringAsync().Result;
            var item = content.JsonToObject<ListPermissionsResponse.Response>();

            return item.value;
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
            string url = $"{GraphApiRootUrl}/drives/{driveId}/items/{itemId}/invite";

            AddPermissionRequest.Body requestBody = new AddPermissionRequest.Body
            {
                recipients = new List<AddPermissionRequest.Recipient>
                    {
                        new AddPermissionRequest.Recipient {email = addPermissionModel.email}
                    },
                roles = new List<string> { addPermissionModel.role.ToString() },
                sendInvitation = addPermissionModel.sendInvitation,
                requireSignIn = true
            };
            string jsonContent = requestBody.ObjectToJson();
            HttpContent httpContent = new StringContent(jsonContent);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = RestApi.PostAsync(url, httpContent).GetAwaiter().GetResult();
            CheckStatusCode(response, RestApi.RetryResults);

            string content = response.Content.ReadAsStringAsync().Result;
            var item = content.JsonToObject<AddPermissionResponse.Response>();

            return item.value;
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
            string url = $"{GraphApiRootUrl}/drives/{driveId}/items/{itemId}/permissions/{permissionId}";

            var response = RestApi.DeleteAsync(url).GetAwaiter().GetResult();
            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return true;
            }
            CheckStatusCode(response, RestApi.RetryResults);
            return false;
        }
        #endregion
    }
}
