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