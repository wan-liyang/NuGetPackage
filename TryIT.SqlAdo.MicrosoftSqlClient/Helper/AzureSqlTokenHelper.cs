using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Text;
using TryIT.SqlAdo.MicrosoftSqlClient.Models;

namespace TryIT.SqlAdo.MicrosoftSqlClient.Helper
{
    /// <summary>
    /// get azure sql access token for a service principal
    /// </summary>
    public static class AzureSqlTokenHelper
    {
        private static string _cachedToken = null;
        private static DateTimeOffset _tokenExpiry = DateTimeOffset.MinValue;
        private static readonly object _lock = new object();

        /// <summary>
        /// get access token
        /// </summary>
        /// <returns></returns>
        public static string GetToken(AzureServicePrincipal servicePrincipal)
        {
            lock (_lock)
            {
                if (_cachedToken != null && DateTimeOffset.UtcNow < _tokenExpiry.AddMinutes(-5))
                {
                    return _cachedToken;
                }

                var app = ConfidentialClientApplicationBuilder.Create(servicePrincipal.ClientId)
                    .WithClientSecret(servicePrincipal.ClientSecret)
                    .WithAuthority(new Uri($"https://login.microsoftonline.com/{servicePrincipal.TenantId}"))
                    .Build();

                string[] scopes = new[] { "https://database.windows.net/.default" };

                var result = app.AcquireTokenForClient(scopes).ExecuteAsync().GetAwaiter().GetResult();

                _cachedToken = result.AccessToken;
                _tokenExpiry = result.ExpiresOn;

                return _cachedToken;
            }
        }
    }
}
