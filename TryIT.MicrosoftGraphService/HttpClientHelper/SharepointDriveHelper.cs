using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using TryIT.MicrosoftGraphService.ApiModel;
using TryIT.MicrosoftGraphService.ApiModel.Sharepoint;
using TryIT.MicrosoftGraphService.ExtensionHelper;
using static TryIT.MicrosoftGraphService.ApiModel.SharePointResponse;

namespace TryIT.MicrosoftGraphService.HttpClientHelper
{
    internal class SharepointDriveHelper : BaseHelper
    {
        private readonly string GraphApiRootUrl = "https://graph.microsoft.com/v1.0";
        private HttpClient _httpClient;

        public SharepointDriveHelper(HttpClient httpClient)
        {
            if (null == httpClient)
                throw new ArgumentNullException(nameof(httpClient));

            _httpClient = httpClient;
        }

        public GetDriveItemResponse GetFolder(string folderAbsoluteUrl)
        {
            string encodedUrl = Base64EncodeUrl(folderAbsoluteUrl);

            string url = $"https://graph.microsoft.com/v1.0/shares/{encodedUrl}/driveItem";

            try
            {
                var response = _httpClient.GetAsync(url).GetAwaiter().GetResult();
                CheckStatusCode(response);

                string content = response.Content.ReadAsStringAsync().Result;
                return content.JsonToObject<GetDriveItemResponse>();
            }
            catch
            {
                throw;
            }
        }

