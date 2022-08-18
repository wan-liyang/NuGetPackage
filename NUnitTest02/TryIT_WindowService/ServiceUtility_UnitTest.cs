using NUnit.Framework;
using System;
using TryIT.WindowService;

namespace NUnitTest02.TryIT_WindowService
{
    class ServiceUtility_UnitTest
    {
        [Test]
        public void GetStatus_Test()
        {
            bool windows = OperatingSystem.IsWindows();

            if (windows)
            {
                string status = ServiceUtility.GetStatus("MSSQLSERVER");
                Assert.AreEqual("Running", status);
            }
            else
            {
                Assert.Throws<System.PlatformNotSupportedException>(() =>
                {
                    string status = ServiceUtility.GetStatus("MSSQLSERVER");
                    Assert.AreEqual("Running", status);
                });
            }
        }
        [Test]
        public void NotExists_Test()
        {
            if (OperatingSystem.IsWindows())
            {
                Assert.Throws<Exception>(() =>
                {
                    string status = ServiceUtility.GetStatus("MSSQLSERVER_dummy");
                });
            }
            else
            {
                Assert.Throws<System.PlatformNotSupportedException>(() =>
                {
                    string status = ServiceUtility.GetStatus("MSSQLSERVER_dummy");
                });
            }
        }

        [Test]
        public void StopStart_Test()
        {
            if (OperatingSystem.IsWindows())
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
            else
            {
                Assert.Throws<System.PlatformNotSupportedException>(() =>
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
                });
            }

        }
    }
}
