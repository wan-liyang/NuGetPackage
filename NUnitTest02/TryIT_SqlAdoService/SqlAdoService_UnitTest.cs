using TryIT.SqlAdoService;
using NUnit.Framework;
using System;

namespace NUnitTest02.TryIT_SqlAdoService
{
    class SqlAdoService_UnitTest
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

            Assert.Throws<System.ArgumentException>(() => {
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

            Assert.Throws<System.ArgumentException>(() => {
                SqlAdoStatic.FetchDataTable("SELECT 1 FROM dbo.__dummyTestTable", System.Data.CommandType.Text);
            });
        }
    }
}
