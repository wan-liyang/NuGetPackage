using System;
using System.Collections.Generic;

namespace TryIT.JWT.Azure
{
	public class TokenModel
	{
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public int ext_expires_in { get; set; }
        public AccessToken token { get; set; }
    }

    public class AccessToken
    {
        public string access_token { get; set; }

        public int size { get; set; }
        public Header header { get; set; }
        public Payload payload { get; set; }
        public string signature { get; set; }
        public string expires { get; set; }

        public class Header
        {
            public string typ { get; set; }
            public string nonce { get; set; }
            public string alg { get; set; }
            public string x5t { get; set; }
            public string kid { get; set; }
        }

        public class Payload
        {
            public string aud { get; set; }
            public string iss { get; set; }
            public int iat { get; set; }
            public int nbf { get; set; }
            public int exp { get; set; }
            public string aio { get; set; }
            public string app_displayname { get; set; }
            public string appid { get; set; }
            public string appidacr { get; set; }
            public string idp { get; set; }
            public string idtyp { get; set; }
            public string oid { get; set; }
            public string rh { get; set; }
            public string sub { get; set; }
            public string tenant_region_scope { get; set; }
            public string tid { get; set; }
            public string uti { get; set; }
            public string ver { get; set; }
            public List<string> wids { get; set; }
            public int xms_tcdt { get; set; }
            public DateTime issue_at { get; set; }
            public DateTime not_before { get; set; }
            public DateTime expiry_at { get; set; }
        }
    }
}

