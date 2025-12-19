using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;

namespace NUnitTest02.TryIT_PGP
{
    public class PGP_UnitTest
    {
        [Test]
        public async Task DecryptToStreamAsync_Test_10MB()
        {
            var file_10_MB = await TempFileGenerator.Generate10MBAsync();

            string publicKeyFile = @"C:\Temp\pgp-test-pub.asc";
            string privateKeyFile = @"C:\Temp\pgp-test-sec.asc";
            string passPhrase = "pass";

            string outPgpFile = @$"C:\Temp\{Path.GetFileName(file_10_MB)}.pgp";
            string outOriFile = @$"C:\Temp\{Path.GetFileName(file_10_MB)}.origin";

            // Obtain encrypted stream (assume this returns a readable Stream)
            await using var encryptedStream = await TryIT.Pgp.PgpCrypto.EncryptFileToStreamAsync(
                publicKeyFile,
                file_10_MB
            );

            // Write encrypted stream to disk and ensure the file handle is closed before decrypting.
            // Use a scoped await-using so disposal happens immediately after the copy.
            await using (var outFs = File.Create(outPgpFile))
            {
                await encryptedStream.CopyToAsync(outFs);
            } // outFs closed here -> no file lock when decrypting

            // Now call decrypt which can open the outputFile without file-in-use error.
            await using var decryptedStream = await TryIT.Pgp.PgpCrypto.DecryptToStreamAsync(
                privateKeyFile,
                passPhrase,
                outPgpFile
            );

            Assert.IsNotNull(decryptedStream, "Decrypted stream should not be null");

            // Write the decrypted stream to a separate file for inspection.
            await using (var outFs2 = File.Create(outOriFile))
            {
                await decryptedStream.CopyToAsync(outFs2);
            }

            // Optionally assert the output file exists and has content
            Assert.IsTrue(File.Exists(outOriFile), $"Decrypted output file '{outOriFile}' should exist.");
            Assert.IsTrue(new FileInfo(outOriFile).Length > 0, "Decrypted output file should not be empty.");
        }

        [Test]
        public async Task DecryptToStreamAsync_Test_100MB()
        {
            var file_100_MB = await TempFileGenerator.Generate100MBAsync();

            string publicKeyFile = @"C:\Temp\pgp-test-pub.asc";
            string privateKeyFile = @"C:\Temp\pgp-test-sec.asc";
            string passPhrase = "pass";

            string outPgpFile = @$"C:\Temp\{Path.GetFileName(file_100_MB)}.pgp";
            string outOriFile = @$"C:\Temp\{Path.GetFileName(file_100_MB)}.origin";

            // Obtain encrypted stream (assume this returns a readable Stream)
            await using var encryptedStream = await TryIT.Pgp.PgpCrypto.EncryptFileToStreamAsync(
                publicKeyFile,
                file_100_MB
            );

            // Write encrypted stream to disk and ensure the file handle is closed before decrypting.
            // Use a scoped await-using so disposal happens immediately after the copy.
            await using (var outFs = File.Create(outPgpFile))
            {
                await encryptedStream.CopyToAsync(outFs);
            } // outFs closed here -> no file lock when decrypting

            // Now call decrypt which can open the outputFile without file-in-use error.
            await using var decryptedStream = await TryIT.Pgp.PgpCrypto.DecryptToStreamAsync(
                privateKeyFile,
                passPhrase,
                outPgpFile
            );

            Assert.IsNotNull(decryptedStream, "Decrypted stream should not be null");

            // Write the decrypted stream to a separate file for inspection.
            await using (var outFs2 = File.Create(outOriFile))
            {
                await decryptedStream.CopyToAsync(outFs2);
            }

            // Optionally assert the output file exists and has content
            Assert.IsTrue(File.Exists(outOriFile), $"Decrypted output file '{outOriFile}' should exist.");
            Assert.IsTrue(new FileInfo(outOriFile).Length > 0, "Decrypted output file should not be empty.");
        }

        [Test]
        public async Task DecryptToStreamAsync_Test_1_5GB()
        {
            var file_1_5_GB = await TempFileGenerator.Generate1_5GBAsync();

            string publicKeyFile = @"C:\Temp\pgp-test-pub.asc";
            string privateKeyFile = @"C:\Temp\pgp-test-sec.asc";
            string passPhrase = "pass";

            string outPgpFile = @$"C:\Temp\{Path.GetFileName(file_1_5_GB)}.pgp";
            string outOriFile = @$"C:\Temp\{Path.GetFileName(file_1_5_GB)}.origin";

            // Obtain encrypted stream (assume this returns a readable Stream)
            await using var encryptedStream = await TryIT.Pgp.PgpCrypto.EncryptFileToStreamAsync(
                publicKeyFile,
                file_1_5_GB
            );

            // Write encrypted stream to disk and ensure the file handle is closed before decrypting.
            // Use a scoped await-using so disposal happens immediately after the copy.
            await using (var outFs = File.Create(outPgpFile))
            {
                await encryptedStream.CopyToAsync(outFs);
            } // outFs closed here -> no file lock when decrypting

            // Now call decrypt which can open the outputFile without file-in-use error.
            await using var decryptedStream = await TryIT.Pgp.PgpCrypto.DecryptToStreamAsync(
                privateKeyFile,
                passPhrase,
                outPgpFile
            );

            Assert.IsNotNull(decryptedStream, "Decrypted stream should not be null");

            // Write the decrypted stream to a separate file for inspection.
            await using (var outFs2 = File.Create(outOriFile))
            {
                await decryptedStream.CopyToAsync(outFs2);
            }

            // Optionally assert the output file exists and has content
            Assert.IsTrue(File.Exists(outOriFile), $"Decrypted output file '{outOriFile}' should exist.");
            Assert.IsTrue(new FileInfo(outOriFile).Length > 0, "Decrypted output file should not be empty.");
        }
    }
}

