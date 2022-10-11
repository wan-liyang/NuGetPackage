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

        public string Encrypt(string plaintext)
        {
            byte[] _byte = Encoding.UTF8.GetBytes(plaintext);
            var result = cryptoClient.Encrypt(EncryptionAlgorithm.RsaOaep256, _byte).Ciphertext;
            return Encoding.UTF8.GetString(result);
        }

        public string Decrypt(string ciphertext)
        {
            byte[] _byte = Encoding.UTF8.GetBytes(ciphertext);
            var result = cryptoClient.Decrypt(EncryptionAlgorithm.RsaOaep256, _byte).Plaintext;
            return Encoding.UTF8.GetString(result);
        }
    }
}