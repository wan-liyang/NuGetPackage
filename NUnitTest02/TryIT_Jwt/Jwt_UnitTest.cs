using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using TryIT.Jwt;
using TryIT.JWT.Azure;
using System.Reflection.Emit;

namespace NUnitTest02.TryIT_JWT
{
	public class AzureJwt_UnitTest
	{
        [Test]
        public void Jwt_Test()
        {
            Jwt jwt = new Jwt(new JwtParameter
            {
                Issuer = "testaa",
                Audience = "testdd",
                TokenSecret = Encoding.UTF8.GetBytes("You_Need_To_Provide_A_Longer_Secret_Key_Here"),
                CustomClaims = new List<CustomClaim>
                {
                    new("userid", 1.1),
                    new("username", "test"),
                    new("isman", false),
                    new("is18", true)
                }
            });

            var token = jwt.GenerateToken();

            var result = jwt.ValidateToken(token);

            var claim1 = Jwt.GetClaimValue(token, "userid");
            var claim2 = Jwt.GetClaimValue(token, "is19");

            Console.WriteLine(token);
        }

        [Test]
        public void AzureJwt_Test()
        {
            RequestModel request = new RequestModel
            {
                TenantId = "213055c2-9576-46cf-824c-38abe5799cc8",
                ClientId = "66212106-3838-4908-ba69-4b2d0e93ddcc",
                ClientSecret = "aU28Q~jr14NzT4_7v9On5l_6PYXF0NK_TeQNoa~0"
            };

            AzureJwt jwt = new AzureJwt(request);
            var token = jwt.GetToken();

            Console.Write(token);

            jwt.Validate5(token.access_token);

            //jwt.Validate(token.access_token);
        }

