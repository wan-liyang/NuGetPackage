using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using TryIT.MicrosoftGraphApi.Helper;
using TryIT.MicrosoftGraphApi.Model.Sharepoint;
using TryIT.MicrosoftGraphApi.Request.Sharepoint;
using TryIT.MicrosoftGraphApi.Response.Sharepoint;

namespace TryIT.MicrosoftGraphApi.HttpClientHelper
{
    internal class SharepointHelper : BaseHelper
    {
        private TryIT.RestApi.Api api;
        private readonly HttpClient _httpClient;

        public SharepointHelper(HttpClient httpClient)
        {
            if (null == httpClient)
                throw new ArgumentNullException(nameof(httpClient));

            // use RestApi library and enable retry
            api = new RestApi.Api(new RestApi.Models.ApiConfig
            {
                HttpClient = httpClient,
                EnableRetry = true,
            });

            _httpClient = httpClient;
        }

        /// <summary>
        /// get folder information with folder url
        /// </summary>
        /// <param name="folderUrl"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public GetDriveItemResponse.Item GetFolder(string folderUrl)
        {
            string encodedUrl = Base64EncodeUrl(folderUrl);

            GetDriveItemResponse.Item item = null;

            string url = $"https://graph.microsoft.com/v1.0/shares/{encodedUrl}/driveItem";

            try
            {
                var response = api.GetAsync(url).GetAwaiter().GetResult();
                CheckStatusCode(response, api.RetryResults);

                string content = response.Content.ReadAsStringAsync().Result;
                item = content.JsonToObject<GetDriveItemResponse.Item>();
            }
            catch
            {
                throw;
            }

            if (item == null)
            {
                throw new Exception($"item not found: {folderUrl}");
            }

            if (string.IsNullOrEmpty(item.parentReference.siteId))
            {
                var site = new SiteHelper(_httpClient).GetSiteByUrl(folderUrl);
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

            try
            {
                var response = api.GetAsync(url).GetAwaiter().GetResult();

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }

                CheckStatusCode(response, api.RetryResults);

                string content = response.Content.ReadAsStringAsync().Result;
                var responseObj = content.JsonToObject<GetDriveItemResponse.Item>();
                return responseObj;
            }
            catch
            {
                throw;
            }
        }

