using System;
using System.Text;
using Azure.Core;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Keys.Cryptography;

namespace TryIT.Azure
{
	public class KeyVault_Key
	{
        private KeyClient keyClient;

        /// <summary>
        /// initial instance with Vault Url and Credential, e.g. <see cref="ClientCertificateCredential"/>
        /// </summary>
        /// <param name="vaultUrl"></param>
        /// <param name="credential"></param>
        public KeyVault_Key(string vaultUrl, TokenCredential credential)
        {
            keyClient = new KeyClient(new Uri(vaultUrl), credential);
        }

        public string Encrypt(string keyName, string plaintext)
        {
            var cryptoClient = keyClient.GetCryptographyClient(keyName);
            byte[] plaintextByte = Encoding.UTF8.GetBytes(plaintext);

            cryptoClient.Encrypt(EncryptionAlgorithm.RsaOaep256, plaintextByte);
        }
    }
}

