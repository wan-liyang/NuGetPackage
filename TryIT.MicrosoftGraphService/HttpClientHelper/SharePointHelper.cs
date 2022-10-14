using TryIT.MicrosoftGraphService.ExtensionHelper;
using TryIT.MicrosoftGraphService.ApiModel;
using TryIT.MicrosoftGraphService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using static TryIT.MicrosoftGraphService.ApiModel.SharePointResponse;

namespace TryIT.MicrosoftGraphService.HttpClientHelper
{
    internal class SharePointHelper : BaseHelper
    {
        private HttpClient _httpClient;

        public SharePointHelper(HttpClient httpClient)
        {
            if (null == httpClient)
                throw new ArgumentNullException(nameof(httpClient));

            _httpClient = httpClient;
        }

        /// <summary>
        /// get site info, https://graph.microsoft.com/v1.0/sites/{hostname}:/sites/{sitename}
        /// </summary>
        /// <param name="hostname"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public SharePointResponse.GetSiteResponse GetSite(string hostName, string siteName)
        {
            string url = $"https://graph.microsoft.com/v1.0/sites/{hostName}:/sites/{siteName}";

            try
            {
                HttpResponseMessage response = _httpClient.GetAsync(url).GetAwaiter().GetResult();

                string content = response.Content.ReadAsStringAsync().Result;

                return content.JsonToObject<SharePointResponse.GetSiteResponse>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public GetDriveItemResponse GetFolderByUrl(string folderAbsoluteUrl)
        {
            string encodedUrl = Base64EncodeUrl(folderAbsoluteUrl);

            string url = $"https://graph.microsoft.com/v1.0/shares/{encodedUrl}/driveItem";

            try
            {
                var response = _httpClient.GetAsync(url).GetAwaiter().GetResult();

                string content = response.Content.ReadAsStringAsync().Result;

                return content.JsonToObject<GetDriveItemResponse>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public GetDriveItemResponse CreateFolder(string siteId, string parentFolderId, SharePointCreateFolderModel siteCreateFolderModule)
        {
            string url = $"https://graph.microsoft.com/v1.0/sites/{siteId}/drive/items/{parentFolderId}/children";

            try
            {
                string jsonContent = siteCreateFolderModule.ObjectToJson();

                HttpContent httpContent = new StringContent(jsonContent);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = _httpClient.PostAsync(url, httpContent).GetAwaiter().GetResult();
                CheckStatusCode(response);
                string content = response.Content.ReadAsStringAsync().Result;

                var item = content.JsonToObject<GetDriveItemResponse>();
                return item;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// get specific file under the folder
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="driveItemId"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public GetDriveItemResponse GetFileByName(string siteId, string driveItemId, string fileName)
        {
            string url = $"https://graph.microsoft.com/v1.0/sites/{siteId}/drive/items/{driveItemId}/children?$filter=name eq '{fileName}'";

            try
            {
                var response = _httpClient.GetAsync(url).GetAwaiter().GetResult();

                string content = response.Content.ReadAsStringAsync().Result;

                // this api will return list, even only one item found
                var list = content.JsonToObject<GetDriveItemListResponse>();

                if (list.value != null && list.value.Count > 0)
                {
                    return list.value.FirstOrDefault();
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// get SharePoint DirveItem by Id, <paramref name="itemId"/> is SharePoint DriveItem Unique Id 
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public GetDriveItemResponse GetItemById(string siteId, string itemId)
        {
            string url = $"https://graph.microsoft.com/v1.0/sites/{siteId}/drive/items/{itemId}";

            try
            {
                var response = _httpClient.GetAsync(url).GetAwaiter().GetResult();
                CheckStatusCode(response);
                string content = response.Content.ReadAsStringAsync().Result;

                return content.JsonToObject<GetDriveItemResponse>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// create Preview link for drive item, it can be use in iframe
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public GetDriveItemPreviewResponse CreateItemPreviewLink(string siteId, string itemId)
        {
            string url = $"https://graph.microsoft.com/v1.0/sites/{siteId}/drive/items/{itemId}/preview";

            try
            {
                var response = _httpClient.PostAsync(url, null).GetAwaiter().GetResult();
                CheckStatusCode(response);
                string content = response.Content.ReadAsStringAsync().Result;

                return content.JsonToObject<GetDriveItemPreviewResponse>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// upload file into sharepoint, if file size less than 4 MB, use normal upload, otherwise use upload session 
        /// </summary>
        /// <param name="fileModule"></param>
        /// <returns></returns>
        public GetDriveItemResponse UploadFile(SharePointModel.SiteUploadFileModel fileModule)
        {
            // if file size < 4 MB, use normal upload, otherwise use upload session
            if (fileModule.FileContent.Length < 4 * 1024 * 1024)
            {
                return UploadNormalFile(fileModule);
            }
            else
            {
                return UploadLargeFile(fileModule);
            }
        }

        /// <summary>
        /// upload file into sharepoint
        /// <para>https://docs.microsoft.com/en-us/graph/api/driveitem-put-content?view=graph-rest-1.0&tabs=http</para>
        /// </summary>
        /// <param name="fileModule"></param>
        /// <returns></returns>
        private GetDriveItemResponse UploadNormalFile(SharePointModel.SiteUploadFileModel fileModule)
        {
            string siteId = fileModule.SiteId;
            string driveItemId = fileModule.DriveItemId;
            string fileName = fileModule.FileName;
            byte[] fileContent = fileModule.FileContent;

            var file = GetFileByName(siteId, driveItemId, fileName);

            /*
                upload new: /sites/{siteId}/drive/items/{driveItemId}:/{fileName}:/content
                replace existing: /sites/{siteId}/drive/items/{driveItemId}/content
             */
            string url = $"https://graph.microsoft.com/v1.0/sites/{siteId}/drive/items";

            if (file != null && !string.IsNullOrEmpty(file.id))
            {
                url += $"/{file.id}/content";
            }
            else
            {
                url += $"/{driveItemId}:/{fileName}:/content";
            }

            try
            {
                HttpContent httpContent = new ByteArrayContent(fileContent);
                string contentType = MIMEType.GetContentType(fileName);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);

                var response = _httpClient.PutAsync(url, httpContent).GetAwaiter().GetResult();
                CheckStatusCode(response);
                string content = response.Content.ReadAsStringAsync().Result;

                return content.JsonToObject<GetDriveItemResponse>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// if file over 4 MB, use resumable upload session
        /// <para>https://docs.microsoft.com/en-us/graph/api/driveitem-createuploadsession?view=graph-rest-1.0</para>
        /// </summary>
        /// <param name="fileModule"></param>
        private GetDriveItemResponse UploadLargeFile(SharePointModel.SiteUploadFileModel fileModule)
        {
            string siteId = fileModule.SiteId;
            string driveItemId = fileModule.DriveItemId;
            string filename = fileModule.FileName;

            CreateUploadSessionRequestBody requestBody = new CreateUploadSessionRequestBody
            {
                ConflictBehavior = ConflictBehavior.replace.ToString(),
                Name = filename
            };

            CreateUploadSessionResponse response = CreateUploadSession(siteId, driveItemId, requestBody);

            return UploadFileWithUploadSession(response, fileModule);
        }
        private CreateUploadSessionResponse CreateUploadSession(string siteId, string driveItemId, CreateUploadSessionRequestBody requestBody)
        {
            string url = $"https://graph.microsoft.com/v1.0/sites/{siteId}/drive/items/{driveItemId}:/{requestBody.Name}:/createUploadSession";

            try
            {
                HttpContent httpContent = this.GetJsonHttpContent(requestBody);
                var response = _httpClient.PostAsync(url, httpContent).GetAwaiter().GetResult();
                CheckStatusCode(response);
                string content = response.Content.ReadAsStringAsync().Result;

                return content.JsonToObject<CreateUploadSessionResponse>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private GetDriveItemResponse UploadFileWithUploadSession(CreateUploadSessionResponse uploadSession, SharePointModel.SiteUploadFileModel fileModule)
        {
            string url = uploadSession.uploadUrl;

            try
            {
                HttpContent httpContent = new ByteArrayContent(fileModule.FileContent);
                string contentType = MIMEType.GetContentType(fileModule.FileName);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                httpContent.Headers.ContentRange = new ContentRangeHeaderValue(0, fileModule.FileContent.Length - 1, fileModule.FileContent.Length);

                var response = _httpClient.PutAsync(url, httpContent).GetAwaiter().GetResult();
                CheckStatusCode(response);
                string content = response.Content.ReadAsStringAsync().Result;

                return content.JsonToObject<GetDriveItemResponse>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// delete SharePoint DirveItem by Id, <paramref name="itemId"/> is SharePoint DriveItem Unique Id 
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public bool DeleteItemById(string siteId, string itemId)
        {
            string url = $"https://graph.microsoft.com/v1.0/sites/{siteId}/drive/items/{itemId}";

            try
            {
                var response = _httpClient.DeleteAsync(url).GetAwaiter().GetResult();

                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// get file content in byte[] by fileId
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public byte[] GetFileContent(string siteId, string fileId)
        {
            string url = $"https://graph.microsoft.com/v1.0/sites/{siteId}/drive/items/{fileId}/content";

            try
            {
                // for resolve "The underlying connection was closed: An unexpected error occurred on a send" error
                ServicePointManager.SecurityProtocol = GetSecurityProtocol();

                var response = _httpClient.GetAsync(url).GetAwaiter().GetResult();

                byte[] content = response.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();

                return content;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static SecurityProtocolType GetSecurityProtocol()
        {
            /*
             * force to use Tls12 protocol

                Insecure Transport: Weak SSL Protocol (4 issues)

                Abstract
                The SSLv2, SSLv23, SSLv3, TLSv1.0 and TLSv1.1 protocols contain flaws that make them insecure and
                should not be used to transmit sensitive data.

                Explanation
                The Transport Layer Security (TLS) and Secure Sockets Layer (SSL) protocols provide a protection
                mechanism to ensure the authenticity, confidentiality, and integrity of data transmitted between a client and
                web server. Both TLS and SSL have undergone revisions resulting in periodic version updates. Each new
                revision is designed to address the security weaknesses discovered in previous versions. Use of an
                insecure version of TLS/SSL weakens the data protection strength and might allow an attacker to
                compromise, steal, or modify sensitive information.
                Weak versions of TLS/SSL might exhibit one or more of the following properties:
                - No protection against man-in-the-middle attacks - Same key used for authentication and encryption -
                Weak message authentication control - No protection against TCP connection closing - Use of weak cipher
                suites
                The presence of these properties might allow an attacker to intercept, modify, or tamper with sensitive data.

                Recommendation
                Fortify highly recommends forcing the client to use only the most secure protocols.                 
            */

            return SecurityProtocolType.Tls12;
        }


        public GetDriveItemListResponse GetItemsByUrl(string folderAbsoluteUrl)
        {
            string encodedUrl = Base64EncodeUrl(folderAbsoluteUrl);

            string url = $"https://graph.microsoft.com/v1.0/shares/{encodedUrl}/driveItem/children";

            try
            {
                var response = _httpClient.GetAsync(url).GetAwaiter().GetResult();

                string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                return content.JsonToObject<GetDriveItemListResponse>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public GetDriveItemResponse GetFileByUrl(string fileAbsoluteUrl)
        {
            string encodedUrl = Base64EncodeUrl(fileAbsoluteUrl);

            string url = $"https://graph.microsoft.com/v1.0/shares/{encodedUrl}/driveItem";

            try
            {
                var response = _httpClient.GetAsync(url).GetAwaiter().GetResult();

                string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                return content.JsonToObject<GetDriveItemResponse>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string Base64EncodeUrl(string url)
        {
            string base64Value = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(url));
            string encodedUrl = "u!" + base64Value.TrimEnd('=').Replace('/', '_').Replace('+', '-');
            return encodedUrl;
        }
    }
}
