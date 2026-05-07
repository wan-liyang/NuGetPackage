using Microsoft.Identity.Client;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using TryIT.SqlAdo.MicrosoftSqlClient.Models;

namespace TryIT.SqlAdo.MicrosoftSqlClient.Helper
{
    /// <summary>
    /// Provides Azure SQL access tokens
    /// using Azure AD service principal authentication.
    /// </summary>
    public static class AzureSqlTokenHelper
    {
        /// <summary>
        /// Azure SQL scope
        /// </summary>
        private static readonly string[] _scopes =
        {
            "https://database.windows.net/.default"
        };

        /// <summary>
        /// Cache MSAL application instances by tenant/client combination
        /// </summary>
        private static readonly ConcurrentDictionary<string, Lazy<IConfidentialClientApplication>> _applications = new ConcurrentDictionary<string, Lazy<IConfidentialClientApplication>>();

        /// <summary>
        /// Get access token
        /// </summary>
        /// <param name="servicePrincipal">Azure service principal</param>
        /// <returns>Access token</returns>
        public static async Task<string> GetTokenAsync(AzureServicePrincipal servicePrincipal)
        {
            if (servicePrincipal == null)
            {
                throw new ArgumentNullException(nameof(servicePrincipal));
            }

            ValidateServicePrincipal(servicePrincipal);

            string cacheKey = BuildCacheKey(servicePrincipal);

            var app = _applications.GetOrAdd(
                cacheKey,
                _ => new Lazy<IConfidentialClientApplication>(
                    () => BuildApplication(servicePrincipal),
                    LazyThreadSafetyMode.ExecutionAndPublication))
                .Value;

            var result = await app
                .AcquireTokenForClient(_scopes)
                .ExecuteAsync()
                .ConfigureAwait(false);

            return result.AccessToken;
        }

        /// <summary>
        /// Get access token (synchronous wrapper)
        /// </summary>
        /// <param name="servicePrincipal">Azure service principal</param>
        /// <returns>Access token</returns>
        public static string GetToken(AzureServicePrincipal servicePrincipal)
        {
            return GetTokenAsync(servicePrincipal)
                .GetAwaiter()
                .GetResult();
        }

        /// <summary>
        /// Build MSAL confidential client application
        /// </summary>
        private static IConfidentialClientApplication BuildApplication(AzureServicePrincipal servicePrincipal)
        {
            return ConfidentialClientApplicationBuilder
                .Create(servicePrincipal.ClientId)
                .WithClientSecret(servicePrincipal.ClientSecret)
                .WithAuthority($"https://login.microsoftonline.com/{servicePrincipal.TenantId}")
                .Build();
        }

        /// <summary>
        /// Build cache key
        /// </summary>
        private static string BuildCacheKey(AzureServicePrincipal servicePrincipal)
        {
            return $"{servicePrincipal.TenantId}|{servicePrincipal.ClientId}";
        }

        /// <summary>
        /// Validate service principal configuration
        /// </summary>
        private static void ValidateServicePrincipal(AzureServicePrincipal servicePrincipal)
        {
            if (string.IsNullOrWhiteSpace(servicePrincipal.TenantId))
            {
                throw new ArgumentException("TenantId is required.");
            }

            if (string.IsNullOrWhiteSpace(servicePrincipal.ClientId))
            {
                throw new ArgumentException("ClientId is required.");
            }

            if (string.IsNullOrWhiteSpace(servicePrincipal.ClientSecret))
            {
                throw new ArgumentException("ClientSecret is required.");
            }
        }
    }
}