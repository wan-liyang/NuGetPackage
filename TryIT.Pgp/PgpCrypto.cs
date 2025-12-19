using System.IO;
using System.Threading.Tasks;

namespace TryIT.Pgp
{
    /// <summary>
    /// Pgp crypto operations
    /// </summary>
    public static class PgpCrypto
    {
        /// <summary>
        /// encrypt file
        /// </summary>
        /// <param name="publicKeyFileNameAndPath">public key file name (with full path)</param>
        /// <param name="inputFileNameAndPath">input file to be encrypt (with full path)</param>
        /// <param name="outputFileNameAndPath">encrypted output file, if empty, will be .pgp extension in same input path</param>
        /// <returns>encrytped file name with full path</returns>
        /// <exception cref="FileNotFoundException"></exception>
        public static async Task<string> EncryptFileAsync(
            string publicKeyFileNameAndPath, 
            string inputFileNameAndPath, 
            string outputFileNameAndPath = "")
        {
            return await PgpEncryptor.EncryptFileAsync(
                publicKeyFileNameAndPath,
                inputFileNameAndPath,
                outputFileNameAndPath);
        }

        /// <summary>
        /// Encrypt file to stream
        /// </summary>
        /// <param name="publicKeyFileNameAndPath">public key file name (full path)</param>
        /// <param name="inputFileNameAndPath">input file name to be encrypt (full path)</param>
        /// <param name="memoryThresholdBytes">threshold bytes to decrypt into memory or temp file, default 100 MB</param>
        /// <returns></returns>
        public static async Task<Stream> EncryptFileToStreamAsync(
            string publicKeyFileNameAndPath,
            string inputFileNameAndPath,
            long memoryThresholdBytes = 100 * 1024 * 1024 // 100 MB
        )
        {
            return await PgpEncryptor.EncryptFileToStreamAsync(
                publicKeyFileNameAndPath,
                inputFileNameAndPath,
                memoryThresholdBytes);
        }


        /// <summary>
        /// decrypt file
        /// </summary>
        /// <param name="privateKeyFileNameAndPath">private key file name (with full path)</param>
        /// <param name="passPhrase">passPhrase of private key, put empty string if no passPhrase</param>
        /// <param name="inputFileNameAndPath">input file name to be decrypt (with full path)</param>
        /// <param name="outputFileNameAndPath">decrypted output file, if empty, will be remove .gpg extension from input file</param>
        /// <returns></returns>
        public static async Task<string> DecryptFileAsync(
            string privateKeyFileNameAndPath, 
            string passPhrase, 
            string inputFileNameAndPath, 
            string outputFileNameAndPath = "")
        {
            return await PgpDecryptor.DecryptFileAsync(
                privateKeyFileNameAndPath,
                passPhrase,
                inputFileNameAndPath,
                outputFileNameAndPath);
        }

        /// <summary>
        /// Decrypt file to stream
        /// </summary>
        /// <param name="privateKeyFileNameAndPath">private key file name (full path)</param>
        /// <param name="passPhrase">passPhrase of private key, put empty string if no passPhrase</param>
        /// <param name="inputFileNameAndPath">input file name to be decrypt (full path)</param>
        /// <param name="memoryThresholdBytes">threshold bytes to decrypt into memory or temp file, default 100 MB</param>
        /// <returns></returns>
        public static async Task<Stream> DecryptToStreamAsync(
            string privateKeyFileNameAndPath,
            string passPhrase,
            string inputFileNameAndPath,
            long memoryThresholdBytes = 100 * 1024 * 1024 // 100 MB default
        )
        {
            return await PgpDecryptor.DecryptToStreamAsync(
                privateKeyFileNameAndPath,
                passPhrase,
                inputFileNameAndPath,
                memoryThresholdBytes);
        }
    }
}
