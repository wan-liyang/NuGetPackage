using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.IO;
using System.Linq;

namespace TryIT.FileService.Zip
{
    /// <summary>
    /// Create zip file or Unzip a file
    /// </summary>
    public class ZipUnzip
    {

        #region private method
        private enum PathType
        {
            FilePath_CreateFolder,
            FolderPath_CreateFolder
        }
        /// <summary>
        /// create folder / file in specific path, if the folder / file not exists
        /// <para>if already exists, do nothing</para>
        /// </summary>
        /// <param name="folderPathOrFilePath"></param>
        private static void CreateIfNotExists(string folderPathOrFilePath, PathType pathType)
        {
            if (string.IsNullOrEmpty(folderPathOrFilePath))
                throw new ArgumentNullException("folderPathOrFilePath");

            string folderPath = folderPathOrFilePath;
            if (pathType == PathType.FilePath_CreateFolder)
            {
                folderPath = Path.GetDirectoryName(folderPathOrFilePath);
            }

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }
        /// <summary>
        /// Deletes the specified directory
        /// </summary>
        /// <param name="dirPath">The name of the directory to remove.</param>
        /// <param name="isRecursive">true to remove directories, subdirectories, and files in path; otherwise, false.</param>
        private static void DeleteFolder(string dirPath, bool isRecursive = false)
        {
            // delete directory if exists
            if (Directory.Exists(dirPath))
            {
                // if not Recursive, only delete directory when it's empty
                if (!isRecursive)
                {
                    bool isEmpty = IsDirectoryEmpty(dirPath);
                    if (isEmpty)
                    {
                        Directory.Delete(dirPath);
                    }
                }
                else
                {
                    Directory.Delete(dirPath, isRecursive);
                }
            }
        }
        // <summary>
        /// check whether specific directory is empty, true: it's empty, otherwise false
        /// </summary>
        /// <param name="dirPath"></param>
        /// <returns></returns>
        private static bool IsDirectoryEmpty(string dirPath)
        {
            return !Directory.EnumerateFileSystemEntries(dirPath).Any();
        }
        #endregion

        #region Create Zip File

        /// <summary>
        /// unzip <paramref name="sourceZipFile"/> into <paramref name="targetDir"/>
        /// </summary>
        /// <param name="sourceZipFile"></param>
        /// <param name="targetDir"></param>
        public static void UnZip(string sourceZipFile, string targetDir)
        {
            FastZip fastZip = new FastZip();
            string fileFilter = null;

            // Will always overwrite if target filenames already exist
            fastZip.ExtractZip(sourceZipFile, targetDir, fileFilter);
        }

        /// <summary>
        /// create zip file based on list of file
        /// </summary>
        /// <param name="filePathAndName"></param>
        /// <param name="outputZipFilePathAndName"></param>
        /// <param name="password"></param>
        public static void Zip(string[] filePathAndName, string outputZipFilePathAndName, string password = null)
        {
            // create temp folder to keep all files need to zip
            string tempPath = Path.GetTempPath();
            string tempFolder = Path.Combine(tempPath, "Zip_" + Guid.NewGuid().ToString());
            CreateIfNotExists(tempFolder, PathType.FolderPath_CreateFolder);

            // copy files into temp folder
            foreach (var sourceFilePathAndName in filePathAndName)
            {
                if (File.Exists(sourceFilePathAndName))
                {
                    FileInfo fileInfo = new FileInfo(sourceFilePathAndName);

                    string fileName = fileInfo.Name;
                    string destinationFilePathAndName = Path.Combine(tempFolder, fileName);

                    File.Copy(sourceFilePathAndName, destinationFilePathAndName);
                }
            }

            // create zip file
            CreateZip(tempFolder, outputZipFilePathAndName, password);

            // delete temp folder
            DeleteFolder(tempFolder, true);
        }
        /// <summary>
        /// create zip file based on source folder
        /// </summary>
        /// <param name="sourceFolderPath"></param>
        /// <param name="outputZipFilePathAndName"></param>
        /// <param name="password"></param>
        public static void Zip(string sourceFolderPath, string outputZipFilePathAndName, string password = null)
        {
            CreateZip(sourceFolderPath, outputZipFilePathAndName, password);
        }

        /// <summary>
        /// Compresses the files in the nominated folder, and creates a zip file 
        /// </summary>
        /// <param name="sourceFolderPath"></param>
        /// <param name="outputZipFilePathAndName"></param>
        /// <param name="password"></param>
        private static void CreateZip(string sourceFolderPath, string outputZipFilePathAndName, string password = null)
        {
            CreateIfNotExists(outputZipFilePathAndName, PathType.FilePath_CreateFolder);

            using (FileStream fsOut = File.Create(outputZipFilePathAndName))
            using (var zipStream = new ZipOutputStream(fsOut))
            {
                //0-9, 9 being the highest level of compression
                zipStream.SetLevel(3);

                // optional. Null is the same as not setting. Required if using AES.
                zipStream.Password = password;

                // This setting will strip the leading part of the folder path in the entries, 
                // to make the entries relative to the starting folder.
                // To include the full path for each entry up to the drive root, assign to 0.
                int folderOffset = sourceFolderPath.Length + (sourceFolderPath.EndsWith("\\") ? 0 : 1);

                CompressFolder(sourceFolderPath, zipStream, folderOffset);
            }
        }

        /// <summary>
        /// Recursively compresses a folder structure
        /// </summary>
        /// <param name="path"></param>
        /// <param name="zipStream"></param>
        /// <param name="folderOffset"></param>
        private static void CompressFolder(string path, ZipOutputStream zipStream, int folderOffset)
        {
            var files = Directory.GetFiles(path);

            foreach (var filename in files)
            {
                var fi = new FileInfo(filename);

                // Make the name in zip based on the folder
                var entryName = filename.Substring(folderOffset);

                // Remove drive from name and fix slash direction
                entryName = ZipEntry.CleanName(entryName);

                var newEntry = new ZipEntry(entryName);

                // Note the zip format stores 2 second granularity
                newEntry.DateTime = fi.LastWriteTime;

                // Specifying the AESKeySize triggers AES encryption. 
                // Allowable values are 0 (off), 128 or 256.
                // A password on the ZipOutputStream is required if using AES.
                //   newEntry.AESKeySize = 256;

                // To permit the zip to be unpacked by built-in extractor in WinXP and Server2003,
                // WinZip 8, Java, and other older code, you need to do one of the following: 
                // Specify UseZip64.Off, or set the Size.
                // If the file may be bigger than 4GB, or you do not need WinXP built-in compatibility, 
                // you do not need either, but the zip will be in Zip64 format which
                // not all utilities can understand.
                //   zipStream.UseZip64 = UseZip64.Off;
                newEntry.Size = fi.Length;

                zipStream.PutNextEntry(newEntry);

                // Zip the file in buffered chunks
                // the "using" will close the stream even if an exception occurs
                var buffer = new byte[4096];
                using (FileStream fsInput = File.OpenRead(filename))
                {
                    StreamUtils.Copy(fsInput, zipStream, buffer);
                }
                zipStream.CloseEntry();
            }

            // Recursively call CompressFolder on all folders in path
            var folders = Directory.GetDirectories(path);
            foreach (var folder in folders)
            {
                CompressFolder(folder, zipStream, folderOffset);
            }
        }
        #endregion
    }
}
