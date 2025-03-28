﻿using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace TryIT.Jwt
{
    public class Jwt
    {
        private JwtParameter _parameter;
        public Jwt(JwtParameter parameter)
		{
            if (parameter == null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }
            if (string.IsNullOrEmpty(parameter.Issuer))
            {
                throw new ArgumentNullException(nameof(parameter.Issuer));
            }
            if (string.IsNullOrEmpty(parameter.Audience) && (parameter.Audiences == null || parameter.Audiences.Count == 0))
            {
                throw new ArgumentNullException($"{nameof(parameter.Audience)} and {nameof(parameter.Audiences)} cannot both empty");
            }
            if (parameter.TokenSecret == null || parameter.TokenSecret.Length == 0)
            {
                throw new ArgumentNullException(nameof(parameter.TokenSecret));
            }

            _parameter = parameter;
		}

        /// <summary>
        /// generate token
        /// </summary>
        /// <returns></returns>
		public string GenerateToken()
		{
            var tokenHandler = new JwtSecurityTokenHandler();

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iss, _parameter.Issuer),
                new Claim(JwtRegisteredClaimNames.Aud, _parameter.Audience)
            };

            if (_parameter.CustomClaims != null && _parameter.CustomClaims.Count() > 0)
            {
                foreach (var claimPair in _parameter.CustomClaims)
                {
                    var valueType = ClaimValueTypes.String;

                    switch (Type.GetTypeCode(claimPair.Value.GetType()))
                    {
                        case TypeCode.Boolean:
                            valueType = ClaimValueTypes.Boolean;
                            break;
                        case TypeCode.Double:
                            valueType = ClaimValueTypes.Double;
                            break;
                        case TypeCode.Int32:
                            valueType = ClaimValueTypes.Integer32;
                            break;
                        default:
                            valueType = ClaimValueTypes.String;
                            break;
                    }

                    var claim = new Claim(claimPair.Key, claimPair.Value.ToString(), valueType);
                    claims.Add(claim);
                }
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.Add(TimeSpan.FromSeconds(_parameter.TokenLifetimeSecond)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_parameter.TokenSecret), SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// validate token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool ValidateToken(string token)
        {
            try
            {
                TokenValidationParameters validationParameters = new TokenValidationParameters
                {
                    ValidIssuer = _parameter.Issuer,
                    ValidAudience = _parameter.Audience,
                    ValidAudiences = _parameter.Audiences,
                    IssuerSigningKey = new SymmetricSecurityKey(_parameter.TokenSecret),

                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true
                };

                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

                SecurityToken jwt;
                var result = tokenHandler.ValidateToken(token, validationParameters, out jwt);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// get claim from token
        /// </summary>
        /// <param name="token"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetClaimValue(string token, string name)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.ReadJwtToken(token);

            if (securityToken != null)
            {
                if (securityToken.Claims != null && securityToken.Claims.Count() > 0)
                {
                    var claim = securityToken.Claims.Where(p => p.Type.Equals(name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                    if (claim != null)
                    {
                        return claim.Value;
                    }
                }
            }

            return null;
        }
    }
}

