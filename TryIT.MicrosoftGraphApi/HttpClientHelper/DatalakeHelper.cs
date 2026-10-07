using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TryIT.MicrosoftGraphApi.Helper;
using TryIT.MicrosoftGraphApi.Model;
using TryIT.MicrosoftGraphApi.Model.Datalake;

namespace TryIT.MicrosoftGraphApi.HttpClientHelper
{
    internal class DatalakeHelper : BaseHelper
    {
        public DatalakeHelper(MsGraphApiConfig config) : base(config) { }

        public async Task<Stream> GetFileAsync(GetFileRequest getFileRequest)
        {
            var url =
            $"https://{getFileRequest.InstanceName}.dfs.core.windows.net/" +
            $"{getFileRequest.FilePath.TrimStart('/')}";

            var response = await RestApi.GetAsync(url);
            CheckStatusCode(response);

            return await response.Content.ReadAsStreamAsync();
        }

        public async Task UploadFileAsync(UploadFileRequest uploadFileRequest)
        {
            var url =
            $"https://{uploadFileRequest.InstanceName}.dfs.core.windows.net/" +
            $"{uploadFileRequest.FilePath.TrimStart('/')}";

            // ---------------------------------------------------------
            // 1. Create the file
            // ---------------------------------------------------------

            string createFileUrl = url + "?resource=file";
            var createFileResponse = await RestApi.PutAsync(createFileUrl, null);
            CheckStatusCode(createFileResponse);


            // ---------------------------------------------------------
            // 2. Append file content
            // ---------------------------------------------------------

            // ensure stream is at beginning when possible
            if (uploadFileRequest.FileStream.CanSeek)
            {
                uploadFileRequest.FileStream.Position = 0;
            }

            long position = 0;
            const int bufferSize = 4 * 1024 * 1024; // 4 MB
            var buffer = new byte[bufferSize];
            int bytesRead;

            while ((bytesRead = await uploadFileRequest.FileStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                using (var content = new ByteArrayContent(buffer, 0, bytesRead))
                {
                    string appendFileUrl = url + $"?action=append&position={position}";

                    HttpContent httpContent = content;
                    httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                    var appendFileResponse = await RestApi.PatchAsync(appendFileUrl, httpContent);
                    CheckStatusCode(appendFileResponse);
                }

                position += bytesRead;
            }

            // ---------------------------------------------------------
            // 3. Flush the file to commit appended data
            // ---------------------------------------------------------

            string flushFileUrl = url + $"?action=flush&position={position}";
            // flush doesn't need a body
            var flushResponse = await RestApi.PatchAsync(flushFileUrl, null);
            CheckStatusCode(flushResponse);
        }
    }
}
