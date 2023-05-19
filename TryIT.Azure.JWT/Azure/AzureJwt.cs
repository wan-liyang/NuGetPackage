using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using TryIT.JWT.Helper;

namespace TryIT.JWT.Azure
{
	public class AzureJwt
	{
        private static HttpClient _httpClient;
        private RequestModel _request;
        public AzureJwt(RequestModel request)
		{
            if (string.IsNullOrEmpty(request.TenantId))
            {
                throw new System.ArgumentNullException(nameof(request.TenantId));
            }
            if (string.IsNullOrEmpty(request.ClientId))
            {
                throw new System.ArgumentNullException(nameof(request.ClientId));
            }
            if (string.IsNullOrEmpty(request.ClientSecret))
            {
                throw new System.ArgumentNullException(nameof(request.ClientSecret));
            }
            if (string.IsNullOrEmpty(request.GrantType))
            {
                throw new System.ArgumentNullException(nameof(request.GrantType));
            }
            if (string.IsNullOrEmpty(request.Scope))
            {
                throw new System.ArgumentNullException(nameof(request.Scope));
            }

            _request = request;

            WebProxy proxy = WebProxyHelper.GetProxy(request.Proxy_Url, request.Proxy_Username, request.Proxy_Password);

            HttpClientHandler clientHandler = new HttpClientHandler()
            {
                AllowAutoRedirect = true,
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
            };

            if (proxy != null)
            {
                clientHandler.Proxy = proxy;
            }

            _httpClient = new HttpClient(clientHandler);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
        }

		public HttpResponse GetToken()
        {
            try
            {
                string url = _request.TokenUrl.Replace("{{tenant-id}}", _request.TenantId);

                var formContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("client_id", _request.ClientId),
                    new KeyValuePair<string, string>("client_secret", _request.ClientSecret),
                    new KeyValuePair<string, string>("grant_type", _request.GrantType),
                    new KeyValuePair<string, string>("scope", _request.Scope)
                });

                HttpResponseMessage response = _httpClient.PostAsync(url, formContent).GetAwaiter().GetResult();

                string content = response.Content.ReadAsStringAsync().Result;

