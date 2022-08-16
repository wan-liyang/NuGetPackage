using SqlAdoService;
using NUnit.Framework;
using System;

namespace NUnitTest.SqlAdoService
{
    class ServiceUtility_UnitTest
    {
        [Test]
        public void SqlAdoObject_Test()
        {
            var config = new AdoConfig
            {
                ConnectionString = "",
                TimeoutSecond = 30
            };

            Assert.Throws<NullReferenceException>(() => {
                new SqlAdoObject(config).ExecuteNonQuery("SELECT 1 FROM dbo.__dummyTestTable", System.Data.CommandType.Text);
            });

            config = new AdoConfig
            {
                ConnectionString = "test",
                TimeoutSecond = 30
            };

            Assert.Throws<System.Data.SqlClient.SqlException>(() => {
                new SqlAdoObject(config).ExecuteNonQuery("SELECT 1 FROM dbo.__dummyTestTable", System.Data.CommandType.Text);
            });
        }

        [Test]
        public void SqlAdoStatic_Test()
        {
            SqlAdoStatic.InitConfig(new AdoConfig
            {
                ConnectionString = "",
                TimeoutSecond = 30
            });

            Assert.Throws<NullReferenceException>(() => {
                SqlAdoStatic.FetchDataTable("SELECT 1 FROM dbo.__dummyTestTable", System.Data.CommandType.Text);
            });

            SqlAdoStatic.InitConfig(new AdoConfig
            {
                ConnectionString = "test",
                TimeoutSecond = 30
            });

            Assert.Throws<System.Data.SqlClient.SqlException>(() => {
                SqlAdoStatic.FetchDataTable("SELECT 1 FROM dbo.__dummyTestTable", System.Data.CommandType.Text);
            });
        }
    }
}
