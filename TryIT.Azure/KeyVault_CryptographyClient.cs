using System;
using System.Text;
using Azure.Core;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Keys.Cryptography;

namespace TryIT.Azure
{
	public class KeyVault_CryptographyClient
	{
        private CryptographyClient cryptoClient;
        internal KeyVault_CryptographyClient(CryptographyClient cryptoClient)
        {
            this.cryptoClient = cryptoClient;
        }

        /// <summary>
        /// do encryption, return byte value, when do decryption needs pass same byte value
        /// </summary>
        /// <param name="plaintext"></param>
        public byte[] Encrypt(string plaintext)
        {
            byte[] _byte = Encoding.UTF8.GetBytes(plaintext);
            return cryptoClient.Encrypt(EncryptionAlgorithm.RsaOaep256, _byte).Ciphertext;
        }

        /// <summary>
        /// do decryption, required byte value from Encrypt method, return byte value for convert to expected result
        /// </summary>
        /// <param name="cipherValue"></param>
        public byte[] Decrypt(byte[] cipherValue)
        {
            return cryptoClient.Decrypt(EncryptionAlgorithm.RsaOaep256, cipherValue).Plaintext;
        }
    }
}