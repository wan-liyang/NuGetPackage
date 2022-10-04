using System;
using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace TryIT.Azure
{
    /// <summary>
    /// Azure KeyVault related function
    /// </summary>
    public class KeyVault
    {
        private SecretClient secretClient;

        /// <summary>
        /// initial instance with Vault Url
        /// </summary>
        /// <param name="vaultUrl"></param>
        public KeyVault(string vaultUrl)
        {
            secretClient = new SecretClient(new Uri(vaultUrl), new DefaultAzureCredential());
        }

        /// <summary>
        /// initial instance with Vault Url and Credential, e.g. <see cref="ClientCertificateCredential"/>
        /// </summary>
        /// <param name="vaultUrl"></param>
        /// <param name="credential"></param>
        public KeyVault(string vaultUrl, TokenCredential credential)
        {
            secretClient = new SecretClient(new Uri(vaultUrl), credential);
        }

        /// <summary>
        /// get Secret value
        /// </summary>
        /// <param name="secretName"></param>
        /// <returns></returns>
        public string GetSecretValue(string secretName)
        {
            return secretClient.GetSecret(secretName).Value.Value;
        }
    }
}

