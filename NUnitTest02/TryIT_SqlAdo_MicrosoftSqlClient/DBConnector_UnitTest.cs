using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TryIT.SqlAdo.MicrosoftSqlClient;
using TryIT.SqlAdo.MicrosoftSqlClient.CopyMode;
using TryIT.SqlAdo.MicrosoftSqlClient.Models;

namespace NUnitTest02.TryIT_SqlAdo_MicrosoftSqlClient
{
    public class DBConnector_UnitTest
    {
        public DbConnector dbConnector;

        [SetUp]
        public void Setup()
        {
            //DbConnector.RegisterColumnEncryptionKeyStore_AKV(new AzureKeyVaultProvider
            //{
            //    TenantId = "",
            //    ClientId = "",
            //    ClientSecret = "",
            //    ProxyUrl = ""
            //});

            ConnectorConfig config = new ConnectorConfig
            {
                ConnectionString = "Server=myserver;Database=mydb;Trusted_Connection=True;TrustServerCertificate=True",
                TimeoutSecond = 10 * 60 // 10 minute
            };
            dbConnector = new DbConnector(config);
        }

        [Test]
        public void Test1()
        {
            dbConnector.FetchDataTable("commandText", System.Data.CommandType.Text);
        }
        [Test]
        public void Copy_Test()
        {
            DataTable dataTable= new DataTable();
            dataTable.Columns.Add("Id");
            dataTable.Columns.Add("Text_S");

            DataRow dataRow = dataTable.NewRow();
            dataRow["Id"] = 1;
            dataRow["Text_S"] = "Text 1";
            dataTable.Rows.Add(dataRow);

            DataRow dataRow2 = dataTable.NewRow();
            dataRow2["Id"] = 2;
            dataRow2["Text_S"] = "Text 2";
            dataTable.Rows.Add(dataRow2);

            ICopyMode copyDataModel = new CopyMode_Insert
            {
                SourceData = dataTable,
                TargetTable = "dbo.Test_20231020",
                ColumnMappings = new Dictionary<string, string>
                {
                    {"Id", "Id" },
                    {"Text_S", "Text" }
                }
            };
            dbConnector.CopyData(copyDataModel);
        }
    }
}
