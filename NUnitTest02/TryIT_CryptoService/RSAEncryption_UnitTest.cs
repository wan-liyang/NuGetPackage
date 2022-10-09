using System;
using System.Security.Cryptography;
using System.Text;

namespace NUnitTest02.TryIT_CryptoService
{
    public class RSAEncryption_Test
    {
        [Test]
        public void RSA_Test()
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

            var publicKey = rsa.ToXmlString(false);
            var privateKey = rsa.ToXmlString(true);

            string text = "hello world";

            byte[] _byte = Encoding.UTF8.GetBytes(text);

            byte[] encryptedText = TryIT.CryptoService.RSAEncryption.RSAEncrypt(_byte, publicKey);

            byte[] decryptedText = TryIT.CryptoService.RSAEncryption.RSADecrypt(encryptedText, privateKey);

            string finalText = Encoding.UTF8.GetString(decryptedText);

            Assert.That(finalText, Is.EqualTo(text));
        }
    }
}

