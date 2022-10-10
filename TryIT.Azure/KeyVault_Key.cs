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

        // public string Encrypt(CryptographyClient cryptoClient, string plaintext)
        // {
        //     byte[] _byte = Encoding.UTF8.GetBytes(plaintext);
        //     var result = cryptoClient.Encrypt(EncryptionAlgorithm.RsaOaep256, _byte).Ciphertext;
        //     return Encoding.UTF8.GetString(result);
        // }

        // public string Decrypt(CryptographyClient cryptoClient, string ciphertext)
        // {
        //     byte[] _byte = Encoding.UTF8.GetBytes(ciphertext);
        //     var result = cryptoClient.Decrypt(EncryptionAlgorithm.RsaOaep256, _byte).Plaintext;
        //     return Encoding.UTF8.GetString(result);
        // }

        // public string Encrypt(string keyName, string plaintext)
        // {
        //     byte[] _byte = Encoding.UTF8.GetBytes(plaintext);
        //     var result = Encrypt(keyName, _byte);
        //     return Encoding.UTF8.GetString(result);
        // }

        // public byte[] Encrypt(string keyName, byte[] plaintext)
        // {
        //     var cryptoClient = keyClient.GetCryptographyClient(keyName);

        //     return cryptoClient.Encrypt(EncryptionAlgorithm.RsaOaep256, plaintext).Ciphertext;
        // }

        // public string Decrypt(string keyName, string ciphertext)
        // {
        //     byte[] _byte = Encoding.UTF8.GetBytes(ciphertext);
        //     var result = Decrypt(keyName, _byte);
        //     return Encoding.UTF8.GetString(result);
        // }
        // public byte[] Decrypt(string keyName, byte[] ciphertext)
        // {
        //     var cryptoClient = keyClient.GetCryptographyClient(keyName);

        //     return cryptoClient.Decrypt(EncryptionAlgorithm.RsaOaep256, ciphertext).Plaintext;
        // }
    }
}

