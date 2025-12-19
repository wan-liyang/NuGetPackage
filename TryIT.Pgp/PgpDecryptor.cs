using PgpCore;
using System;
using System.IO;
using System.Threading.Tasks;

namespace TryIT.Pgp
{
    /// <summary>
    /// Pgp decryptor
    /// </summary>
    internal static class PgpDecryptor
    {
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
            Util.ValidateFileExists(privateKeyFileNameAndPath, "Private key file not found.");
            Util.ValidateFileExists(inputFileNameAndPath, "Input file not found.");

            // set output file as .pgp extension and save into same input path
            if (string.IsNullOrEmpty(outputFileNameAndPath))
            {
                outputFileNameAndPath = inputFileNameAndPath.Replace(".pgp", "");
            }

            // Load keys
            FileInfo privateKey = new FileInfo(privateKeyFileNameAndPath);
            EncryptionKeys encryptionKeys = new EncryptionKeys(privateKey, passPhrase);

            // Reference input/output files
            FileInfo input = new FileInfo(inputFileNameAndPath);
            FileInfo output = new FileInfo(outputFileNameAndPath);

            Util.DeleteIfExists(outputFileNameAndPath);

            // Decrypt
            PgpCore.PGP pgp = new PgpCore.PGP(encryptionKeys);
            await pgp.DecryptFileAsync(input, output);

            return outputFileNameAndPath;
        }

        /// <summary>
        /// Decrypt file to stream
        /// </summary>
        /// <param name="privateKeyFileNameAndPath">private key file name (with full path)</param>
        /// <param name="passPhrase">passPhrase of private key, put empty string if no passPhrase</param>
        /// <param name="inputFileNameAndPath">input file name to be decrypt (with full path)</param>
        /// <param name="memoryThresholdBytes">threshold bytes to decrypt into memory or temp file, default 100 MB</param>
        /// <returns></returns>
        public static async Task<Stream> DecryptToStreamAsync(
                string privateKeyFileNameAndPath,
                string passPhrase,
                string inputFileNameAndPath,
                long memoryThresholdBytes = 100 * 1024 * 1024 // 100 MB default
            )
        {
            Util.ValidateFileExists(privateKeyFileNameAndPath, "Private key file not found.");
            Util.ValidateFileExists(inputFileNameAndPath, "Input file not found.");

            var inputFile = new FileInfo(inputFileNameAndPath);
            var privateKey = new FileInfo(privateKeyFileNameAndPath);
            var encryptionKeys = new EncryptionKeys(privateKey, passPhrase);

            var pgp = new PgpCore.PGP(encryptionKeys);

            Stream outputStream;

            // Decide memory vs file stream
            if (inputFile.Length <= memoryThresholdBytes)
            {
                // ✅ Small file → Memory
                outputStream = new MemoryStream();
            }
            else
            {
                // ✅ Large file → Temp file (safe for GBs)
                var tempFilePath = Path.Combine(
                    Path.GetTempPath(),
                    $"{Guid.NewGuid()}.decrypted"
                );

                outputStream = new AutoDeleteFileStream(
                    tempFilePath,
                    FileMode.Create,
                    FileAccess.ReadWrite,
                    FileShare.None,
                    bufferSize: 81920,
                    useAsync: true
                );
            }

            await using var inputStream = inputFile.OpenRead();

            // Stream decrypt (no full memory load)
            await pgp.DecryptStreamAsync(inputStream, outputStream);

            if (outputStream.CanSeek)
                outputStream.Position = 0;

            return outputStream; // Caller owns disposal
        }
    }
}