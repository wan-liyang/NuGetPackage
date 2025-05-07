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
    	}
}

