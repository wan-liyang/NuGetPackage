using System;
using System.IO;
using System.Linq;

namespace FileService
{
    public class FileUtility
    {

        #region To / From Base64String
        /// <summary>
        /// convert a file to base64string
        /// </summary>
        /// <param name="fileNameAndPath"></param>
        /// <returns></returns>
        public static string ToBase64String(string fileNameAndPath)
        {
            byte[] bytes = System.IO.File.ReadAllBytes(fileNameAndPath);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// convert base64string to a file byte
        /// </summary>
        /// <param name="base64string"></param>
        /// <returns></returns>
        public static byte[] FromBase64String(string base64string)
        {
            return Convert.FromBase64String(base64string);
        }
        #endregion

        #region ToByte
        /// <summary>
        /// convert file to byte
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="isDelete"></param>
        /// <returns></returns>
        public static byte[] ToByte(string filePath, bool isDelete = false)
        {
            byte[] bFiles;
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                var size = fs.Length;
                var b = new byte[size];
                fs.Read(b, 0, b.Length);
                bFiles = b;
            }

            if (isDelete)
            {
                File.Delete(filePath);
            }

            return bFiles;
        } 
        #endregion

        public static void SaveAs(string fileNameAndPath, byte[] fileByte)
        {
            CreateIfNotExists(fileNameAndPath, PathType.FilePath_CreateFolder);

            File.WriteAllBytes(fileNameAndPath, fileByte);
        }

        public static void DeleteFile(string filePath, bool isRemoveFolderIfEmpty = false)
        {
            // delete file if exists
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            // delete directory if empty
            if (isRemoveFolderIfEmpty)
            {
                string dir = Path.GetDirectoryName(filePath);
                DeleteFolder(dir);
            }
        }

        /// <summary>
        /// Delete file from path <paramref name="rootPath"/> when path exists, e.g. DeleteFile("path", "*.*", false)
        /// <para>do nothing if path not exists</para>
        /// </summary>
        /// <param name="rootPath">the path need delete file</param>
        /// <param name="pattern">search pattern to get the files, cannot be empty, The search string to match against the names of files in path. This parameter can contain a combination of valid literal path and wildcard (* and ?) characters, but it doesn't support regular expressions.</param>
        /// <param name="isRemoveFolderIfEmpty">indicator whether delete folder if it become empty folder</param>
        public static void DeleteFile(string rootPath, string pattern, bool isRemoveFolderIfEmpty = false)
        {
            if (Directory.Exists(rootPath))
            {
                var files = Directory.GetFiles(rootPath, pattern);
                foreach (var item in files)
                {
                    DeleteFile(item, isRemoveFolderIfEmpty);
                }
            }
        }

        /// <summary>
        /// Deletes the specified directory
        /// </summary>
        /// <param name="dirPath">The name of the directory to remove.</param>
        /// <param name="isRecursive">true to remove directories, subdirectories, and files in path; otherwise, false.</param>
        public static void DeleteFolder(string dirPath, bool isRecursive = false)
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

        /// <summary>
        /// check whether specific directory is empty, true: it's empty, otherwise false
        /// </summary>
        /// <param name="dirPath"></param>
        /// <returns></returns>
        public static bool IsDirectoryEmpty(string dirPath)
        {
            return !Directory.EnumerateFileSystemEntries(dirPath).Any();
        }

        /// <summary>
        /// write text to specific file, create a new file if not exists, no action will be perform if <paramref name="text"/> is empty
        /// </summary>
        /// <param name="fileNameAndPath">file path with file name</param>
        /// <param name="text">text to write</param>
        /// <param name="isOverwrite">true: overwrite existing content, false: append text to end</param>
        /// <exception cref="ArgumentNullException">if <paramref name="fileNameAndPath"/> is empty</exception>
        /// <exception cref="DirectoryNotFoundException">if <paramref name="fileNameAndPath"/> directory not found</exception>
        public static void Write(string fileNameAndPath, string text, bool isOverwrite = false)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            if (string.IsNullOrEmpty(fileNameAndPath))
            {
                throw new ArgumentNullException(nameof(fileNameAndPath));
            }

            // create directory if not exists
            string dir = Path.GetDirectoryName(fileNameAndPath);
            if (string.IsNullOrEmpty(dir))
            {
                throw new DirectoryNotFoundException($"Directory not found for: {fileNameAndPath}");
            }

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            // if need overwrite, do delete & re-create
            if (isOverwrite)
            {
                File.WriteAllText(fileNameAndPath, text);
            }
            else
            {
                File.AppendAllText(fileNameAndPath, text);
            }
        }

        /// <summary>
        /// write text to specific file follow up new line, create a new file if not exists
        /// </summary>
        /// <param name="fileNameAndPath"></param>
        /// <param name="text"></param>
        /// <param name="isOverwrite"></param>
        public static void WriteLine(string fileNameAndPath, string text, bool isOverwrite = false)
        {
            Write(fileNameAndPath, text + Environment.NewLine, isOverwrite);
        }

        public enum PathType
        {
            FilePath_CreateFolder,
            FolderPath_CreateFolder
        }
        /// <summary>
        /// create folder / file in specific path, if the folder / file not exists
        /// <para>if already exists, do nothing</para>
        /// </summary>
        /// <param name="folderPathOrFilePath"></param>
        public static void CreateIfNotExists(string folderPathOrFilePath, PathType pathType)
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
        /// clean file name to replace special character to '_', and cut to specific length, <paramref name="maxLength"/> default 0 will not cut file name
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public static string CleanName(string fileName, int maxLength = 0)
        {
            if (string.IsNullOrEmpty(fileName)) 
                throw new ArgumentNullException("fileName");

            fileName = fileName.Replace("\\", "_");
            fileName = fileName.Replace("/", "_");
            fileName = fileName.Replace(":", "_");
            fileName = fileName.Replace("*", "_");
            fileName = fileName.Replace("?", "_");
            fileName = fileName.Replace("\"", "_");
            fileName = fileName.Replace("<", "_");
            fileName = fileName.Replace(">", "_");
            fileName = fileName.Replace("|", "_");

            if (fileName.Length > 0 && maxLength > 0 && fileName.Length > maxLength)
            {
                fileName = fileName.Substring(0, maxLength);
            }
            return fileName;
        }
    }
}
