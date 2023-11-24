using OVR_Dash_Manager.Functions.Dashes;
using OVR_Dash_Manager.Functions.Steam;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ServiceProcess;

namespace OVR_Dash_Manager.Functions
{
    public static class Service_Manager
    {
        private static Dictionary<string, ServiceController> Services = new Dictionary<string, ServiceController>();

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
                        service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30));
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Unable to stop service {ServiceName} - {ex.Message}");
                    }
                }
            }
        }

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
                        service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30));
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Unable to start service {ServiceName} - {ex.Message}");
                    }
                }
            }
        }

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

        public static string GetState(string ServiceName)
        {
            if (Services.TryGetValue(ServiceName, out var service))
            {
                service.Refresh();
                return service.Status.ToString();
            }

            return "Not Found";
        }

        public static string GetStartup(string ServiceName)
        {
            if (Services.TryGetValue(ServiceName, out var service))
            {
                service.Refresh();
                return service.StartType.ToString();
            }

            return "Not Found";
        }

        public static bool ManageService(string serviceName, bool startService)
        {
            try
            {
                if (startService)
                {
                    StartService(serviceName);
                }
                else
                {
                    StopService(serviceName);
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error managing the service: {ex.Message}");
                return false;
            }
        }
    }

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

        public static bool Activate(Dash_Type Dash)
        {
            Debug.WriteLine("Starting Activation: " + Dash.ToString());

            var Activated = false;
            var OVRServiceRunning = (Service_Manager.GetState("OVRService") == "Running");
            var OVRService_WasRunning = false;

            if (OVRServiceRunning)
            {
                OVRService_WasRunning = true;
                Debug.WriteLine("Stopping OVRService");
                SteamRunning.ManagerCalledExit = true;
                Service_Manager.StopService("OVRService");
            }

            Debug.WriteLine("Checking OVRService");
            OVRServiceRunning = (Service_Manager.GetState("OVRService") == "Running");

            if (!OVRServiceRunning)
            {
                Debug.WriteLine("Activating Dash");
                Activated = Dash_Manager.SetActiveDash(Dash);
            }
            else
            {
                Debug.WriteLine("!!!!!! OVRService Can Not Be Stopped");
            }

            if (OVRService_WasRunning)
            {
                Debug.WriteLine("Restarting OVRService");
                Service_Manager.StartService("OVRService");
            }

            return Activated;
        }

        public static void StopOculusServices(MainWindow mainForm)
        {
            if (Debugger.IsAttached && !Dashes.UtilityFunctions.EmulateReleaseMode(mainForm))
            {
                return;
            }
            if (Properties.Settings.Default.CloseOculusClientOnExit)
            {
                foreach (var client in Process.GetProcessesByName("OculusClient"))
                    client.CloseMainWindow();
            }

            if (Properties.Settings.Default.CloseOculusServicesOnExit)
            {
                if (Service_Manager.GetStartup("OVRLibraryService") == "Manual")
                {
                    Service_Manager.StopService("OVRLibraryService");
                }

                if (Service_Manager.GetStartup("OVRService") == "Manual")
                {
                    Service_Manager.StopService("OVRService");
                }
            }
        }
    }
}