        public GetDriveItemResponse UploadFile(string folderAbsoluteUrl, string fileName, byte[] fileContent)
        {
            var sharepointFolder = GetFolder(folderAbsoluteUrl);

            // if file size < 4 MB, use normal upload, otherwise use upload session
            if (fileContent.Length < 4 * 1024 * 1024)
            {
                string siteId = sharepointFolder.parentReference.siteId;
                var children = GetChildren(folderAbsoluteUrl);

                string fileId = children.value.Where(p => p.name.Equals(fileName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault()?.id;

                UploadSmallFileModel smallFileModel = new UploadSmallFileModel
                {
                    SiteId = siteId,
                    ItemId = sharepointFolder.id,
                    FileName = fileName,
                    FileContent = fileContent,
                    FileId = fileId
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

        private GetDriveItemResponse UploadSmallFile(UploadSmallFileModel fileModel)
        {
            string siteId = fileModel.SiteId;
            string itemId = fileModel.ItemId;
            string fileName = CleanFileName(fileModel.FileName);
            byte[] fileContent = fileModel.FileContent;

            /*
                upload new: /sites/{siteId}/drive/items/{driveItemId}:/{fileName}:/content
                replace existing: /sites/{siteId}/drive/items/{driveItemId}/content
             */
            string url = $"https://graph.microsoft.com/v1.0/sites/{siteId}/drive/items";

            if (!string.IsNullOrEmpty(fileModel.FileId))
            {
                url += $"/{fileModel.FileId}/content";
            }
            else
            {
                url += $"/{itemId}:/{fileName}:/content";
            }

            try
            {
                HttpContent httpContent = new ByteArrayContent(fileContent);
                string contentType = MIMEType.GetContentType(fileName);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);

                var response = _httpClient.PutAsync(url, httpContent).GetAwaiter().GetResult();

                // catch error to response with detail file information
                try
                {
                    CheckStatusCode(response);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Upload failed, fileName: {fileName}, Url: {url}", ex);
                }
                string content = response.Content.ReadAsStringAsync().Result;

                return content.JsonToObject<GetDriveItemResponse>();
            }
            catch
            {
                throw;
            }
        }

        private GetDriveItemResponse UploadLargeFile(UploadLargeFileModel fileModel)
        {
            CreateUploadSessionResponse uploadSession = null;
            string url = $"https://graph.microsoft.com/v1.0/drives/{fileModel.DriveId}/items/{fileModel.ItemId}:/{fileModel.FileName}:/createUploadSession";
            try
            {
                CreateUploadSessionRequestBody requestBody = new CreateUploadSessionRequestBody
                {
                    ConflictBehavior = ConflictBehavior.replace.ToString(),
                    Name = fileModel.FileName
                };

                HttpContent httpContent = this.GetJsonHttpContent(requestBody);
                var response = _httpClient.PostAsync(url, httpContent).GetAwaiter().GetResult();
                CheckStatusCode(response);

                string content = response.Content.ReadAsStringAsync().Result;
                uploadSession = content.JsonToObject<CreateUploadSessionResponse>();
            }
            catch
            {
                throw;
            }


            url = uploadSession.uploadUrl;
            try
            {
                HttpContent httpContent = new ByteArrayContent(fileModel.FileContent);
                string contentType = MIMEType.GetContentType(CleanFileName(fileModel.FileName));
                httpContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                httpContent.Headers.ContentRange = new ContentRangeHeaderValue(0, fileModel.FileContent.Length - 1, fileModel.FileContent.Length);

                var response = _httpClient.PutAsync(url, httpContent).GetAwaiter().GetResult();
                CheckStatusCode(response);

                string content = response.Content.ReadAsStringAsync().Result;
                return content.JsonToObject<GetDriveItemResponse>();
            }
            catch
            {
                throw;
            }
        }

        public GetDriveItemListResponse GetChildren(string folderAbsoluteUrl)
        {
            var folder = GetFolder(folderAbsoluteUrl);

            string url = $"https://graph.microsoft.com/v1.0/drives/{folder.parentReference.driveId}/items/{folder.id}/children";

            try
            {
                var response = _httpClient.GetAsync(url).GetAwaiter().GetResult();
                CheckStatusCode(response);

                string content = response.Content.ReadAsStringAsync().Result;
                return content.JsonToObject<GetDriveItemListResponse>();
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

            var file = files.value.Where(p => p.name.Equals(fileName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

            string url = $"{GraphApiRootUrl}/drives/{folder.parentReference.driveId}/items/{file.id}/content";
            try
            {
                // for resolve "The underlying connection was closed: An unexpected error occurred on a send" error
                // ServicePointManager.SecurityProtocol = GetSecurityProtocol();

                var response = _httpClient.GetAsync(url).GetAwaiter().GetResult();
                CheckStatusCode(response);

                byte[] content = response.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();

                return content;
            }
            catch
            {
                throw;
            }
        }

        public GetDriveItemResponse CreateFolder(string folderAbsoluteUrl, string subFolderName)
        {
            SharePointCreateFolderModel model = new SharePointCreateFolderModel
            {
                Name = subFolderName,
                ConflictBehavior = ConflictBehavior.replace.ToString(),
                Folder = new GetDriveItemResponse.Folder()
            };

            var folder = GetFolder(folderAbsoluteUrl);
            string url = $"{GraphApiRootUrl}/drives/{folder.parentReference.driveId}/items/{folder.id}/children";

            try
            {
                string jsonContent = model.ObjectToJson();

                HttpContent httpContent = new StringContent(jsonContent);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = _httpClient.PostAsync(url, httpContent).GetAwaiter().GetResult();
                CheckStatusCode(response);

                string content = response.Content.ReadAsStringAsync().Result;
                var item = content.JsonToObject<GetDriveItemResponse>();
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
                MoveItemRequestBody requestBody = new MoveItemRequestBody
                {
                    parentReference = new ParentReference
                    {
                        id = folder.id
                    },
                    name = targetFileName
                };

                string jsonContent = requestBody.ObjectToJson();

                HttpContent httpContent = new StringContent(jsonContent);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = _httpClient.PutAsync(url, httpContent).GetAwaiter().GetResult();

                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    return true;
                }

                CheckStatusCode(response);
                return false;
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

        private string CleanFileName(string filename)
        {
            return filename.Replace("#", "_");
        }
    }
}
