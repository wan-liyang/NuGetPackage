using Azure.Core;
using Azure.Identity;
using Microsoft.Identity.Client;
using System;
using TryIT.SqlAdo.MicrosoftSqlClient.Models;

namespace TryIT.SqlAdo.MicrosoftSqlClient.Helper
{
    internal class AzureHelper
    {
        /// <summary>
        /// get token credential
        /// </summary>
        /// <param name="azureKeyVaultProvider"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static TokenCredential GetClientSecretCredential(AzureServicePrincipal azureKeyVaultProvider)
        {
            if (azureKeyVaultProvider == null)
            {
                throw new ArgumentNullException(nameof(azureKeyVaultProvider));
            }
            if (string.IsNullOrEmpty(azureKeyVaultProvider.TenantId))
            {
                throw new ArgumentNullException(nameof(azureKeyVaultProvider.TenantId));
            }
            if (string.IsNullOrEmpty(azureKeyVaultProvider.ClientId))
            {
                throw new ArgumentNullException(nameof(azureKeyVaultProvider.ClientId));
            }
            if (string.IsNullOrEmpty(azureKeyVaultProvider.ClientSecret))
            {
                throw new ArgumentNullException(nameof(azureKeyVaultProvider.ClientSecret));
            }

            if (!string.IsNullOrEmpty(azureKeyVaultProvider.ProxyUrl))
            {
                // set environment variable, to solve Retry failed after 4 tries. Retry settings can be adjusted in ClientOptions.Retry. (The SSL connection could not be established) accessing keyvault
                Environment.SetEnvironmentVariable("HTTP_PROXY", azureKeyVaultProvider.ProxyUrl);
                Environment.SetEnvironmentVariable("HTTPS_PROXY", azureKeyVaultProvider.ProxyUrl);
            }            

            return new ClientSecretCredential(azureKeyVaultProvider.TenantId, azureKeyVaultProvider.ClientId, azureKeyVaultProvider.ClientSecret);
        }
    }
}
