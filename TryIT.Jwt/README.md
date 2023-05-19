## How to use this library

### access https://docs.microsoft.com/en-us/graph/auth-v2-user to understand more ###

```
using TryIT.Jwt;

Jwt jwt = new Jwt(new JwtParameter
{
    Issuer = "testaa",
    Audience = "testdd",
    TokenSecret = Encoding.UTF8.GetBytes("You_Need_To_Provide_A_Longer_Secret_Key_Here"),
    CustomClaims = new List<CustomClaim>
    {
        new("userid", 1.1),
        new("username", "test"),
        new("isman", true),
        new("is18", false)
    }
});

var token = jwt.GenerateToken();
var result = jwt.ValidateToken(token);

var claim1 = jwt.GetClaim(token, "userid");
var claim2 = jwt.GetClaim(token, "is19");

Console.WriteLine(token);
```