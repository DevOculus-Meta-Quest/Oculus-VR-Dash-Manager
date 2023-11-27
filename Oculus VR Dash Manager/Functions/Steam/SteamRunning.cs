using Newtonsoft.Json;
using OVR_Dash_Manager.Functions.Dashes;
using OVR_Dash_Manager.Functions.Oculus;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Timers;

namespace OVR_Dash_Manager.Functions.Steam
{
    public static class SteamRunning
    {
        private static bool _IsSetup;

        public delegate void Steam_VR_Running_State_Changed();

        // Delegates and events for state changes
        private delegate void Steam_Running_State_Changed();

        public static event Steam_VR_Running_State_Changed Steam_VR_Running_State_Changed_Event;

        private static event Steam_Running_State_Changed Steam_Running_State_Changed_Event;

        public static bool ManagerCalledExit { get; set; }

        public static string Steam_Directory { get; private set; }

        // Properties
        public static bool Steam_Installed { get; private set; }

        public static bool Steam_Running { get; private set; }
        public static string Steam_VR_Directory { get; private set; }
        public static bool Steam_VR_Installed { get; private set; }
        public static bool Steam_VR_Monitor_Running { get; private set; }
        public static bool Steam_VR_Server_Running { get; private set; }

        // Method to check if Steam and SteamVR are installed
        public static void CheckInstalled()
        {
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var openVRPath = Path.Combine(localAppData, "openvr\\openvrpaths.vrpath");

            if (File.Exists(openVRPath))
            {
                var json = File.ReadAllText(openVRPath);

                if (json.Contains("config") || json.Contains("runtime"))
                {
                    try
                    {
                        var config = JsonConvert.DeserializeObject<OpenVR_Stripped>(json);

                        if (config != null)
                        {
                            Steam_Directory = config.config.FirstOrDefault();
                            Steam_VR_Directory = config.runtime.FirstOrDefault();

                            if (!string.IsNullOrEmpty(Steam_Directory))
                                Steam_Directory = StringManipulationUtilities.RemoveStringFromEnd(Steam_Directory, @"\\config");
                        }
                    }
                    catch (Exception)
                    {
                        // Handle exception
                    }
                }
            }

            if (!string.IsNullOrEmpty(Steam_Directory))
            {
                if (!Directory.Exists(Steam_Directory))
                    Steam_Directory = string.Empty;
                else
                    Steam_Installed = true;
            }

            if (!string.IsNullOrEmpty(Steam_VR_Directory))
            {
                if (!Directory.Exists(Steam_VR_Directory))
                    Steam_VR_Directory = string.Empty;
                else
                    Steam_VR_Installed = true;
            }
        }

        // Method to close SteamVR and reset link
        public static void Close_SteamVR_ResetLink()
        {
            Close_SteamVR_Server();
            Oculus_Link.StopLink();
            Close_SteamVR_Server();

            var inAMomentThread = new Thread(StartLinkInAMoment);
            inAMomentThread.Start();
        }

        // Method to close SteamVR server
        public static void Close_SteamVR_Server()
        {
            if (Steam_VR_Server_Running)
            {
                var vrServer = Process.GetProcessesByName("vrserver");

                if (vrServer.Length == 1)
                    vrServer[0].Kill();
            }

            CloseSteamVRMonitor();
        }

        // Setup method to initialize event handlers and check installed processes
        public static void Setup()
        {
            if (_IsSetup) return;

            _IsSetup = true;
            CheckInstalled();
            Steam_VR_Running_State_Changed_Event += Steam_Steam_VR_Running_State_Changed_Event;
            ProcessWatcher.ProcessStarted += Process_Watcher_ProcessStarted;
            ProcessWatcher.ProcessExited += Process_Watcher_ProcessExited;
            TimerManager.CreateTimer("SteamVR Focus Fix", TimeSpan.FromSeconds(1), Check_SteamVR_FocusProblem);

            var processNamesToCheck = new List<string> { "steam", "vrserver", "vrmonitor" };

            foreach (var processName in processNamesToCheck)
            {
                var processes = Process.GetProcessesByName(processName);

                if (processes.Length > 0)
                    Set_Running_State($"{processName}.exe", true);
            }
        }

        // Method to close SteamVR monitor
        private static void CloseSteamVRMonitor()
        {
            var vrmonitor = Process.GetProcessesByName("vrmonitor");

            if (vrmonitor.Length == 1)
            {
                if (vrmonitor[0].MainWindowHandle != IntPtr.Zero)
                {
                    try
                    {
                        vrmonitor[0].CloseMainWindow();
                    }
                    catch (Exception)
                    {
                        try
                        {
                            vrmonitor[0].Kill();
                        }
                        catch (Exception)
                        {
                            // Handle exception
                        }
                    }
                }
            }
        }

        private static void Process_Watcher_ProcessExited(string pProcessName, int pProcessID) => Set_Running_State(pProcessName, false);

        // Event handlers for process started and exited
        private static void Process_Watcher_ProcessStarted(string pProcessName, int pProcessID) => Set_Running_State(pProcessName, true);

