using System.Collections.Generic;

namespace TryIT.Jwt
{
    public class JwtParameter
	{
        public JwtParameter()
        {
            TokenLifetimeSecond = 60 * 60;
        }
		public string Issuer { get; set; }
		public string Audience { get; set; }
		public byte[] TokenSecret { get; set; }
        public List<CustomClaim> CustomClaims { get; set; }

        /// <summary>
        /// token validity period, default 1 hour
        /// </summary>
        public double TokenLifetimeSecond { get; set; }
    }

    public class CustomClaim
    {
        public CustomClaim(string key, object value)
        {
            Key = key;
            Value = value;
        }
        public string Key { get; set; }
        public object Value { get; set; }
    }
}