        [Test]
        public void ValidateToken()
        {
            string jwt = "eyJ0eXAiOiJKV1QiLCJub25jZSI6Ilgtb1RYWFRoN2NGSEhLODJoV2pJMWRuSWMtUUJPYlRvT2M1OVh6ekNUWlEiLCJhbGciOiJSUzI1NiIsIng1dCI6Ii1LSTNROW5OUjdiUm9meG1lWm9YcWJIWkdldyIsImtpZCI6Ii1LSTNROW5OUjdiUm9meG1lWm9YcWJIWkdldyJ9.eyJhdWQiOiIwMDAwMDAwMy0wMDAwLTAwMDAtYzAwMC0wMDAwMDAwMDAwMDAiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC9jYTkwZDhmNS04OTYzLTRiNmUtYmNhOS05YWM0NjhiY2M3YTgvIiwiaWF0IjoxNjkyMTk2NjgzLCJuYmYiOjE2OTIxOTY2ODMsImV4cCI6MTY5MjIwMTYzMCwiYWNjdCI6MCwiYWNyIjoiMSIsImFpbyI6IkFWUUFxLzhVQUFBQWh4Qkwxdkg3bUNUSHBiTVlKUVA5YmN1amwyTVl4bWdzNmpOSGdHNkw1MVJTRlF3RWhwanNEN0VLQlVVRC9nb2ZwMzZ0bzFiZHUvdnFwRnFGK2dKMmNBdnpWWStlUWVSZDlDUDVhVDE0Sm9BPSIsImFtciI6WyJ3aWEiLCJtZmEiXSwiYXBwX2Rpc3BsYXluYW1lIjoibmNzaXQtZXRsLXByZC1hcHAiLCJhcHBpZCI6IjQ5NzEwODNlLWRmMjktNDE4ZS1iNjAzLWM5ZmZhNTg3ZmQ2NyIsImFwcGlkYWNyIjoiMSIsImZhbWlseV9uYW1lIjoiV2FuIiwiZ2l2ZW5fbmFtZSI6IkxpeWFuZyIsImlkdHlwIjoidXNlciIsImluX2NvcnAiOiJ0cnVlIiwiaXBhZGRyIjoiMTY1LjIyNS4xMTIuNjciLCJuYW1lIjoiV2FuIExpIFlhbmcgIChOQ1MpIiwib2lkIjoiM2E3NTY4NTUtODA5Mi00YTZkLWE0MmUtM2JhOTk2YWMyZTMyIiwib25wcmVtX3NpZCI6IlMtMS01LTIxLTEyODgwMzI5NS0zMjY5ODExNDMtMzU3NDU5Mjk2LTI4MDM5NCIsInBsYXRmIjoiMyIsInB1aWQiOiIxMDAzN0ZGRUE3MTg0MTUwIiwicmgiOiIwLkFWWUE5ZGlReW1PSmJrdThxWnJFYUx6SHFBTUFBQUFBQUFBQXdBQUFBQUFBQUFCV0FKZy4iLCJzY3AiOiJBcHBsaWNhdGlvbi5SZWFkLkFsbCBlbWFpbCBGaWxlcy5SZWFkV3JpdGUgRmlsZXMuUmVhZFdyaXRlLkFsbCBHcm91cE1lbWJlci5SZWFkV3JpdGUuQWxsIE1haWwuUmVhZCBNYWlsLlNlbmQgb3BlbmlkIHByb2ZpbGUgU2l0ZXMuUmVhZC5BbGwgU2l0ZXMuUmVhZFdyaXRlLkFsbCBVc2VyLlJlYWQgVXNlci5SZWFkLkFsbCIsInNpZ25pbl9zdGF0ZSI6WyJpbmtub3dubnR3ayJdLCJzdWIiOiJUeXFCSW9JSlQ0ckdTb0RWaUN6MWNDRnZZYU9tUVh2STlURk5NR1FKSlBRIiwidGVuYW50X3JlZ2lvbl9zY29wZSI6IkFTIiwidGlkIjoiY2E5MGQ4ZjUtODk2My00YjZlLWJjYTktOWFjNDY4YmNjN2E4IiwidW5pcXVlX25hbWUiOiJsaXlhbmcud2FuMkBuY3MuY28iLCJ1cG4iOiJsaXlhbmcud2FuMkBuY3MuY28iLCJ1dGkiOiJXWUJRMnZZR2FVMmhRTk40WU5Fb0FBIiwidmVyIjoiMS4wIiwid2lkcyI6WyI5Yjg5NWQ5Mi0yY2QzLTQ0YzctOWQwMi1hNmFjMmQ1ZWE1YzMiLCJiNzlmYmY0ZC0zZWY5LTQ2ODktODE0My03NmIxOTRlODU1MDkiXSwieG1zX3N0Ijp7InN1YiI6IlZXS2VoX3ZPWXNVR0RkRFFtZmM0ZFBZV0F5bmh3ZGJjUU5wczFtWE9BaHMifSwieG1zX3RjZHQiOjE0NzQ4NzQ0MjZ9.cCc8PvrXFZ0hEGlX9WHkw6aKp9NxVuT8wbert_Xtn7u6n9SJFSGwCukumIsY3QdRkr2opSS0mV3VbqQlW3aOsn2bPMfusgz-xidgi94q2_5xEWMJev8vec5Cs0hg-wEuxEwLQKeTbl3LY6GwGn62RnOV8jjEKk24lsPGrto82s30WYf-4ZfTluXjmmLxa9AZmng6J20MVdj0m0uvcM9NchmwghnBES87G2FK-UFDBjlpF4knX7yt3hlC-vMWskhipfZBlL5TBDl4OSPBsqiHvJYcNXodiYLZ_y1XR03Zl0fa119nftA0arnQ7vbNNTwEp134Vq6Hld5mcFpZyeL3LQ";

            string x5c = "MIIDBTCCAe2gAwIBAgIQGQ6YG6NleJxJGDRAwAd/ZTANBgkqhkiG9w0BAQsFADAtMSswKQYDVQQDEyJhY2NvdW50cy5hY2Nlc3Njb250cm9sLndpbmRvd3MubmV0MB4XDTIyMTAwMjE4MDY0OVoXDTI3MTAwMjE4MDY0OVowLTErMCkGA1UEAxMiYWNjb3VudHMuYWNjZXNzY29udHJvbC53aW5kb3dzLm5ldDCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBALSS+lq9iVLMS8jXsz0IdSes5+sEqAwIYEWEg5GjLhB8u+VYpIgfMINuVrkfeoHTKaKJHZUb4e0p0b7Y0DfW+ZuMyQjKUkXCeQ7l5eJnHewoN2adQufiZjKvCe5uzkvR6VEGwNcobQh6j+1wOFJ0CNvCfk5xogGt74jy5atOutwquoUMO42KOcjY3SXFefhUvsTVe1B0eMwDEa7jFB8bXtSGSc2yZsYyqBIycA07XHeg5CN8q5JmLfBnUJrtGAR0yUmYs/jNdAmNy27y83/rWwTSkP4H5xhihezL0QpjwP2BfwD8p6yBu6eLzw0V4aRt/wiLd9ezcrxqCMIr9ALfN5ECAwEAAaMhMB8wHQYDVR0OBBYEFJcSH+6Eaqucndn9DDu7Pym7OA8rMA0GCSqGSIb3DQEBCwUAA4IBAQADKkY0PIyslgWGmRDKpp/5PqzzM9+TNDhXzk6pw8aESWoLPJo90RgTJVf8uIj3YSic89m4ftZdmGFXwHcFC91aFe3PiDgCiteDkeH8KrrpZSve1pcM4SNjxwwmIKlJdrbcaJfWRsSoGFjzbFgOecISiVaJ9ZWpb89/+BeAz1Zpmu8DSyY22dG/K6ZDx5qNFg8pehdOUYY24oMamd4J2u2lUgkCKGBZMQgBZFwk+q7H86B/byGuTDEizLjGPTY/sMms1FAX55xBydxrADAer/pKrOF1v7Dq9C1Z9QVcm5D9G4DcenyWUdMyK43NXbVQLPxLOng51KO9icp2j4U7pwHP";

            Decode(jwt, x5c);

            x5c = "-----BEGIN CERTIFICATE-----\n" + x5c + "\n-----END CERTIFICATE-----";

            var handler = new JwtSecurityTokenHandler();
            var certificate = new X509Certificate2(Convert.FromBase64String(x5c));

            IdentityModelEventSource.ShowPII = true;
            TokenValidationParameters validationParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                ValidateLifetime = false,
                ValidateAudience = false,
                ValidateIssuer = false,
                IssuerSigningKeyResolver = (t, st, i, p) => new[] {
                    new X509SecurityKey(certificate)
                },
            };

