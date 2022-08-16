using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace TryIT.CryptoService
{
    /// <summary>
    /// MD5 hash
    /// </summary>
    public class MD5Hash
    {
        /// <summary>
        /// get MD5 hash result for a string value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetMD5Hash(string value)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                // Convert the input string to a byte array and compute the hash.
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(value));
                return GetHash(data);
            }
        }

        /// <summary>
        /// get MD5 hash result for a FileStream value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetMD5Hash(FileStream value)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                // compute the hash.
                byte[] data = md5Hash.ComputeHash(value);
                return GetHash(data);
            }
        }

        /// <summary>
        /// Get hexadecimal string from byte
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static string GetHash(byte[] data)
        {
            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        /// <summary>
        /// Verify a hash against a string.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="hashValue"></param>
        /// <returns></returns>
        public static bool VerifyMd5Hash(string value, string hashValue)
        {
            // Hash the input.
            string hashOfInput = GetMD5Hash(value);

            // Create a StringComparer an compare the hashes.
            System.StringComparer comparer = System.StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hashValue))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Get MD5 hash from file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetFileMD5(string path)
        {
            FileInfo fileInfo = new FileInfo(path);
            if (!fileInfo.Exists)
            {
                return string.Empty;
            }
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return GetMD5Hash(fs);
            }
        }
    }
}