        public GetDriveItemResponse.Item UploadFile(string folderAbsoluteUrl, string fileName, byte[] fileContent)
        {
            fileName = UtilityHelper.CleanItemName(fileName);

            var sharepointFolder = GetFolder(folderAbsoluteUrl);

            // if file size < 4 MB, use normal upload, otherwise use upload session
            if (fileContent.Length < 4 * 1024 * 1024)
            {
                string siteId = sharepointFolder.parentReference.siteId;
                var children = GetChildren(folderAbsoluteUrl);

                string fileId = children.Where(p => p.name.Equals(fileName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault()?.id;

                UploadSmallFileModel smallFileModel = new UploadSmallFileModel
                {
                    DriveId = sharepointFolder.parentReference.driveId,
                    ParentId = sharepointFolder.id,
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
                    DriveId = sharepointFolder.parentReference.driveId,
                    ItemId = sharepointFolder.id,
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

            try
            {
                HttpContent httpContent = new ByteArrayContent(fileContent);
                string contentType = MIMEType.GetContentType(fileName);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);

                var response = api.PutAsync(url, httpContent).GetAwaiter().GetResult();

                // catch error to response with detail file information
                try
                {
                    CheckStatusCode(response, api.RetryResults);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Upload failed, fileName: {fileName}, Url: {url}", ex);
                }
                string content = response.Content.ReadAsStringAsync().Result;

                return content.JsonToObject<GetDriveItemResponse.Item>();
            }
            catch
            {
                throw;
            }
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

                var response = api.PutAsync(url, httpContent).GetAwaiter().GetResult();
                CheckStatusCode(response, api.RetryResults);

                string content = response.Content.ReadAsStringAsync().Result;
                return content.JsonToObject<GetDriveItemResponse.Item>();
            }
            catch(Exception ex)
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

                HttpContent httpContent = this.GetJsonHttpContent(requestBody);
                var response = api.PostAsync(url, httpContent).GetAwaiter().GetResult();
                CheckStatusCode(response, api.RetryResults);

                string content = response.Content.ReadAsStringAsync().Result;
                return content.JsonToObject<CreateUploadSessionResponse.Response>();
            }
            catch(Exception ex)
            {
                throw new Exception($"Create Upload Session faild, file '{fileModel.FileName}' ", ex);
            }
        }


        private List<GetDriveItemResponse.Item> ChildrenItems;
        public List<GetDriveItemResponse.Item> GetChildren(string folderAbsoluteUrl)
        {
            var folder = GetFolder(folderAbsoluteUrl);

            string url = $"{GraphApiRootUrl}/drives/{folder.parentReference.driveId}/items/{folder.id}/children";

            try
            {
                var response = api.GetAsync(url).GetAwaiter().GetResult();
                CheckStatusCode(response, api.RetryResults);

                string content = response.Content.ReadAsStringAsync().Result;
                var responseObj = content.JsonToObject<GetDriveItemResponse.Response>();

                ChildrenItems = new List<GetDriveItemResponse.Item>();
                ChildrenItems.AddRange(responseObj.value);

                if (!string.IsNullOrEmpty(responseObj.odatanextLink))
                {
                    GetChildrenNextLink(responseObj.odatanextLink);
                }

                return ChildrenItems;
            }
            catch
            {
                throw;
            }
        }

        public void GetChildrenNextLink(string nextLink)
        {
            try
            {
                var response = api.GetAsync(nextLink).GetAwaiter().GetResult();
                CheckStatusCode(response, api.RetryResults);

                string content = response.Content.ReadAsStringAsync().Result;
                var responseObj = content.JsonToObject<GetDriveItemResponse.Response>();

                ChildrenItems.AddRange(responseObj.value);

                if (!string.IsNullOrEmpty(responseObj.odatanextLink))
                {
                    GetChildrenNextLink(responseObj.odatanextLink);
                }
            }
            catch
            {
                throw;
            }
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

            var file = files.Where(p => p.name.Equals(fileName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

            string url = $"{GraphApiRootUrl}/drives/{folder.parentReference.driveId}/items/{file.id}/content";
            try
            {
                // for resolve "The underlying connection was closed: An unexpected error occurred on a send" error
                // ServicePointManager.SecurityProtocol = GetSecurityProtocol();

                var response = api.GetAsync(url).GetAwaiter().GetResult();
                CheckStatusCode(response, api.RetryResults);

                byte[] content = response.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();

                return content;
            }
            catch
            {
                throw;
            }
        }

        public byte[] GetFileContent(string graphdownloadUrl)
        {
            try
            {
                var response = api.GetAsync(graphdownloadUrl).GetAwaiter().GetResult();
                CheckStatusCode(response, api.RetryResults);

                byte[] content = response.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();

                return content;
            }
            catch
            {
                throw;
            }
        }

        public GetDriveItemResponse.Item CreateFolder(string folderAbsoluteUrl, string subFolderName)
        {
            CreateFolderRequest.Body model = new CreateFolderRequest.Body
            {
                Name = UtilityHelper.CleanItemName(subFolderName),
                ConflictBehavior = ConflictBehaviorEnum.replace.ToString(),
                Folder = new GetDriveItemResponse.Item.Folder()
            };

            var folder = GetFolder(folderAbsoluteUrl);
            string url = $"{GraphApiRootUrl}/drives/{folder.parentReference.driveId}/items/{folder.id}/children";

            try
            {
                string jsonContent = model.ObjectToJson();

                HttpContent httpContent = new StringContent(jsonContent);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = api.PostAsync(url, httpContent).GetAwaiter().GetResult();
                CheckStatusCode(response, api.RetryResults);

                string content = response.Content.ReadAsStringAsync().Result;
                var item = content.JsonToObject<GetDriveItemResponse.Item>();
                return item;
            }
            catch
            {
                throw;
            }
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

            try
            {
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

                var response = api.PutAsync(url, httpContent).GetAwaiter().GetResult();

                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    return true;
                }

                CheckStatusCode(response, api.RetryResults);
                return false;
            }
            catch
            {
                throw;
            }
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

            try
            {
                var response = api.DeleteAsync(url).GetAwaiter().GetResult();
                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    return true;
                }
                CheckStatusCode(response, api.RetryResults);
                return false;
            }
            catch
            {
                throw;
            }
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
            var children = GetChildren(folderUrl).Where(p => p.name.IsEquals(itemOldName)).FirstOrDefault();
            if (children == null)
            {
                throw new Exception($"'{itemOldName}' not found");
            }

            string url = $"{GraphApiRootUrl}/drives/{folder.parentReference.driveId}/items/{children.id}";

            try
            {
                string cleanName = UtilityHelper.CleanItemName(itemNewName);
                RenameItemRequest.Body requestBody = new RenameItemRequest.Body
                {
                    Name = cleanName
                };
                string jsonContent = requestBody.ObjectToJson();
                HttpContent httpContent = new StringContent(jsonContent);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = api.PutAsync(url, httpContent).GetAwaiter().GetResult();
                CheckStatusCode(response, api.RetryResults);

                string content = response.Content.ReadAsStringAsync().Result;
                var item = content.JsonToObject<GetDriveItemResponse.Item>();

                return item;
            }
            catch
            {
                throw;
            }
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

            try
            {
                string cleanName = UtilityHelper.CleanItemName(newName);
                RenameItemRequest.Body requestBody = new RenameItemRequest.Body
                {
                    Name = cleanName
                };
                string jsonContent = requestBody.ObjectToJson();
                HttpContent httpContent = new StringContent(jsonContent);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = api.PutAsync(url, httpContent).GetAwaiter().GetResult();
                CheckStatusCode(response, api.RetryResults);

                string content = response.Content.ReadAsStringAsync().Result;
                var item = content.JsonToObject<GetDriveItemResponse.Item>();

                return item;
            }
            catch
            {
                throw;
            }
        }

        private string Base64EncodeUrl(string url)
        {
            string base64Value = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(url));
            string encodedUrl = "u!" + base64Value.TrimEnd('=').Replace('/', '_').Replace('+', '-');
            return encodedUrl;
        }