            JwtSecurityToken outerJWT = handler.ReadJwtToken(jwt);
            SecurityToken validatedSecurityToken = null;
            var cp = handler.ValidateToken(jwt, validationParameters, out validatedSecurityToken);

            //Jwt jwt = new Jwt(new JwtParameter
            //{
            //    TokenSecret = x5c
            //});

            //bool result = jwt.ValidateToken(token);

        }

        public static string Decode(string token, string key, bool verify = true)
        {
            string[] parts = token.Split('.');
            string header = parts[0];
            string payload = parts[1];
            byte[] crypto = Base64UrlDecode(parts[2]);

            string headerJson = Encoding.UTF8.GetString(Base64UrlDecode(header));
            JObject headerData = JObject.Parse(headerJson);

            string payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(payload));
            JObject payloadData = JObject.Parse(payloadJson);

            if (verify)
            {
                var keyBytes = Convert.FromBase64String(key); // your key here

                AsymmetricKeyParameter asymmetricKeyParameter = PublicKeyFactory.CreateKey(keyBytes);
                RsaKeyParameters rsaKeyParameters = (RsaKeyParameters)asymmetricKeyParameter;
                RSAParameters rsaParameters = new RSAParameters();
                rsaParameters.Modulus = rsaKeyParameters.Modulus.ToByteArrayUnsigned();
                rsaParameters.Exponent = rsaKeyParameters.Exponent.ToByteArrayUnsigned();
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.ImportParameters(rsaParameters);

                SHA256 sha256 = SHA256.Create();
                byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(parts[0] + '.' + parts[1]));

