## Tableau REST API in C# method

### Tableau REST API list https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref.htm

```
var requestModel = new TryIT.TableauApi.SiteModel.ApiRequestModel
{
    HostUrl = "",
    ApiVersion = "",
    Sitename = "",
    TokenName = "",
    TokenSecret = "",
    Proxy = new TryIT.TableauApi.SiteModel.WebProxyModel
    {
        Url = ""
    }
};
using(TableauConnector connector = new TableauConnector(requestModel))
{
    connector.GetAllUsers();
}
```
