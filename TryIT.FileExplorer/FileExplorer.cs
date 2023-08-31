using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TryIT.FileExplorer
{
    /// <summary>
    /// init file explorer object
    /// </summary>
    public class FileExplorer
    {
        private List<string> _files = new List<string>();

        /// <summary>
        /// copy <paramref name="sourceDir"/> to <paramref name="destinationDir"/>, refer to https://learn.microsoft.com/en-us/dotnet/standard/io/how-to-copy-directories
        /// </summary>
        /// <param name="sourceDir"></param>
        /// <param name="destinationDir"></param>
        /// <param name="recursive"></param>
        /// <param name="overwrite"></param>
        /// <returns>copied files</returns>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public List<string> CopyDirectory(string sourceDir, string destinationDir, bool recursive, bool overwrite)
        {
            // Get information about the source directory
            var dir = new DirectoryInfo(sourceDir);

            // Check if the source directory exists
            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            // Cache directories before we start copying
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Create the destination directory
            if (!Directory.Exists(destinationDir))
            {
                Directory.CreateDirectory(destinationDir);
            }

            // Get the files in the source directory and copy to the destination directory
            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath, overwrite);

                _files.Add(file.FullName);
            }

            // If recursive and copying subdirectories, recursively call this method
            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true, overwrite);
                }
            }

            return _files;
        }
    }
}
