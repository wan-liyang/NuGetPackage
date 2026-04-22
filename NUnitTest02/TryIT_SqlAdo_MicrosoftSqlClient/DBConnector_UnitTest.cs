using System.ComponentModel.DataAnnotations;
using System.Data;
using TryIT.ObjectExtension;
using TryIT.SqlAdo.MicrosoftSqlClient;
using TryIT.SqlAdo.MicrosoftSqlClient.Attributes;
using TryIT.SqlAdo.MicrosoftSqlClient.CopyMode;
using TryIT.SqlAdo.MicrosoftSqlClient.Helper;
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
                DbLogDelegate = LogDelegate,
                ConnectionString = "Server={serverName};Database={dbName};Trusted_Connection=True;TrustServerCertificate=True",
                TimeoutSecond = 10 * 60 // 10 minute
            };
            dbConnector = new DbConnector(config);
        }

        private async Task LogDelegate(DbLogContext context)
        {
            // Here you can implement your logging logic, for example:
            Console.WriteLine($"[{context.Stage}] {context.Provider} - {context.Database} - {context.CommandText}");
        }

        [Test]
        public void CopyData()
        {
            ConnectorConfig config = new ConnectorConfig
            {
                ConnectionString = "",
                TimeoutSecond = 10 * 60 // 10 minute
            };
            dbConnector = new DbConnector(config);

            string sql = "";

            var table = dbConnector.FetchDataTable(sql, CommandType.Text);

            string json = "";

            CopyMode_InsertUpdate mode = json.JsonToObject<CopyMode_InsertUpdate>();
            mode.SourceData = table;

            config = new ConnectorConfig
            {
                ConnectionString = "",
                TimeoutSecond = 10 * 60 // 10 minute
            };

            dbConnector = new DbConnector(config);
            dbConnector.CopyData(mode);
        }

        [Test]
        public async Task Test_ExecuteScalarAsync()
        {
            var correlationContext = new DbCorrelationContext
            {
                CorrelationId = Guid.NewGuid().ToString(),
                CorrelationExtra = new Dictionary<string, string>
                {
                    { "TestKey", "TestValue" }
                }
            };

            using (new CorrelationScope(correlationContext))
            {
                string sql = "SELECT GETDATE()";
                var result = await dbConnector.ExecuteScalarAsync<DateTime>(sql, CommandType.Text, null);
                Assert.IsTrue(result > DateTime.MinValue, "Result should be a valid DateTime");
            }

            var correlationContext2 = new DbCorrelationContext
            {
                CorrelationId = Guid.NewGuid().ToString(),
                CorrelationExtra = new Dictionary<string, string>
                {
                    { "TestKey2", "TestValue2" }
                }
            };

            using (new CorrelationScope(correlationContext2))
            {
                string sql = "SELECT GETDATE()";
                var result = await dbConnector.ExecuteScalarAsync<DateTime>(sql, CommandType.Text, null);
                Assert.IsTrue(result > DateTime.MinValue, "Result should be a valid DateTime");
            }
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
            dataRow["Text_S"] = "Text 111";
            dataTable.Rows.Add(dataRow);

            DataRow dataRow2 = dataTable.NewRow();
            dataRow2["Id"] = 2;
            dataRow2["Text_S"] = "Text 222111";
            dataTable.Rows.Add(dataRow2);

            ICopyMode copyDataModel = new CopyMode_TruncateInsert
            {
                SourceData = dataTable,
                TargetTable = "dbo.Test_20231020",
                ColumnMappings = new Dictionary<string, string>
                {
                    {"id", "id" },
                    {"text_s", "text" }
                }
            };
            dbConnector.CopyData(copyDataModel);
        }

        [Test]
        public async Task GereateInsertSql_Test()
        {
            // dbo.Test_20231020

            var obj = new TestGenrateScript
            {
                Text = "Test Value"
            };

            var sqlGenerator = SqlGenerator.GenerateInsertSql("dbo.Test_20260422", obj);

            var result = await dbConnector.ExecuteScalarAsync<int>(sqlGenerator.Sql, CommandType.Text, sqlGenerator.Parameters.ToArray());

            Assert.That(result, Is.EqualTo(1));
        }

        [Test]
        public async Task GenerateUpdateSql_Test()
        {
            // Assume a record with Id = 1 already exists in dbo.Test_20260422
            var obj = new TestGenrateScript
            {
                Id = 1,
                Text = "Updated Value"
            };

            var sqlGenerator = SqlGenerator.GenerateUpdateSql("dbo.Test_20260422", obj);

            // If your update returns OUTPUT INSERTED.Id, use ExecuteScalarAsync
            var result = await dbConnector.ExecuteScalarAsync<int>(sqlGenerator.Sql, CommandType.Text, sqlGenerator.Parameters.ToArray());

            Assert.That(result, Is.EqualTo(1));
        }

        [Test]
        public async Task GenerateInsertUpdateSql_Test()
        {
            // If Id = 2 does not exist, this will insert; if it exists, it will update
            var obj = new TestGenrateScript
            {
                Id = 2,
                Text = "InsertOrUpdate Value"
            };

            var sqlGenerator = SqlGenerator.GenerateInsertUpdateSql("dbo.Test_20260422", obj);

            // If your insert/update returns OUTPUT INSERTED.Id, use ExecuteScalarAsync
            var result = await dbConnector.ExecuteScalarAsync<int>(sqlGenerator.Sql, CommandType.Text, sqlGenerator.Parameters.ToArray());

            Assert.That(result, Is.EqualTo(2));
        }

        public class TestGenrateScript
        {
            [Key]
            [SqlOutput]
            [SqlIdentity]
            public int Id { get; set; }
            public string Text { get; set; }
        }
    }
}
