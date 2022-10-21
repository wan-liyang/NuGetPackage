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

        public KeyVault_CryptographyClient GetCryptographyClient(string keyName, string keyVersion = null)
        {
            var client = keyClient.GetCryptographyClient(keyName);
            return new KeyVault_CryptographyClient(client);
        }

        /// <summary>
        /// get Key
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public KeyVaultKey GetKey(string keyName, string version = null)
        {
            return keyClient.GetKey(keyName, version);
        }
    }
}