                return content.JsonToObject<HttpResponse>();
            }
            catch
            {
                throw;
            }
        }


        public void Validate2(string jwtToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwtToken);

            var iss = token.Issuer;
            // var tfp = token.Payload["tfp"].ToString(); // Sign-in policy name
            var metadataEndpoint = $"{iss}.well-known/openid-configuration";

            var cm = new ConfigurationManager<OpenIdConnectConfiguration>(metadataEndpoint,
                            new OpenIdConnectConfigurationRetriever(),
                            new HttpDocumentRetriever());
            var discoveryDocument = cm.GetConfigurationAsync().GetAwaiter().GetResult();
            var signingKeys = discoveryDocument.SigningKeys;

            var tvp = new TokenValidationParameters()
            {
                ValidateAudience = true,
                ValidAudiences = token.Audiences,
                ValidateIssuer = true,
                ValidIssuers = new string[] { token.Issuer },
                RequireSignedTokens = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                // Allow for some drift in server time. A lower value is better, two minutes or less is recommended.
                ClockSkew = TimeSpan.FromMinutes(2),
                IssuerSigningKeys = signingKeys
            };

            try
            {
                SecurityToken validatedToken = new JwtSecurityToken();
                ClaimsPrincipal claimsPrincipal = null;
                var tokenHandler = new JwtSecurityTokenHandler();
                claimsPrincipal = tokenHandler.ValidateToken(jwtToken, tvp, out validatedToken);
                //context.User = claimsPrincipal;
            }
            catch (SecurityTokenValidationException e)
            {
                //context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                //logger.LogInformation($"Token validation failed!");
                return;
            }
            catch (Exception e)
            {
                //context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                //logger.LogInformation($"Unknown exception during validation!");
                return;
            }
        }

        public void Validate3(string jwtToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwtToken);

            RSA rsa = RSA.Create();

            string pubkey = "MIIDBTCCAe2gAwIBAgIQGQ6YG6NleJxJGDRAwAd/ZTANBgkqhkiG9w0BAQsFADAtMSswKQYDVQQDEyJhY2NvdW50cy5hY2Nlc3Njb250cm9sLndpbmRvd3MubmV0MB4XDTIyMTAwMjE4MDY0OVoXDTI3MTAwMjE4MDY0OVowLTErMCkGA1UEAxMiYWNjb3VudHMuYWNjZXNzY29udHJvbC53aW5kb3dzLm5ldDCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBALSS+lq9iVLMS8jXsz0IdSes5+sEqAwIYEWEg5GjLhB8u+VYpIgfMINuVrkfeoHTKaKJHZUb4e0p0b7Y0DfW+ZuMyQjKUkXCeQ7l5eJnHewoN2adQufiZjKvCe5uzkvR6VEGwNcobQh6j+1wOFJ0CNvCfk5xogGt74jy5atOutwquoUMO42KOcjY3SXFefhUvsTVe1B0eMwDEa7jFB8bXtSGSc2yZsYyqBIycA07XHeg5CN8q5JmLfBnUJrtGAR0yUmYs/jNdAmNy27y83/rWwTSkP4H5xhihezL0QpjwP2BfwD8p6yBu6eLzw0V4aRt/wiLd9ezcrxqCMIr9ALfN5ECAwEAAaMhMB8wHQYDVR0OBBYEFJcSH+6Eaqucndn9DDu7Pym7OA8rMA0GCSqGSIb3DQEBCwUAA4IBAQADKkY0PIyslgWGmRDKpp/5PqzzM9+TNDhXzk6pw8aESWoLPJo90RgTJVf8uIj3YSic89m4ftZdmGFXwHcFC91aFe3PiDgCiteDkeH8KrrpZSve1pcM4SNjxwwmIKlJdrbcaJfWRsSoGFjzbFgOecISiVaJ9ZWpb89/+BeAz1Zpmu8DSyY22dG/K6ZDx5qNFg8pehdOUYY24oMamd4J2u2lUgkCKGBZMQgBZFwk+q7H86B/byGuTDEizLjGPTY/sMms1FAX55xBydxrADAer/pKrOF1v7Dq9C1Z9QVcm5D9G4DcenyWUdMyK43NXbVQLPxLOng51KO9icp2j4U7pwHP";

            byte[] pub = Convert.FromBase64String(pubkey);

            string cert = "-----BEGIN CERTIFICATE-----\nMIIDBTCCAe2gAwIBAgIQGQ6YG6NleJxJGDRAwAd/ZTANBgkqhkiG9w0BAQsFADAtMSswKQYDVQQDEyJhY2NvdW50cy5hY2Nlc3Njb250cm9sLndpbmRvd3MubmV0MB4XDTIyMTAwMjE4MDY0OVoXDTI3MTAwMjE4MDY0OVowLTErMCkGA1UEAxMiYWNjb3VudHMuYWNjZXNzY29udHJvbC53aW5kb3dzLm5ldDCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBALSS+lq9iVLMS8jXsz0IdSes5+sEqAwIYEWEg5GjLhB8u+VYpIgfMINuVrkfeoHTKaKJHZUb4e0p0b7Y0DfW+ZuMyQjKUkXCeQ7l5eJnHewoN2adQufiZjKvCe5uzkvR6VEGwNcobQh6j+1wOFJ0CNvCfk5xogGt74jy5atOutwquoUMO42KOcjY3SXFefhUvsTVe1B0eMwDEa7jFB8bXtSGSc2yZsYyqBIycA07XHeg5CN8q5JmLfBnUJrtGAR0yUmYs/jNdAmNy27y83/rWwTSkP4H5xhihezL0QpjwP2BfwD8p6yBu6eLzw0V4aRt/wiLd9ezcrxqCMIr9ALfN5ECAwEAAaMhMB8wHQYDVR0OBBYEFJcSH+6Eaqucndn9DDu7Pym7OA8rMA0GCSqGSIb3DQEBCwUAA4IBAQADKkY0PIyslgWGmRDKpp/5PqzzM9+TNDhXzk6pw8aESWoLPJo90RgTJVf8uIj3YSic89m4ftZdmGFXwHcFC91aFe3PiDgCiteDkeH8KrrpZSve1pcM4SNjxwwmIKlJdrbcaJfWRsSoGFjzbFgOecISiVaJ9ZWpb89/+BeAz1Zpmu8DSyY22dG/K6ZDx5qNFg8pehdOUYY24oMamd4J2u2lUgkCKGBZMQgBZFwk+q7H86B/byGuTDEizLjGPTY/sMms1FAX55xBydxrADAer/pKrOF1v7Dq9C1Z9QVcm5D9G4DcenyWUdMyK43NXbVQLPxLOng51KO9icp2j4U7pwHP\n-----END CERTIFICATE-----";

            //byte[] pub = Convert.FromBase64String("-----BEGIN CERTIFICATE-----\nMIIDBTCCAe2gAwIBAgIQGQ6YG6NleJxJGDRAwAd/ZTANBgkqhkiG9w0BAQsFADAtMSswKQYDVQQDEyJhY2NvdW50cy5hY2Nlc3Njb250cm9sLndpbmRvd3MubmV0MB4XDTIyMTAwMjE4MDY0OVoXDTI3MTAwMjE4MDY0OVowLTErMCkGA1UEAxMiYWNjb3VudHMuYWNjZXNzY29udHJvbC53aW5kb3dzLm5ldDCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBALSS+lq9iVLMS8jXsz0IdSes5+sEqAwIYEWEg5GjLhB8u+VYpIgfMINuVrkfeoHTKaKJHZUb4e0p0b7Y0DfW+ZuMyQjKUkXCeQ7l5eJnHewoN2adQufiZjKvCe5uzkvR6VEGwNcobQh6j+1wOFJ0CNvCfk5xogGt74jy5atOutwquoUMO42KOcjY3SXFefhUvsTVe1B0eMwDEa7jFB8bXtSGSc2yZsYyqBIycA07XHeg5CN8q5JmLfBnUJrtGAR0yUmYs/jNdAmNy27y83/rWwTSkP4H5xhihezL0QpjwP2BfwD8p6yBu6eLzw0V4aRt/wiLd9ezcrxqCMIr9ALfN5ECAwEAAaMhMB8wHQYDVR0OBBYEFJcSH+6Eaqucndn9DDu7Pym7OA8rMA0GCSqGSIb3DQEBCwUAA4IBAQADKkY0PIyslgWGmRDKpp/5PqzzM9+TNDhXzk6pw8aESWoLPJo90RgTJVf8uIj3YSic89m4ftZdmGFXwHcFC91aFe3PiDgCiteDkeH8KrrpZSve1pcM4SNjxwwmIKlJdrbcaJfWRsSoGFjzbFgOecISiVaJ9ZWpb89/+BeAz1Zpmu8DSyY22dG/K6ZDx5qNFg8pehdOUYY24oMamd4J2u2lUgkCKGBZMQgBZFwk+q7H86B/byGuTDEizLjGPTY/sMms1FAX55xBydxrADAer/pKrOF1v7Dq9C1Z9QVcm5D9G4DcenyWUdMyK43NXbVQLPxLOng51KO9icp2j4U7pwHP\n-----END CERTIFICATE-----");

            //rsa.ImportSubjectPublicKeyInfo(pub, out _);
            rsa.ImportFromPem(cert);

            byte[] data = Convert.FromBase64String(token.RawData);
            byte[] sign = Convert.FromBase64String(token.RawSignature);

            bool isTrue = rsa.VerifyData(data, sign, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }

        public JwtSecurityToken Validate(string token)
        {
            string stsDiscoveryEndpoint = "https://login.microsoftonline.com/common/v2.0/.well-known/openid-configuration";
            //string stsDiscoveryEndpoint = "https://login.microsoftonline.com/common/.well-known/openid-configuration";
            //string stsDiscoveryEndpoint = "https://login.microsoftonline.com/213055c2-9576-46cf-824c-38abe5799cc8/discovery/v2.0/keys";

            ConfigurationManager<OpenIdConnectConfiguration> configManager = new ConfigurationManager<OpenIdConnectConfiguration>(stsDiscoveryEndpoint, new OpenIdConnectConfigurationRetriever());

            OpenIdConnectConfiguration config = configManager.GetConfigurationAsync().GetAwaiter().GetResult();


            string key = @"-----BEGIN CERTIFICATE-----
MIIDBTCCAe2gAwIBAgIQGQ6YG6NleJxJGDRAwAd/ZTANBgkqhkiG9w0BAQsFADAtMSswKQYDVQQDEyJhY2NvdW50cy5hY2Nlc3Njb250cm9sLndpbmRvd3MubmV0MB4XDTIyMTAwMjE4MDY0OVoXDTI3MTAwMjE4MDY0OVowLTErMCkGA1UEAxMiYWNjb3VudHMuYWNjZXNzY29udHJvbC53aW5kb3dzLm5ldDCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBALSS+lq9iVLMS8jXsz0IdSes5+sEqAwIYEWEg5GjLhB8u+VYpIgfMINuVrkfeoHTKaKJHZUb4e0p0b7Y0DfW+ZuMyQjKUkXCeQ7l5eJnHewoN2adQufiZjKvCe5uzkvR6VEGwNcobQh6j+1wOFJ0CNvCfk5xogGt74jy5atOutwquoUMO42KOcjY3SXFefhUvsTVe1B0eMwDEa7jFB8bXtSGSc2yZsYyqBIycA07XHeg5CN8q5JmLfBnUJrtGAR0yUmYs/jNdAmNy27y83/rWwTSkP4H5xhihezL0QpjwP2BfwD8p6yBu6eLzw0V4aRt/wiLd9ezcrxqCMIr9ALfN5ECAwEAAaMhMB8wHQYDVR0OBBYEFJcSH+6Eaqucndn9DDu7Pym7OA8rMA0GCSqGSIb3DQEBCwUAA4IBAQADKkY0PIyslgWGmRDKpp/5PqzzM9+TNDhXzk6pw8aESWoLPJo90RgTJVf8uIj3YSic89m4ftZdmGFXwHcFC91aFe3PiDgCiteDkeH8KrrpZSve1pcM4SNjxwwmIKlJdrbcaJfWRsSoGFjzbFgOecISiVaJ9ZWpb89/+BeAz1Zpmu8DSyY22dG/K6ZDx5qNFg8pehdOUYY24oMamd4J2u2lUgkCKGBZMQgBZFwk+q7H86B/byGuTDEizLjGPTY/sMms1FAX55xBydxrADAer/pKrOF1v7Dq9C1Z9QVcm5D9G4DcenyWUdMyK43NXbVQLPxLOng51KO9icp2j4U7pwHP
-----END CERTIFICATE-----";

            var validKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            TokenValidationParameters validationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                //IssuerSigningTokens = config.SigningTokens,
                ValidateLifetime = false,
                IssuerSigningKeys = config.SigningKeys,
                //IssuerSigningKey = validKey,
                //ValidateIssuerSigningKey = true,
                //TryAllIssuerSigningKeys = true
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            SecurityToken jwt;
            IdentityModelEventSource.ShowPII = true;
            var result = tokenHandler.ValidateToken(token, validationParameters, out jwt);

            return jwt as JwtSecurityToken;
        }

        public void Validate5(string jwtToken)
        {
            //string stsDiscoveryEndpoint = "https://login.microsoftonline.com/common/v2.0/.well-known/openid-configuration";
            string stsDiscoveryEndpoint = "https://login.microsoftonline.com/common/.well-known/openid-configuration";
            //string stsDiscoveryEndpoint = "https://login.microsoftonline.com/213055c2-9576-46cf-824c-38abe5799cc8/discovery/v2.0/keys";

            ConfigurationManager<OpenIdConnectConfiguration> configManager = new ConfigurationManager<OpenIdConnectConfiguration>(stsDiscoveryEndpoint, new OpenIdConnectConfigurationRetriever());

            OpenIdConnectConfiguration config = configManager.GetConfigurationAsync().GetAwaiter().GetResult();

            var handler = new JwtSecurityTokenHandler();

            JwtSecurityToken outerJWT = handler.ReadJwtToken(jwtToken);
            JwtHeader header = outerJWT.Header;
            //string x5c = header.X5c; // <-- This is not working!!!
            string x5c = "MIIDBTCCAe2gAwIBAgIQGQ6YG6NleJxJGDRAwAd/ZTANBgkqhkiG9w0BAQsFADAtMSswKQYDVQQDEyJhY2NvdW50cy5hY2Nlc3Njb250cm9sLndpbmRvd3MubmV0MB4XDTIyMTAwMjE4MDY0OVoXDTI3MTAwMjE4MDY0OVowLTErMCkGA1UEAxMiYWNjb3VudHMuYWNjZXNzY29udHJvbC53aW5kb3dzLm5ldDCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBALSS+lq9iVLMS8jXsz0IdSes5+sEqAwIYEWEg5GjLhB8u+VYpIgfMINuVrkfeoHTKaKJHZUb4e0p0b7Y0DfW+ZuMyQjKUkXCeQ7l5eJnHewoN2adQufiZjKvCe5uzkvR6VEGwNcobQh6j+1wOFJ0CNvCfk5xogGt74jy5atOutwquoUMO42KOcjY3SXFefhUvsTVe1B0eMwDEa7jFB8bXtSGSc2yZsYyqBIycA07XHeg5CN8q5JmLfBnUJrtGAR0yUmYs/jNdAmNy27y83/rWwTSkP4H5xhihezL0QpjwP2BfwD8p6yBu6eLzw0V4aRt/wiLd9ezcrxqCMIr9ALfN5ECAwEAAaMhMB8wHQYDVR0OBBYEFJcSH+6Eaqucndn9DDu7Pym7OA8rMA0GCSqGSIb3DQEBCwUAA4IBAQADKkY0PIyslgWGmRDKpp/5PqzzM9+TNDhXzk6pw8aESWoLPJo90RgTJVf8uIj3YSic89m4ftZdmGFXwHcFC91aFe3PiDgCiteDkeH8KrrpZSve1pcM4SNjxwwmIKlJdrbcaJfWRsSoGFjzbFgOecISiVaJ9ZWpb89/+BeAz1Zpmu8DSyY22dG/K6ZDx5qNFg8pehdOUYY24oMamd4J2u2lUgkCKGBZMQgBZFwk+q7H86B/byGuTDEizLjGPTY/sMms1FAX55xBydxrADAer/pKrOF1v7Dq9C1Z9QVcm5D9G4DcenyWUdMyK43NXbVQLPxLOng51KO9icp2j4U7pwHP";

            var certificate = new X509Certificate2(Convert.FromBase64String(x5c));

            TokenValidationParameters validationParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                ValidateLifetime = false,
                ValidateAudience = false,
                ValidateIssuer = false,
                //IssuerSigningKeyResolver = (t, st, i, p) => new[] {
                //    new X509SecurityKey(certificate)
                //},
                //IssuerSigningKey = new X509SecurityKey(certificate),
                IssuerSigningKeys = config.SigningKeys
                
            };

            SecurityToken validatedSecurityToken = null;
            try
            {

                IdentityModelEventSource.ShowPII = true;
                var cp = handler.ValidateToken(jwtToken, validationParameters, out validatedSecurityToken);
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }

            var a = validatedSecurityToken;
        }

        public void Validate4(string jwtToken)
        {
            string token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImtpZCI6IkN0VHVoTUptRDVNN0RMZHpEMnYyeDNRS1NSWSJ9.eyJhdWQiOiI3YjFjZTFhZC1hZjE1LTRlNWYtOWFlNC1hYWYwYTY4YTdhYjQiLCJpc3MiOiJodHRwczovL2xvZ2luLm1pY3Jvc29mdG9ubGluZS5jb20vZThlNmQwMTgtYTgzNC00MDZiLTlmNDMtMmU5NGFlNDI1ODc2L3YyLjAiLCJpYXQiOjE1ODkyODQ2OTEsIm5iZiI6MTU4OTI4NDY5MSwiZXhwIjoxNTg5Mjg4NTkxLCJhaW8iOiJBVVFBdS84UEFBQUEyNWpRNzJBc3IyWHBYMEJlUkZRNU1lTTdSLy8zbnpIbUxDUHNYekJYRWZpSGlkQWM4Y0RPNHJoUUVEdk56OWtnRTdPK1pYbmxNTTVRNmk4RjZYY0hLZz09IiwibmFtZSI6IlZpamFpIEFuYW5kIFJhbWFsaW5nYW0iLCJub25jZSI6IjY1OWM5MjU0LTQyN2YtNDg5MC05ODQ5LTU0ZTk1Yjc0NDYyNCIsIm9pZCI6ImU2YmFkYTg2LTk4NDktNGFhNC1hZWQ0LTg5YzZlZmE5YTc0MSIsInByZWZlcnJlZF91c2VybmFtZSI6InZpamFpYW5hbmRAQzk4Ni5vbm1pY3Jvc29mdC5jb20iLCJzdWIiOiJIdjhtQ3RDVkx1NW8wYklrSDJVd2RCTnVUWTlqeC1RNUU4LTVuYU5pdkFJIiwidGlkIjoiZThlNmQwMTgtYTgzNC00MDZiLTlmNDMtMmU5NGFlNDI1ODc2IiwidXRpIjoiVml0alZEcVh5RS0yaWNLQUlRT19BQSIsInZlciI6IjIuMCJ9.UAT3FkgCBYqM7Mfux1V-yF1QTqg0Dlz4Y2G8VQqNqg3WXWdQWf8v4MHcrZVzycV6FSA0-C4ANRpkBxeX1mdmtic4l6e5onOsRS3r_PsWpp7mew_XlTt9TQ1W1pO5dn6lw6J4U3k41kmXVAPwH9hbZNEmVVM6KjNQLW-SdCfaJJIB0XVIqEK2HOlBPxSI8hugh9S5yRMYz6-xi7SrG-wQJtsa9s7Wz5O4FYW2YmjHdUIdj_xwJbfS6_rknJetO756okz4tHY70N3GAKlr_zvfXvuAMjXfsXQNQN5-TQnDcWVkvK6SrhCGQunlPmjvvTvJyp7KLZVrRhxnz8w98yaEfA";
            string myTenant = "e8e6d018-a834-406b-9f43-2e94ae425876";
            var myAudience = "7b1ce1ad-af15-4e5f-9ae4-aaf0a68a7ab4";
            var myIssuer = String.Format(CultureInfo.InvariantCulture, "https://login.microsoftonline.com/{0}/v2.0", myTenant);
            var mySecret = "t.GDqjoBYBhB.tRC@lbq1GdslFjk8=57";
            var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(mySecret));
            var stsDiscoveryEndpoint = String.Format(CultureInfo.InvariantCulture, "https://login.microsoftonline.com/{0}/.well-known/openid-configuration", myTenant);
            var configManager = new ConfigurationManager<OpenIdConnectConfiguration>(stsDiscoveryEndpoint, new OpenIdConnectConfigurationRetriever());
            var config = configManager.GetConfigurationAsync().GetAwaiter().GetResult();

            var tokenHandler = new JwtSecurityTokenHandler();

            var validationParameters = new TokenValidationParameters
            {
                ValidAudience = myAudience,
                ValidIssuer = myIssuer,
                IssuerSigningKeys = config.SigningKeys,
                ValidateLifetime = false,
                IssuerSigningKey = mySecurityKey
            };

            var validatedToken = (SecurityToken)new JwtSecurityToken();

            // Throws an Exception as the token is invalid (expired, invalid-formatted, etc.)  
            tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
            Console.WriteLine(validatedToken);
        }
    }
}

