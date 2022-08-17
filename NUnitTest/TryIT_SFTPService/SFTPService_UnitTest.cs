using NUnit.Framework;
using Renci.SshNet;
using TryIT.SFTPService;

namespace NUnitTest.SFTP
{
    class SFTPService_UnitTest
    {
        ConnectionInfo _connection;
        [SetUp]
        public void Initial()
        {
            //_connection = SFTPService.InitConenctionInfoPassword("10.70.137.132", 22, "sftptestuser", "_Pass123");
            _connection = SFTP.SFTPService.SFTP.InitConenctionInfoPrivateKey("10.70.137.132", 22, "sftpuser2", @"D:\WanLiYang\Project\NuGetPackage\SFTP\sftpuser2-openssh.ppk");
        }

        [Test]
        public void TestConnection()
        {
            //SFTPService.TestConnection2();

            bool success = SFTPService.SFTP.TestConnection(_connection);
            Assert.IsTrue(success);
        }

        [Test]
        public void ListDirectory()
        {
            SFTPService.SFTP.ListDirectoryAndFile(_connection);

            Assert.Pass();
        }

        [Test]
        public void CreateDirectory()
        {
            SFTPService.SFTP.CreateDirectory(_connection, "/folder3");

            Assert.Pass();
        }

        [Test]
        public void Upload()
        {
            string sourceFileNameAndPath = @"D:\1.txt";

            Assert.DoesNotThrow(() => {
                SFTPService.SFTP.Upload(_connection, sourceFileNameAndPath, "/test.txt");
            });

            Assert.Pass();
        }

    }
}
