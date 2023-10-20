## How to use this library
1. add reference to ```TryIT.SqlAdo.MicrosoftSqlClient``` into your code
2. refer to example code below to perform db operation

```
using TryIT.SqlAdo.MicrosoftSqlClient;
using TryIT.SqlAdo.MicrosoftSqlClient.Models;


public void GetData()
{
    ConnectorConfig config = new ConnectorConfig
    {
        ConnectionString = "",
        TimeoutSecond = 10 * 60 // 10 minute
    };
    DbConnector dbConnector = new DbConnector(config);
    dbConnector.FetchDataTable("commandText", System.Data.CommandType.Text);
}
```

if the database has AlwaysEncrypted enabled, and you need query encrypted column, and the key store provider is Azure Key Vault, do below at Startup.cs or call this during program starting, PLEASE CALL ONCE ONLY

```
DbConnector.RegisterColumnEncryptionKeyStore_AKV(new AzureKeyVaultProvider
{
    TenantId = "",
    ClientId = "",
    ClientSecret = "",
    ProxyUrl = ""
});
```