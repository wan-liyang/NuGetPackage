using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TryIT.SqlAdo.MicrosoftSqlClient;
using TryIT.SqlAdo.MicrosoftSqlClient.Models;

namespace NUnitTest02
{
    public class DBConnector_UnitTest
    {
        [Test]
        public void Test1()
        {
            ConnectorConfig config = new ConnectorConfig
            {
                ConnectionString = "",
                TimeoutSecond = 10 * 60 // 10 minute
            };
            DbConnector dbConnector = new DbConnector(config);
            dbConnector.FetchDataTable("commandText", System.Data.CommandType.Text);


            CopyDataModel copyDataModel = new CopyDataModel
            {
                
            };
            dbConnector.CopyData(copyDataModel);
        }
    }
}
