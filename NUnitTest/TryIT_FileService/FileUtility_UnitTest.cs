using TryIT.FileService;
using NUnit.Framework;
using System;
using System.IO;

namespace NUnitTest.FileService
{
    class FileUtility_UnitTest
    {
        [Test]
        public void DataTable_ToList_Test()
        {
            string fileNameAndPath = @"D:\test.txt";
            FileUtility.Write(fileNameAndPath, "test value");
            FileUtility.WriteLine(fileNameAndPath, "test new line value 1");
            FileUtility.WriteLine(fileNameAndPath, "test new line value 2");

            Assert.Pass();
        }

        [Test]
        public void GetDirectoryName()
        {
            string path1 = Path.GetDirectoryName("aaas.txt");

            string path2 = Path.GetDirectoryName("D:\\aaas.txt");

            var exc = new DirectoryNotFoundException("filePathAndName");

            Assert.Pass();
        }

        [Test]
        public void WriteLine()
        {
            Assert.Throws<DirectoryNotFoundException>(()=> {
                FileUtility.WriteLine("Test.txt", $"Run on {DateTime.Now}");
            });

            Assert.Pass();
        }
    }
}