        #region Permissions
        /// <summary>
        /// list permission of item
        /// </summary>
        /// <param name="folderUrl"></param>
        /// <returns></returns>
        public List<ListPermissionsResponse.Value> ListPermissions(string folderUrl)
        {
            var folder = GetFolder(folderUrl);
            string url = $"{GraphApiRootUrl}/drives/{folder.parentReference.driveId}/items/{folder.id}/permissions";

            try
            {
                var response = api.GetAsync(url).GetAwaiter().GetResult();
                CheckStatusCode(response, api.RetryResults);

                string content = response.Content.ReadAsStringAsync().Result;
                var item = content.JsonToObject<ListPermissionsResponse.Response>();

                return item.value;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// add permission
        /// </summary>
        /// <param name="folderUrl"></param>
        /// <param name="addPermissionModel"></param>
        /// <returns></returns>
        public List<AddPermissionResponse.Value> AddPermissions(string folderUrl, AddPermissionModel addPermissionModel)
        {
            var folder = GetFolder(folderUrl);
            string url = $"{GraphApiRootUrl}/drives/{folder.parentReference.driveId}/items/{folder.id}/invite";

            try
            {
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

                var response = api.PostAsync(url, httpContent).GetAwaiter().GetResult();
                CheckStatusCode(response, api.RetryResults);

                string content = response.Content.ReadAsStringAsync().Result;
                var item = content.JsonToObject<AddPermissionResponse.Response>();

                return item.value;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// delete permission
        /// </summary>
        /// <param name="folderUrl"></param>
        /// <param name="permissionId">the id of permission object, can get from <see cref="ListPermissions(string)"/></param>
        /// <returns></returns>
        public bool DeletePermission(string folderUrl, string permissionId)
        {
            var folder = GetFolder(folderUrl);
            string url = $"{GraphApiRootUrl}/drives/{folder.parentReference.driveId}/items/{folder.id}/permissions/{permissionId}";

            try
            {
                var response = api.DeleteAsync(url).GetAwaiter().GetResult();
                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    return true;
                }
                CheckStatusCode(response, api.RetryResults);
                return false;
            }
            catch
            {
                throw;
            }
        }
        #endregion
    }
}