        // Method to set the running state of Steam related processes
        private static void Set_Running_State(string processName, bool state)
        {
            switch (processName)
            {
                case "steam.exe":
                    Steam_Running = state;
                    Steam_Running_State_Changed_Event?.Invoke();
                    break;

                case "vrserver.exe":
                    Steam_VR_Server_Running = state;
                    Steam_VR_Running_State_Changed_Event?.Invoke();
                    break;

                case "vrmonitor.exe":
                    Steam_VR_Monitor_Running = state;
                    break;

                default:
                    break;
            }
        }

        // Method to start link after a delay
        private static void StartLinkInAMoment()
        {
            Thread.Sleep(2000);
            ManagerCalledExit = true;
            Close_SteamVR_Server();
            Oculus_Link.StartLink();
            Thread.Sleep(2000);
            ManagerCalledExit = true;
            Close_SteamVR_Server();
        }

        // Event handler for Steam VR running state change
        private static void Steam_Steam_VR_Running_State_Changed_Event()
        {
            if (!Steam_VR_Server_Running && !ManagerCalledExit && Properties.Settings.Default.ExitLinkOn_UserExit_SteamVR)
            {
                Close_SteamVR_ResetLink();
            }

            ManagerCalledExit = false;
        }

        #region SteamVR Focus Fix

        // Method to focus Steam VR Monitor Window
        public static void Focus_Steam_VR_Monitor_Window()
        {
            if (Steam_VR_Server_Running)
            {
                var vrmonitor = Process.GetProcessesByName("vrmonitor");

                if (vrmonitor.Length == 1)
                {
                    if (vrmonitor[0].MainWindowHandle != IntPtr.Zero)
                    {
                        WindowUtilities.BringWindowToTop(vrmonitor[0].MainWindowHandle);
                        WindowUtilities.SetForegroundWindow(vrmonitor[0].MainWindowHandle);
                        WindowUtilities.SetFocus(vrmonitor[0].MainWindowHandle);
                    }
                }
            }
        }

        // Method to check SteamVR focus problem
        private static void Check_SteamVR_FocusProblem(object sender, ElapsedEventArgs args)
        {
            if (Steam_VR_Server_Running)
            {
                if (Properties.Settings.Default.SteamVRFocusFix)
                {
                    switch (WindowUtilities.GetActiveWindowTitle())
                    {
                        case "Task View":
                            Dash_Manager.MainForm_FixTaskViewIssue();
                            Focus_Steam_VR_Monitor_Window();
                            break;

                        default:
                            break;
                    }
                }
            }
        }

        #endregion SteamVR Focus Fix

        // Class to represent stripped version of OpenVR
        internal class OpenVR_Stripped
        {
            public List<string> config { get; set; }
            public List<string> runtime { get; set; }
        }
    }

    // Class to manage Steam VR Settings
    public static class Steam_VR_Settings
    {
        // Enum to represent OpenXR Runtime
        public enum OpenXR_Runtime
        {
            Unknown = -1,
            Oculus = 0,
            SteamVR = 1
        }

        // Properties
        public static OpenXR_Runtime Current_Open_XR_Runtime { get; private set; }

        // Method to read runtime
        public static OpenXR_Runtime Read_Runtime()
        {
            // Use the correct RegistryKeyType enum from the OVR_Dash_Manager.Functions namespace
            var oculusRunTimePath = RegistryManager.GetKeyValueString(RegistryKeyType.LocalMachine, @"SOFTWARE\Khronos\OpenXR\1", "ActiveRuntime");

            if (oculusRunTimePath.Contains("oculus-runtime\\oculus_openxr_64.json"))
                Current_Open_XR_Runtime = OpenXR_Runtime.Oculus;
            else if (oculusRunTimePath.Contains("SteamVR\\steamxr_win64.json"))
                Current_Open_XR_Runtime = OpenXR_Runtime.SteamVR;
            else if (oculusRunTimePath.Contains("oculus-runtime\\oculus_openxr_32.json"))
                Current_Open_XR_Runtime = OpenXR_Runtime.Oculus;
            else if (oculusRunTimePath.Contains("SteamVR\\steamxr_win32.json"))
                Current_Open_XR_Runtime = OpenXR_Runtime.SteamVR;
            else
                Current_Open_XR_Runtime = OpenXR_Runtime.Unknown;

            return Current_Open_XR_Runtime;
        }

        // Method to disable USB PowerManagement
        public static void Set_Disable_USB_PowerManagement() => Run_RemoveUsbHelper_Action("disableenhancepowermanagement");

        // Method to set SteamVR runtime
        public static void Set_SteamVR_Runtime() => Run_RemoveUsbHelper_Action("setopenxrruntime");

        // Method to run remove USB helper action
        private static void Run_RemoveUsbHelper_Action(string action)
        {
            if (SteamRunning.Steam_VR_Installed)
            {
                if (Directory.Exists(SteamRunning.Steam_VR_Directory))
                {
                    var helper = Path.Combine(SteamRunning.Steam_VR_Directory, "bin\\win32\\removeusbhelper.exe");

                    if (File.Exists(helper))
                        Process.Start(helper, action);
                }
            }
        }
    }
}