                RSAPKCS1SignatureDeformatter rsaDeformatter = new RSAPKCS1SignatureDeformatter(rsa);
                rsaDeformatter.SetHashAlgorithm("SHA256");
                if (!rsaDeformatter.VerifySignature(hash, FromBase64Url(parts[2])))
                    throw new ApplicationException(string.Format("Invalid signature"));
            }

            return payloadData.ToString();
        }

        // from JWT spec
        private static byte[] Base64UrlDecode(string input)
        {
            var output = input;
            output = output.Replace('-', '+'); // 62nd char of encoding
            output = output.Replace('_', '/'); // 63rd char of encoding
            switch (output.Length % 4) // Pad with trailing '='s
            {
                case 0: break; // No pad chars in this case
                case 1: output += "==="; break; // Three pad chars
                case 2: output += "=="; break; // Two pad chars
                case 3: output += "="; break; // One pad char
                default: throw new System.Exception("Illegal base64url string!");
            }
            var converted = Convert.FromBase64String(output); // Standard base64 decoder
            return converted;
        }

        // https://stackoverflow.com/questions/34284810/decoding-base64urluint-encoded-value
        static byte[] FromBase64Url(string base64Url)
        {
            string padded = base64Url.Length % 4 == 0
                ? base64Url : base64Url + "====".Substring(base64Url.Length % 4);
            string base64 = padded.Replace("_", "/")
                .Replace("-", "+");
            return Convert.FromBase64String(base64);
        }

        //public void Validate3(string tokenStr)
        //{
        //    //string tokenStr = "eyJraWQiOiIxZTlnZGs3IiwiYWxnIjoiUlMyNTYifQ.ewogImlzcyI6ICJodHRwOi8vc2VydmVyLmV4YW1wbGUuY29tIiwKICJzdWIiOiAiMjQ4Mjg5NzYxMDAxIiwKICJhdWQiOiAiczZCaGRSa3F0MyIsCiAibm9uY2UiOiAibi0wUzZfV3pBMk1qIiwKICJleHAiOiAxMzExMjgxOTcwLAogImlhdCI6IDEzMTEyODA5NzAsCiAiY19oYXNoIjogIkxEa3RLZG9RYWszUGswY25YeENsdEEiCn0.XW6uhdrkBgcGx6zVIrCiROpWURs-4goO1sKA4m9jhJIImiGg5muPUcNegx6sSv43c5DSn37sxCRrDZZm4ZPBKKgtYASMcE20SDgvYJdJS0cyuFw7Ijp_7WnIjcrl6B5cmoM6ylCvsLMwkoQAxVublMwH10oAxjzD6NEFsu9nipkszWhsPePf_rM4eMpkmCbTzume-fzZIi5VjdWGGEmzTg32h3jiex-r5WTHbj-u5HL7u_KP3rmbdYNzlzd1xWRYTUs4E8nOTgzAUwvwXkIQhOh5TPcSMBYy6X3E7-_gr9Ue6n4ND7hTFhtjYs3cjNKIA08qm5cpVYFMFMG6PkhzLQ";

        //    RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
        //    rsa.ImportParameters(
        //      new RSAParameters()
        //      {
        //          Modulus = FromBase64Url("MIIDBTCCAe2gAwIBAgIQGQ6YG6NleJxJGDRAwAd/ZTANBgkqhkiG9w0BAQsFADAtMSswKQYDVQQDEyJhY2NvdW50cy5hY2Nlc3Njb250cm9sLndpbmRvd3MubmV0MB4XDTIyMTAwMjE4MDY0OVoXDTI3MTAwMjE4MDY0OVowLTErMCkGA1UEAxMiYWNjb3VudHMuYWNjZXNzY29udHJvbC53aW5kb3dzLm5ldDCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBALSS+lq9iVLMS8jXsz0IdSes5+sEqAwIYEWEg5GjLhB8u+VYpIgfMINuVrkfeoHTKaKJHZUb4e0p0b7Y0DfW+ZuMyQjKUkXCeQ7l5eJnHewoN2adQufiZjKvCe5uzkvR6VEGwNcobQh6j+1wOFJ0CNvCfk5xogGt74jy5atOutwquoUMO42KOcjY3SXFefhUvsTVe1B0eMwDEa7jFB8bXtSGSc2yZsYyqBIycA07XHeg5CN8q5JmLfBnUJrtGAR0yUmYs/jNdAmNy27y83/rWwTSkP4H5xhihezL0QpjwP2BfwD8p6yBu6eLzw0V4aRt/wiLd9ezcrxqCMIr9ALfN5ECAwEAAaMhMB8wHQYDVR0OBBYEFJcSH+6Eaqucndn9DDu7Pym7OA8rMA0GCSqGSIb3DQEBCwUAA4IBAQADKkY0PIyslgWGmRDKpp/5PqzzM9+TNDhXzk6pw8aESWoLPJo90RgTJVf8uIj3YSic89m4ftZdmGFXwHcFC91aFe3PiDgCiteDkeH8KrrpZSve1pcM4SNjxwwmIKlJdrbcaJfWRsSoGFjzbFgOecISiVaJ9ZWpb89/+BeAz1Zpmu8DSyY22dG/K6ZDx5qNFg8pehdOUYY24oMamd4J2u2lUgkCKGBZMQgBZFwk+q7H86B/byGuTDEizLjGPTY/sMms1FAX55xBydxrADAer/pKrOF1v7Dq9C1Z9QVcm5D9G4DcenyWUdMyK43NXbVQLPxLOng51KO9icp2j4U7pwHP"),
        //          Exponent = FromBase64Url("AQAB")
        //      });

        //    var validationParameters = new TokenValidationParameters
        //    {
        //        RequireExpirationTime = true,
        //        RequireSignedTokens = true,
        //        ValidateAudience = false,
        //        ValidateIssuer = false,
        //        ValidateLifetime = false,
        //        IssuerSigningKey = new RsaSecurityKey(rsa)
        //    };

        //    SecurityToken validatedSecurityToken = null;
        //    var handler = new JwtSecurityTokenHandler();
        //    handler.ValidateToken(tokenStr, validationParameters, out validatedSecurityToken);
        //    JwtSecurityToken validatedJwt = validatedSecurityToken as JwtSecurityToken;
        //}

        //public void Validate2(string tokenStr)
        //{
        //    //string tokenStr = "eyJraWQiOiIxZTlnZGs3IiwiYWxnIjoiUlMyNTYifQ.ewogImlzcyI6ICJodHRwOi8vc2VydmVyLmV4YW1wbGUuY29tIiwKICJzdWIiOiAiMjQ4Mjg5NzYxMDAxIiwKICJhdWQiOiAiczZCaGRSa3F0MyIsCiAibm9uY2UiOiAibi0wUzZfV3pBMk1qIiwKICJleHAiOiAxMzExMjgxOTcwLAogImlhdCI6IDEzMTEyODA5NzAsCiAiY19oYXNoIjogIkxEa3RLZG9RYWszUGswY25YeENsdEEiCn0.XW6uhdrkBgcGx6zVIrCiROpWURs-4goO1sKA4m9jhJIImiGg5muPUcNegx6sSv43c5DSn37sxCRrDZZm4ZPBKKgtYASMcE20SDgvYJdJS0cyuFw7Ijp_7WnIjcrl6B5cmoM6ylCvsLMwkoQAxVublMwH10oAxjzD6NEFsu9nipkszWhsPePf_rM4eMpkmCbTzume-fzZIi5VjdWGGEmzTg32h3jiex-r5WTHbj-u5HL7u_KP3rmbdYNzlzd1xWRYTUs4E8nOTgzAUwvwXkIQhOh5TPcSMBYy6X3E7-_gr9Ue6n4ND7hTFhtjYs3cjNKIA08qm5cpVYFMFMG6PkhzLQ";
        //    string[] tokenParts = tokenStr.Split('.');

        //    RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
        //    rsa.ImportParameters(
        //      new RSAParameters()
        //      {
        //          Modulus = FromBase64Url("MIIDBTCCAe2gAwIBAgIQGQ6YG6NleJxJGDRAwAd/ZTANBgkqhkiG9w0BAQsFADAtMSswKQYDVQQDEyJhY2NvdW50cy5hY2Nlc3Njb250cm9sLndpbmRvd3MubmV0MB4XDTIyMTAwMjE4MDY0OVoXDTI3MTAwMjE4MDY0OVowLTErMCkGA1UEAxMiYWNjb3VudHMuYWNjZXNzY29udHJvbC53aW5kb3dzLm5ldDCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBALSS+lq9iVLMS8jXsz0IdSes5+sEqAwIYEWEg5GjLhB8u+VYpIgfMINuVrkfeoHTKaKJHZUb4e0p0b7Y0DfW+ZuMyQjKUkXCeQ7l5eJnHewoN2adQufiZjKvCe5uzkvR6VEGwNcobQh6j+1wOFJ0CNvCfk5xogGt74jy5atOutwquoUMO42KOcjY3SXFefhUvsTVe1B0eMwDEa7jFB8bXtSGSc2yZsYyqBIycA07XHeg5CN8q5JmLfBnUJrtGAR0yUmYs/jNdAmNy27y83/rWwTSkP4H5xhihezL0QpjwP2BfwD8p6yBu6eLzw0V4aRt/wiLd9ezcrxqCMIr9ALfN5ECAwEAAaMhMB8wHQYDVR0OBBYEFJcSH+6Eaqucndn9DDu7Pym7OA8rMA0GCSqGSIb3DQEBCwUAA4IBAQADKkY0PIyslgWGmRDKpp/5PqzzM9+TNDhXzk6pw8aESWoLPJo90RgTJVf8uIj3YSic89m4ftZdmGFXwHcFC91aFe3PiDgCiteDkeH8KrrpZSve1pcM4SNjxwwmIKlJdrbcaJfWRsSoGFjzbFgOecISiVaJ9ZWpb89/+BeAz1Zpmu8DSyY22dG/K6ZDx5qNFg8pehdOUYY24oMamd4J2u2lUgkCKGBZMQgBZFwk+q7H86B/byGuTDEizLjGPTY/sMms1FAX55xBydxrADAer/pKrOF1v7Dq9C1Z9QVcm5D9G4DcenyWUdMyK43NXbVQLPxLOng51KO9icp2j4U7pwHP"),
        //          Exponent = FromBase64Url("AQAB")
        //      });

        //    SHA256 sha256 = SHA256.Create();
        //    byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(tokenParts[0] + '.' + tokenParts[1]));

        //    RSAPKCS1SignatureDeformatter rsaDeformatter = new RSAPKCS1SignatureDeformatter(rsa);
        //    rsaDeformatter.SetHashAlgorithm("SHA256");
        //    if (rsaDeformatter.VerifySignature(hash, FromBase64Url(tokenParts[2])))
        //    {
        //        Console.WriteLine("success, Signature is verified");
        //    }
        //    else
        //    {
        //        Console.WriteLine("failed, Signature is verified");
        //    }
        //}

        //static byte[] FromBase64Url(string base64Url)
        //{
        //    string padded = base64Url.Length % 4 == 0
        //        ? base64Url : base64Url + "====".Substring(base64Url.Length % 4);
        //    string base64 = padded.Replace("_", "/")
        //                          .Replace("-", "+");
        //    return Convert.FromBase64String(base64);
        //}
    }
}

