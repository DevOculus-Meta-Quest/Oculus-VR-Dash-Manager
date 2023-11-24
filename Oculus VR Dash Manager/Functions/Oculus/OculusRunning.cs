using OVR_Dash_Manager.Functions.Dashes;
using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Windows;

namespace OVR_Dash_Manager.Functions.Oculus
{
    public static class OculusRunning
    {
        // Properties
        public static string Oculus_Main_Directory { get; private set; }

        public static string Oculus_Dash_Directory { get; private set; }
        public static string Oculus_Dash_File { get; private set; }
        public static string Oculus_Client_EXE { get; private set; }
        public static string Oculus_DebugTool_EXE { get; private set; }
        public static bool Oculus_Is_Installed { get; private set; }
        public static bool Normal_Dash { get; private set; }
        public static bool Custom_Dash { get; private set; }
        public static string Current_Dash_Name { get; private set; }

        private static bool _ClientJustExited;
        private static bool _Report_ClientJustExited;
        private static bool _IsSetup;

        public static void Setup()
        {
            if (!_IsSetup)
            {
                _IsSetup = true;
                ProcessWatcher.ProcessStarted += ProcessWatcher_ProcessStarted;
                ProcessWatcher.ProcessExited += ProcessWatcher_ProcessExited;
            }
        }

        private static void ProcessWatcher_ProcessStarted(string processName, int processId)
        {
            Debug.WriteLine($"Started: {processName} - {DateTime.Now}");
            // Add any specific actions for processes started here
        }

        private static void ProcessWatcher_ProcessExited(string processName, int processId)
        {
            Debug.WriteLine($"Stopped: {processName} - {DateTime.Now}");

            if (processName == "OculusClient.exe" && _Report_ClientJustExited)
            {
                Debug.WriteLine("Set Client Minimize Exit Trigger");
                _ClientJustExited = true;
                _Report_ClientJustExited = false;
            }
        }

        public static void Check_Oculus_Is_Installed()
        {
            var OculusPath = Environment.GetEnvironmentVariable("OculusBase");

            if (Directory.Exists(OculusPath))
            {
                Oculus_Main_Directory = OculusPath;
                Oculus_Dash_Directory = Path.Combine(OculusPath, @"Support\oculus-dash\dash\bin");
                Oculus_Client_EXE = Path.Combine(OculusPath, @"Support\oculus-client\OculusClient.exe");
                Oculus_DebugTool_EXE = Path.Combine(OculusPath, @"Support\oculus-diagnostics\OculusDebugTool.exe");
                Oculus_Dash_File = Path.Combine(Oculus_Dash_Directory, @"OculusDash.exe");

                Oculus_Is_Installed = File.Exists(Oculus_Client_EXE);
            }
        }

        public static void Check_Current_Dash()
        {
            if (Oculus_Is_Installed && File.Exists(Oculus_Dash_File))
            {
                WhichDash(Oculus_Dash_File);
            }
        }

        private static void WhichDash(string FilePath)
        {
            Normal_Dash = false;
            Custom_Dash = false;
            Current_Dash_Name = "Checking";

            var Info = FileVersionInfo.GetVersionInfo(FilePath);
            var Current = Dash_Manager.CheckWhosDash(Info.ProductName);
            Current_Dash_Name = Dash_Manager.GetDashName(Current);

            if (Current_Dash_Name != Dash_Manager.GetDashName(Dashes.Dash_Type.Normal))
            {
                Custom_Dash = true;
            }
            else if (!Check_Is_OfficialDash(FilePath))
            {
                Custom_Dash = true;
                Current_Dash_Name = "Unknown";
            }
            else
            {
                Normal_Dash = true;
            }
        }

        private static bool Check_Is_OfficialDash(string FilePath)
        {
            var cert = X509Certificate.CreateFromSignedFile(FilePath);
            return cert.Issuer == "CN=DigiCert SHA2 Assured ID Code Signing CA, OU=www.digicert.com, O=DigiCert Inc, C=US";
        }

        public static void StartOculusClient(MainWindow mainForm)
        {
            if (Debugger.IsAttached && !Dashes.UtilityFunctions.EmulateReleaseMode(mainForm))
            {
                return;
            }

            if (File.Exists(Oculus_Client_EXE) && Process.GetProcessesByName("OculusClient").Length == 0)
            {
                if (Service_Manager.GetState("OVRService") != "Running" && File.Exists(Path.Combine(Oculus_Main_Directory, "Support\\oculus-runtime\\OVRServiceLauncher.exe")))
                {
                    var serviceLauncher = Process.Start(Path.Combine(Oculus_Main_Directory, "Support\\oculus-runtime\\OVRServiceLauncher.exe"), "-start");
                    serviceLauncher.WaitForExit();

                    for (int i = 0; i < 100; i++)
                    {
                        Thread.Sleep(1000);

                        if (Process.GetProcessesByName("OVRRedir").Length > 0)
                        {
                            Debug.WriteLine("OVRRedir Started");
                            Thread.Sleep(2000);
                            break;
                        }
                    }
                }

                var clientInfo = new ProcessStartInfo
                {
                    WorkingDirectory = Path.GetDirectoryName(Oculus_Client_EXE),
                    FileName = Oculus_Client_EXE
                };

                var client = Process.Start(clientInfo);

                if (Properties.Settings.Default.Minimize_Oculus_Client_OnClientStart)
                {
                    _Report_ClientJustExited = true;

                    for (int i = 0; i < 50; i++)
                    {
                        Thread.Sleep(250);
                        if (_ClientJustExited) break;
                    }

                    _Report_ClientJustExited = false;

                    var location = new Rect();

                    for (int i = 0; i < 20; i++)
                    {
                        WindowUtilities.MinimizeExternalWindow(client.MainWindowHandle);
                        Thread.Sleep(250);
                        WindowUtilities.GetWindowRect(client.MainWindowHandle, ref location);
                        if (double.IsNaN(location.Left)) break;
                    }

                    Debug.WriteLine("Client Window Minimized");
                }
            }
        }

        public static string GetTheVoidUniformsPath()
        {
            var OculusPath = Environment.GetEnvironmentVariable("OculusBase");
            return Path.Combine(OculusPath, @"Support\oculus-dash\dash\data\shaders\theVoid\theVoidUniforms.glsl");
        }

        public static string GetGridPlanePath()
        {
            var OculusPath = Environment.GetEnvironmentVariable("OculusBase");
            return Path.Combine(OculusPath, @"Support\oculus-dash\dash\assets\raw\textures\environment\the_void\grid_plane_006.dds");
        }
    }
}