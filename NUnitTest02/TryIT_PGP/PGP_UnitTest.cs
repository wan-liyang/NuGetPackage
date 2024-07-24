using System;
namespace NUnitTest02.TryIT_PGP
{
    public class PGP_UnitTest
    {
        [Test]
        public void EncryptFile_Test()
        {
            string publicKeyFile = @"C:\publicKey.asc";
            string inputFile = @"C:\inputFile.docx";
            //string outputFile = @"C:\inputFile.docx.pgp";

            TryIT.PGP.Lib.EncryptFile(publicKeyFile, inputFile);

            Assert.Pass();
        }

        [Test]
        public void DecryptFile_Test()
        {
            string privateKeyFile = @"C:\privateKey.asc";
            string passPhrase = "";
            string inputFile = @"C:\inputFile.docx.pgp";
            //string outputFile = @"C:\inputFile.docx.pgp";

            TryIT.PGP.Lib.DecryptFile(privateKeyFile, passPhrase, inputFile);

            Assert.Pass();
        }
    }
}

