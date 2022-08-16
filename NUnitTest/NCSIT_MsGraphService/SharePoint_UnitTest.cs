using MicrosoftGraphService;
using MicrosoftGraphService.Config;
using NUnit.Framework;
using System;
using System.IO;

namespace NUnitTest.Liyang_MsGraphService
{
    public class SharePoint_UnitTest
    {
        string filename = $"{System.IO.Path.GetTempPath()}/Temp_{DateTime.Now.ToString("yyyyMMdd-HHmmss")}.txt";
        private string CreateTempFile(int sizeMB)
        {
            long filesize = 0;
            if (!File.Exists(filename))
            {
                File.AppendAllText(filename, "Test");
            }

            filesize = new FileInfo(filename).Length;

            while (filesize < sizeMB * 1024 * 1024)
            {
                string text = File.ReadAllText(filename);
                File.AppendAllText(filename, text);
                filesize = new FileInfo(filename).Length;
            }

            return filename;
        }

        [Test]
        public void SharePoint_UploadLargeFile_Test()
        {
            MsGraphApiConfig graphConfig = new MsGraphApiConfig
            {
                Proxy_Url = "",
                Proxy_Username = "",
                Proxy_Password = "",
                Token = ""
            };

            MsGraphService.SharePoint graph = new MsGraphService.SharePoint(graphConfig);

            string fileNameAndPath = CreateTempFile(5);

            string hostName = "";
            string siteName = "";
            string folderUrl = "";
            string fileName = Path.GetFileName(fileNameAndPath);
            byte[] fileContent = File.ReadAllBytes(fileNameAndPath);

            Assert.DoesNotThrow(() => {
                var result = graph.UploadFileByFolderUrl(hostName, siteName, folderUrl, fileName, fileContent);

                string filename = Path.GetFileName(fileNameAndPath);
                Assert.AreEqual(filename, result.Name);
            });
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }
        }
    }
}
