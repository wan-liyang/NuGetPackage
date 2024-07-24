using PgpCore;
using System.IO;

namespace TryIT.PGP
{
    /// <summary>
    /// library for Pgp encryption and decryption method
    /// </summary>
    public class Lib
    {
        /// <summary>
        /// validate file exists, throw exception if not exists
        /// </summary>
        /// <param name="fileNameAndPath"></param>
        /// <param name="message"></param>
        /// <exception cref="FileNotFoundException"></exception>
        private static void ValidateFileExists(string fileNameAndPath, string message)
        {
            if (!File.Exists(fileNameAndPath))
                throw new FileNotFoundException(message);
        }

        /// <summary>
        /// delete the file if exists
        /// </summary>
        /// <param name="fileNameAndPath"></param>
        private static void DeleteIfExists(string fileNameAndPath)
        {
            if (File.Exists(fileNameAndPath))
            {
                File.Delete(fileNameAndPath);
            }
        }

        /// <summary>
        /// encrypt file
        /// </summary>
        /// <param name="publicKeyFileNameAndPath">public key file name (with full path)</param>
        /// <param name="inputFileNameAndPath">input file to be encrypt (with full path)</param>
        /// <param name="outputFileNameAndPath">encrypted output file, if empty, will be .pgp extension in same input path</param>
        /// <returns>encrytped file name with full path</returns>
        /// <exception cref="FileNotFoundException"></exception>
        public static string EncryptFile(string publicKeyFileNameAndPath, string inputFileNameAndPath, string outputFileNameAndPath = "")
        {
            ValidateFileExists(publicKeyFileNameAndPath, "public key file not found.");
            ValidateFileExists(inputFileNameAndPath, "input file not found.");

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

            DeleteIfExists(outputFileNameAndPath);

            // Encrypt
            PgpCore.PGP pgp = new PgpCore.PGP(encryptionKeys);
            pgp.EncryptFileAsync(input, output).GetAwaiter().GetResult();

            return outputFileNameAndPath;
        }

        /// <summary>
        /// decrypt file
        /// </summary>
        /// <param name="privateKeyFileNameAndPath">private key file name (with full path)</param>
        /// <param name="passPhrase">passPhrase of private key, put empty string if no passPhrase</param>
        /// <param name="inputFileNameAndPath">input file name to be decrypt (with full path)</param>
        /// <param name="outputFileNameAndPath">decrypted output file, if empty, will be remove .gpg extension from input file</param>
        /// <returns></returns>
        public static string DecryptFile(string privateKeyFileNameAndPath, string passPhrase, string inputFileNameAndPath, string outputFileNameAndPath = "")
        {
            ValidateFileExists(privateKeyFileNameAndPath, "private key file not found.");
            ValidateFileExists(inputFileNameAndPath, "input file not found.");

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

            DeleteIfExists(outputFileNameAndPath);

            // Decrypt
            PgpCore.PGP pgp = new PgpCore.PGP(encryptionKeys);
            pgp.DecryptFileAsync(input, output).GetAwaiter().GetResult();

            return outputFileNameAndPath;
        }
    }
}

