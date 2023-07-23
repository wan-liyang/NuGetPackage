## How to use this library

### access https://docs.microsoft.com/en-us/graph/auth-v2-user to understand more ###

1. create a page to allow manual sign in account
2. page callback method, obtain access_token and save it in somewhere (e.g. Session / DB)
3. use that access_token to call Graph Service


### How to use TokenHelper to obtain access_token
1. add reference to ```TryIT.MicrosoftGraphApi``` into your code
1. create a page to allow Administrator initial token
2. in that page, use example code below to get token and save it

```
// page action call this method, and this method will redirect to AD login page
private MsGraphGetTokenConfig GetTokenConfig
{
    get
    {
        return new MsGraphGetTokenConfig
        { 
            OAuth_AuthorizeUrl = "",
            OAuth_TenantId = "",
            OAuth_ClientId = "",
            OAuth_RedirectUrl = "", // this is Callback url
            OAuth_Scope = "",
            OAuth_GetTokenUrl = "",
            OAuth_ClientSecret = "",

            OAuth_IsProxyRequired = "",
            Proxy_Url = "",
            Proxy_Username = "",
            Proxy_Password = ""
        };
    }
}

public void SignIn()
{
    TokenHelper tokenHelper = new TokenHelper(GetTokenConfig);

    // 1. validate token config
    if (tokenHelper.IsOAuthParameterValid)
    {
        // if token parameter is valid, then use browser redirect to the tokenHelper.AuthorizeUrl (Callback below)
        string authorizeURL = tokenHelper.AuthorizeUrl;

        // use code to redirect to authorizeURL
        this.RedirectUrl(authorizeURL);
    }
}

// this url is configured in Azure, after sign in, will redirect back to this url
public void Callback()
{
    string code = string.Empty;
    string state = string.Empty;

    if (!string.IsNullOrEmpty(code))
    {
        GetTokenResponse tokenResponse = tokenHelper.GetToken(code, state);
        // save token if need
    }
    else
    {
        // invalid response
    }
}

private void SaveToken(GetTokenResponse token)
{
    // save token somewhere
}

private string GetToken()
{
    /*
    * 1. get exists token (StateGuid, ExpiresOn, Access_token, Refresh_token)
    * 2. check token expiry, if ExpiresOn < Now, need refresh token to get new token
    * 3. save new token
    */

    var savedToken = "" // get saved token from session / db, this could be Object or DataTable

    if("savedToken is valid" == true) // check the token validity, e.g. check is not null
    {
        GetTokenResponse token = new GetTokenResponse
        {
            state = "[get state from exists token]",
            expires_on = "[get expires_on from exists token]",
            access_token = "[get expires_on from exists token]",
            refresh_token = "[get refresh_token from exists token]"
        };

        // here check token expiry, expires_on < DateTime.Now, if expired, will call RefreshToken to get latest valid token
        if(token.expires_on < DateTime.Now)
        {
            MsGraphGetTokenConfig tokenConfig = GetTokenConfig;
            TokenHelper tokenHelper = new TokenHelper(tokenConfig);
            GetTokenResponse tokenResponse = tokenHelper.RefreshToken(token.refresh_token, token.state);

            SaveToken(tokenResponse);

            return tokenResponse.access_token;
        }

        return token.access_token;
    }
    else
    {
        throw new Exception("Token not exists, please SignIn first.");
    }
}
```
### How to use this package to access GraphAPI

1. add reference to ```TryIT.MicrosoftGraphApi``` into your code
2. use following example code

```
MsGraphApiConfig graphConfig = new MsGraphApiConfig
{
    Proxy_Url = "[if the network restricted need proxy setup, indicator Proxy Url here]",
    Proxy_Username = "[if the network restricted need proxy setup, indicator Proxy Username here]",
    Proxy_Password = "[if the network restricted need proxy setup, indicator Proxy Password here]",
    Token = GetToken(); // "[this should be valid token, service will use this token to call Graph API]"
};

OutlookApi MsOutlook = new OutlookApi(graphConfig);
OutlookApi.GetMessages();
```


### Note

```SharepointApi``` Upload file method will replace `#` to `_` in file name