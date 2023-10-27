using Renci.SshNet;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace TryIT.SFTP
{
    /// <summary>
    /// operate with SFTP server
    /// <para>1. init connection with <see cref="InitConenctionInfoPassword(string, int, string, string)"/> </para>
    /// <para>or <see cref="InitConenctionInfoPrivateKey(string, int, string, byte[])"/></para>
    /// <para>2. use that connection to do specific operation</para>
    /// </summary>
    public class SFTP
    {
        #region Init Connection Info
        /// <summary>
        /// init connection with username and password
        /// </summary>
        /// <param name="ip">sftp server ip address</param>
        /// <param name="port">sftp server port</param>
        /// <param name="username">sftp account username</param>
        /// <param name="password">sftp account password</param>
        /// <returns></returns>
        public static ConnectionInfo InitConenctionInfoPassword(string ip, int port, string username, string password)
        {
            ConnectionInfo connectionInfo = new PasswordConnectionInfo(ip, port, username, password);

            return connectionInfo;
        }

        /// <summary>
        /// init connection with private key file
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="username"></param>
        /// <param name="privateKeyFileNameAndPath"></param>
        /// <returns></returns>
        public static ConnectionInfo InitConenctionInfoPrivateKey(string ip, int port, string username, string privateKeyFileNameAndPath)
        {
            ConnectionInfo connectionInfo = new PrivateKeyConnectionInfo(ip, port, username, new PrivateKeyFile(privateKeyFileNameAndPath));

            return connectionInfo;
        }

        /// <summary>
        /// init connection with private key file byte
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="username"></param>
        /// <param name="privateKeyFileNameAndPath"></param>
        /// <returns></returns>
        public static ConnectionInfo InitConenctionInfoPrivateKey(string ip, int port, string username, byte[] privateKeyFileNameAndPath)
        {
            MemoryStream keyFileStream = new MemoryStream(privateKeyFileNameAndPath);

            ConnectionInfo connectionInfo = new PrivateKeyConnectionInfo(ip, port, username, new PrivateKeyFile(keyFileStream));

            return connectionInfo;
        }
        #endregion

        /// <summary>
        /// test SFTP connection, return true if connect success, otherwise false
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static bool TestConnection(ConnectionInfo connection)
        {
            bool success = false;
            using (var sftp = new SftpClient(connection))
            {
                sftp.Connect();
                success = true;
                sftp.Disconnect();
            }
            return success;
        }

        #region Directory
        /// <summary>
        /// list all directory from SFTP root path "/"
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static List<string> ListDirectory(ConnectionInfo connection)
        {
            List<string> folders = null;
            using (var sftp = new SftpClient(connection))
            {
                sftp.Connect();

                folders = sftp.ListDirectory("/")
                    .Where(p => p.IsRegularFile == false)
                    .Select(p => p.FullName)
                    .ToList();

                sftp.Disconnect();
            }
            return folders;
        }

        /// <summary>
        /// list all directory and file from SFTP root path "/"
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static List<string> ListDirectoryAndFile(ConnectionInfo connection)
        {
            List<string> folders = null;
            using (var sftp = new SftpClient(connection))
            {
                sftp.Connect();

                folders = sftp.ListDirectory("/")
                    //.Where(p => p.IsRegularFile == false)
                    .Select(p => p.FullName)
                    .ToList();

                sftp.Disconnect();
            }
            return folders;
        }

        /// <summary>
        /// create directory into SFTP folder, if directory already exists, then no action will be perform
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="folderPath"></param>
        public static void CreateDirectory(ConnectionInfo connection, string folderPath)
        {
            using (var sftp = new SftpClient(connection))
            {
                sftp.Connect();

                if (!sftp.Exists(folderPath))
                {
                    sftp.CreateDirectory(folderPath);
                }

                sftp.Disconnect();
            }
        }
        #endregion

        #region File
        /// <summary>
        /// upload source file into SFTP folder, the <paramref name="targetFileNameAndPath"/> folder must exists
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="sourceFileNameAndPath"></param>
        /// <param name="targetFileNameAndPath"></param>
        public static void Upload(ConnectionInfo connection, string sourceFileNameAndPath, string targetFileNameAndPath)
        {
            using (var sftp = new SftpClient(connection))
            {
                sftp.Connect();

                using (var file = File.OpenRead(sourceFileNameAndPath))
                {
                    sftp.UploadFile(file, targetFileNameAndPath);
                }

                sftp.Disconnect();
            }
        }

        /// <summary>
        /// list all files from <paramref name="sftpPath"/>
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="sftpPath"></param>
        /// <param name="fileNameFilterRegex">filter file name with Regex</param>
        /// <returns></returns>
        public static List<string> ListFile(ConnectionInfo connection, string sftpPath = "/", string fileNameFilterRegex = "")
        {
            List<string> files = null;
            using (var sftp = new SftpClient(connection))
            {
                sftp.Connect();

                files = sftp.ListDirectory(sftpPath)
                    .Where(p => p.IsRegularFile == true)
                    .Select(p => p.Name) // not use FullName, for caller based on only file name to save the file
                    .ToList();

                sftp.Disconnect();
            }

            if (files != null && files.Count > 0 && !string.IsNullOrEmpty(fileNameFilterRegex))
            {
                return files.Where(p => Regex.IsMatch(p, fileNameFilterRegex)).ToList();
            }

            return files;
        }

        /// <summary>
        /// download sftp file into <paramref name="downloadPath"/> directory
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="sftpPath">sftp path, if not subdirectory, put "/"</param>
        /// <param name="sftpFile">sftp file name</param>
        /// <param name="downloadPath">target path to store downloaded file, this is directory</param>
        public static void DownloadFile(ConnectionInfo connection, string sftpPath, string sftpFile, string downloadPath)
        {
            using(var sftp = new SftpClient(connection))
            {
                sftp.Connect();
                string fileToDownload = sftpPath + sftpFile;
                string fileToWrite = System.IO.Path.Combine(downloadPath, sftpFile);
                using (var fileStream = File.Create(fileToWrite))
                {
                    sftp.DownloadFile(fileToDownload, fileStream);
                }
            }
        }

        /// <summary>
        /// delete sftp file
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="sftpPath">sftp path, if not subdirectory, put "/"</param>
        /// <param name="sftpFile">sftp file name</param>
        public static void DeleteFile(ConnectionInfo connection, string sftpPath, string sftpFile)
        {
            using (var sftp = new SftpClient(connection))
            {
                sftp.Connect();
                string fileToDelete = System.IO.Path.Combine(sftpPath, sftpFile);
                sftp.DeleteFile(fileToDelete);
            }
        }
        #endregion
    }
}
