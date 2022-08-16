using Renci.SshNet;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SFTPService
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
        /// init connection with username & password
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
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
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
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
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
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
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
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
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
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
        #endregion
    }
}
