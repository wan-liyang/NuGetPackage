using NUnit.Framework;
using System;
using TryIT.WindowService;

namespace NUnitTest.TryIT_WindowService
{
    class ServiceUtility_UnitTest
    {
        [Test]
        public void GetStatus_Test()
        {
            string status = ServiceUtility.GetStatus("MSSQLSERVER");
            Assert.AreEqual("Running", status);
        }
        [Test]
        public void NotExists_Test()
        {
            Assert.Throws<Exception>(() =>
            {
                string status = ServiceUtility.GetStatus("MSSQLSERVER_dummy");
            });
        }

        [Test]
        public void StopStart_Test()
        {
            string serviceName = "MSSQLSERVER";
            string status = ServiceUtility.GetStatus(serviceName);
            Assert.AreEqual("Running", status);

            ServiceUtility.StopService(serviceName);
            status = ServiceUtility.GetStatus(serviceName);
            Assert.AreEqual("Stopped", status);

            ServiceUtility.StartService(serviceName);
            status = ServiceUtility.GetStatus(serviceName);
            Assert.AreEqual("Running", status);

            ServiceUtility.RestartService(serviceName);
            status = ServiceUtility.GetStatus(serviceName);
            Assert.AreEqual("Running", status);
        }
    }
}
