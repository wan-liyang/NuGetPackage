## Tableau REST API in C# method

### Tableau REST API list https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref.htm

```
string hostUrl = "";
string apiVersion = "";
string tokenName = "";
string tokenSecret = "";
string sitename = "";
using(TableauConnector connector = new TableauConnector(hostUrl, apiVersion, tokenName, tokenSecret, sitename))
{
    connector.GetAllUsers();
}
```
