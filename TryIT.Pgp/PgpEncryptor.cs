using PgpCore;
using System;
using System.IO;
using System.Threading.Tasks;

namespace TryIT.Pgp
{
    internal static class PgpEncryptor
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
            Util.ValidateFileExists(publicKeyFileNameAndPath, "Public key file not found.");
            Util.ValidateFileExists(inputFileNameAndPath, "Input file not found.");

            // set output file as .gpg extension and save into same input path
            if (string.IsNullOrEmpty(outputFileNameAndPath))
            {
                outputFileNameAndPath = $"{inputFileNameAndPath}.pgp";
            }

            // Load keys
            FileInfo publicKey = new FileInfo(publicKeyFileNameAndPath);
            EncryptionKeys encryptionKeys = new EncryptionKeys(publicKey);

            // Reference input/output files
            FileInfo input = new FileInfo(inputFileNameAndPath);
            FileInfo output = new FileInfo(outputFileNameAndPath);

            Util.DeleteIfExists(outputFileNameAndPath);

            // Encrypt
            PgpCore.PGP pgp = new PgpCore.PGP(encryptionKeys);
            await pgp.EncryptFileAsync(input, output);

            return outputFileNameAndPath;
        }

        public static async Task<Stream> EncryptFileToStreamAsync(
            string publicKeyFileNameAndPath,
            string inputFileNameAndPath,
            long memoryThresholdBytes = 100 * 1024 * 1024 // 100 MB
        )
        {
            Util.ValidateFileExists(publicKeyFileNameAndPath, "Public key file not found.");
            Util.ValidateFileExists(inputFileNameAndPath, "Input file not found.");

            var inputFile = new FileInfo(inputFileNameAndPath);
            var publicKey = new FileInfo(publicKeyFileNameAndPath);

            var encryptionKeys = new EncryptionKeys(publicKey);
            var pgp = new PgpCore.PGP(encryptionKeys);

            Stream outputStream;

            // Decide memory vs file stream
            if (inputFile.Length <= memoryThresholdBytes)
            {
                // Small file → Memory
                outputStream = new MemoryStream();
            }
            else
            {
                // Large file → Temp file
                var tempFilePath = Path.Combine(
                    Path.GetTempPath(),
                    $"{Guid.NewGuid()}.pgp"
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

            // Stream encryption
            await pgp.EncryptStreamAsync(
                inputStream,
                outputStream,
                withIntegrityCheck: true,
                armor: false // set true if ASCII-armored PGP needed
            );

            if (outputStream.CanSeek)
                outputStream.Position = 0;

            return outputStream; // Caller disposes
        }
    }
}
