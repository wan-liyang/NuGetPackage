using System;
using System.Linq;
using System.ServiceProcess;

namespace TryIT.WindowService
{
    /// <summary>
    /// The Utility for Window Service action
    /// </summary>
    public class ServiceUtility
    {
        /// <summary>
        /// get specific service, return null if service not found
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        private static ServiceController GetService(string serviceName)
        {
            ServiceController service = ServiceController.GetServices().FirstOrDefault(p => p.ServiceName.Equals(serviceName, StringComparison.OrdinalIgnoreCase));
            if (service != null)
            {
                //service = new ServiceController(serviceName, Environment.MachineName);
                ////this will grant permission to access the Service
                //ServiceControllerPermission scp = new ServiceControllerPermission(ServiceControllerPermissionAccess.Control, Environment.MachineName, serviceName);
                //scp.Assert();
            }
            return service;
        }

        #region Service Exists
        /// <summary>
        /// check whether specific service exists
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public static bool IsServiceExists(string serviceName)
        {
            bool isExists = false;
            using (ServiceController service = GetService(serviceName))
            {
                if (service != null)
                {
                    isExists = true;
                }
            }
            return isExists;
        }

        /// <summary>
        /// throw exception if service not exists
        /// </summary>
        /// <param name="serviceName"></param>
        public static void ThrowExceptionIfNotExists(string serviceName)
        {
            bool isExists = IsServiceExists(serviceName);
            if (!isExists)
                throw new Exception($"Service '{serviceName}' is not exists.");
        }
        #endregion

        #region Service Running
        /// <summary>
        /// check whether specific service is exists and running
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public static bool IsServiceRunning(string serviceName)
        {
            bool isRunning = false;
            bool isExists = IsServiceExists(serviceName);
            if (isExists)
            {
                try
                {
                    using (ServiceController service = GetService(serviceName))
                    {
                        if (service.Status.Equals(ServiceControllerStatus.Running)
                            || service.Status.Equals(ServiceControllerStatus.StartPending))
                        {
                            isRunning = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return isRunning;
        }

        /// <summary>
        /// throw exception if service not exists or not running
        /// </summary>
        /// <param name="serviceName"></param>
        public static void ThrowExceptionIfNotRunning(string serviceName)
        {
            ThrowExceptionIfNotExists(serviceName);

            if (!IsServiceRunning(serviceName))
                throw new Exception($"Service '{serviceName}' is not running.");
        } 
        #endregion

        #region Start / Stop / Restart / Execute
        /// <summary>
        /// start specific window service if it's not running
        /// </summary>
        /// <param name="serviceName"></param>
        public static void StartService(string serviceName)
        {
            ThrowExceptionIfNotExists(serviceName);

            try
            {
                using (ServiceController service = GetService(serviceName))
                {
                    if (!service.Status.Equals(ServiceControllerStatus.Running)
                        && !service.Status.Equals(ServiceControllerStatus.StartPending))
                    {
                        service.Start();
                        service.WaitForStatus(ServiceControllerStatus.Running);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// stop specific window service if it's running
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public static void StopService(string serviceName)
        {
            ThrowExceptionIfNotExists(serviceName);

            try
            {
                using (ServiceController service = GetService(serviceName))
                {
                    if (service.Status.Equals(ServiceControllerStatus.Running)
                        || service.Status.Equals(ServiceControllerStatus.StartPending))
                    {
                        service.Stop();
                        service.WaitForStatus(ServiceControllerStatus.Stopped);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// restart specific window service
        /// <para>if current is running, then Stop &amp; Start, otherwise Start</para>
        /// </summary>
        /// <param name="serviceName"></param>
        public static void RestartService(string serviceName)
        {
            ThrowExceptionIfNotExists(serviceName);

            try
            {
                using (ServiceController service = GetService(serviceName))
                {
                    if (service.Status.Equals(ServiceControllerStatus.Running)
                        || service.Status.Equals(ServiceControllerStatus.StartPending))
                    {
                        service.Stop();
                        service.WaitForStatus(ServiceControllerStatus.Stopped);
                    }
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// execute command on window service synchrony, the service running on same machine
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="command">custom command value to service, value must between 128 ~ 255</param>
        /// <returns></returns>
        public static void ExecuteCommand(string serviceName, int command)
        {
            ThrowExceptionIfNotRunning(serviceName);

            try
            {
                using (ServiceController service = GetService(serviceName))
                {
                    service.Refresh();
                    service.ExecuteCommand(command);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        /// <summary>
        /// Gets the status of the service that is referenced by this instance.
        /// One of the <see cref="System.ServiceProcess.ServiceControllerStatus"/> values that indicates
        /// whether the service is running, stopped, or paused, or whether a start, stop, pause, or continue command is pending.
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public static string GetStatus(string serviceName)
        {
            ThrowExceptionIfNotExists(serviceName);

            try
            {
                using (ServiceController service = GetService(serviceName))
                {
                    return service.Status.ToString();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
