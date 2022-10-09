using System;
using System.Security.Cryptography;

namespace TryIT.CryptoService
{
    /// <summary>
    /// perform Encryption or Decryption with RSA based on Public and Private key pair
    /// </summary>
    public class RSAEncryption
    {
        /// <summary>
        /// encrypt data with public key (key needs in Xml format, as .NET Standard only support Xml)
        /// </summary>
        /// <param name="clearData">clear data to be encrypt</param>
        /// <param name="publicKeyInXml">public key</param>
        /// <returns></returns>
        public static byte[] RSAEncrypt(byte[] clearData, string publicKeyInXml)
        {
            using(RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(publicKeyInXml);
                return rsa.Encrypt(clearData, false);
            }
        }

        /// <summary>
        /// decrypt data with public key (key needs in Xml format, as .NET Standard only support Xml)
        /// </summary>
        /// <param name="encryptedData">enrypted data to be decrypt</param>
        /// <param name="privateKeyInXml">private key</param>
        /// <returns></returns>
        public static byte[] RSADecrypt(byte[] encryptedData, string privateKeyInXml)
        {
            using(RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(privateKeyInXml);
                return rsa.Decrypt(encryptedData, false);
            }
        }
    }
}

