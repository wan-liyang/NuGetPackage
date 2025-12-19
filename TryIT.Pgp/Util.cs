using System.IO;

namespace TryIT.Pgp
{
    /// <summary>
    /// Utility methods
    /// </summary>
    internal static class Util
    {
        public static void ValidateFileExists(string privateKeyFileNameAndPath, string v)
        {
            if (!File.Exists(privateKeyFileNameAndPath))
                throw new FileNotFoundException(v);
        }

        /// <summary>
        /// delete the file if exists
        /// </summary>
        /// <param name="fileNameAndPath"></param>
        public static void DeleteIfExists(string fileNameAndPath)
        {
            if (File.Exists(fileNameAndPath))
            {
                File.Delete(fileNameAndPath);
            }
        }
    }
}
