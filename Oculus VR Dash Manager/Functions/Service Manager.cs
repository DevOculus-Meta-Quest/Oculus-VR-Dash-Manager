using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ServiceProcess;

namespace OVR_Dash_Manager.Functions
{
    /// <summary>
    /// Manages Windows services.
    /// </summary>
    public static class Service_Manager
    {
        // Dictionary to hold registered services.
        private static Dictionary<string, ServiceController> Services = new Dictionary<string, ServiceController>();

        /// <summary>
        /// Registers a service by its name.
        /// </summary>
        /// <param name="ServiceName">The name of the service.</param>
        public static void RegisterService(string ServiceName)
        {
            if (!Services.ContainsKey(ServiceName))
            {
                try
                {
                    var service = new ServiceController(ServiceName);
                    Services.Add(ServiceName, service);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Unable to load/find service {ServiceName} - {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Stops a registered service.
        /// </summary>
        /// <param name="ServiceName">The name of the service.</param>
        public static void StopService(string ServiceName)
        {
            if (Services.TryGetValue(ServiceName, out var service))
            {
                service.Refresh();

                if (Running(service.Status))
                {
                    try
                    {
                        service.Stop();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Unable to stop service {ServiceName} - {ex.Message}");
                    }
                }
            }
        }

        /// <summary>
        /// Starts a registered service.
        /// </summary>
        /// <param name="ServiceName">The name of the service.</param>
        public static void StartService(string ServiceName)
        {
            if (Services.TryGetValue(ServiceName, out var service))
            {
                service.Refresh();

                if (!Running(service.Status))
                {
                    try
                    {
                        service.Start();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Unable to start service {ServiceName} - {ex.Message}");
                    }
                }
            }
        }

        /// <summary>
        /// Sets a registered service to start automatically.
        /// </summary>
        /// <param name="ServiceName">The name of the service.</param>
        public static void Set_Automatic_Startup(string ServiceName)
        {
            if (Services.TryGetValue(ServiceName, out var service))
            {
                try
                {
                    service.Refresh();
                    ServiceHelper.ChangeStartMode(service, ServiceStartMode.Automatic);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Unable to set automatic startup service {ServiceName} - {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Sets a registered service to start manually.
        /// </summary>
        /// <param name="ServiceName">The name of the service.</param>
        public static void Set_Manual_Startup(string ServiceName)
        {
            if (Services.TryGetValue(ServiceName, out var service))
            {
                try
                {
                    service.Refresh();
                    ServiceHelper.ChangeStartMode(service, ServiceStartMode.Manual);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Unable to set manual startup service {ServiceName} - {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Checks if a service is running based on its status.
        /// </summary>
        /// <param name="Status">The status of the service.</param>
        /// <returns>True if the service is running; otherwise, false.</returns>
        private static bool Running(ServiceControllerStatus Status)
        {
            switch (Status)
            {
                case ServiceControllerStatus.Running:
                case ServiceControllerStatus.Paused:
                case ServiceControllerStatus.StartPending:
                case ServiceControllerStatus.StopPending:
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Gets the current state of a registered service.
        /// </summary>
        /// <param name="ServiceName">The name of the service.</param>
        /// <returns>The current state of the service.</returns>
        public static string GetState(string ServiceName)
        {
            if (Services.TryGetValue(ServiceName, out var service))
            {
                service.Refresh();
                return service.Status.ToString();
            }

            return "Not Found";
        }

        /// <summary>
        /// Gets the startup mode of a registered service.
        /// </summary>
        /// <param name="ServiceName">The name of the service.</param>
        /// <returns>The startup mode of the service.</returns>
        public static string GetStartup(string ServiceName)
        {
            if (Services.TryGetValue(ServiceName, out var service))
            {
                service.Refresh();
                return service.StartType.ToString();
            }

            return "Not Found";
        }
    }

    /// <summary>
    /// Helper class to change the startup mode of a service.
    /// </summary>
    public static class ServiceHelper
    {
        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool ChangeServiceConfig(
            IntPtr hService,
            uint nServiceType,
            uint nStartType,
            uint nErrorControl,
            string lpBinaryPathName,
            string lpLoadOrderGroup,
            IntPtr lpdwTagId,
            [In] char[] lpDependencies,
            string lpServiceStartName,
            string lpPassword,
            string lpDisplayName);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr OpenService(
            IntPtr hSCManager, string lpServiceName, uint dwDesiredAccess);

        [DllImport("advapi32.dll", EntryPoint = "OpenSCManagerW", ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr OpenSCManager(
            string machineName, string databaseName, uint dwAccess);

        [DllImport("advapi32.dll", EntryPoint = "CloseServiceHandle")]
        public static extern int CloseServiceHandle(IntPtr hSCObject);

        private const uint SERVICE_NO_CHANGE = 0xFFFFFFFF;
        private const uint SERVICE_QUERY_CONFIG = 0x00000001;
        private const uint SERVICE_CHANGE_CONFIG = 0x00000002;
        private const uint SC_MANAGER_ALL_ACCESS = 0x000F003F;

        /// <summary>
        /// Changes the start mode of a service.
        /// </summary>
        /// <param name="svc">The service controller.</param>
        /// <param name="mode">The start mode.</param>
        public static void ChangeStartMode(ServiceController svc, ServiceStartMode mode)
        {
            var scManagerHandle = OpenSCManager(null, null, SC_MANAGER_ALL_ACCESS);

            if (scManagerHandle == IntPtr.Zero)
            {
                throw new ExternalException("Open Service Manager Error");
            }

            var serviceHandle = OpenService(
                scManagerHandle,
                svc.ServiceName,
                SERVICE_QUERY_CONFIG | SERVICE_CHANGE_CONFIG);

            if (serviceHandle == IntPtr.Zero)
            {
                throw new ExternalException("Open Service Error");
            }

            var result = ChangeServiceConfig(
                serviceHandle,
                SERVICE_NO_CHANGE,
                (uint)mode,
                SERVICE_NO_CHANGE,
                null,
                null,
                IntPtr.Zero,
                null,
                null,
                null,
                null);

            if (result == false)
            {
                var nError = Marshal.GetLastWin32Error();
                var win32Exception = new Win32Exception(nError);

                throw new ExternalException("Could not change service start type: "
                    + win32Exception.Message);
            }

            CloseServiceHandle(serviceHandle);
            CloseServiceHandle(scManagerHandle);
        }
    }
}