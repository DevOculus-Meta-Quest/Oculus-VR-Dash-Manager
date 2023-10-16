using AdvancedSharpAdbClient;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using YOVR_Dash_Manager.Functions;

namespace OVR_Dash_Manager.Software
{
    public static class ADB
    {
        private static Process ADBServer;

        public static void Start()
        {
            /// ADB Auto Start Created By https://github.com/quagsirus
            ///

            if (Properties.Settings.Default.QuestPolling)
            {
                // Start an adb server if we don't already have one
                if (!AdbServer.Instance.GetStatus().IsRunning)
                {
                    var server = new AdbServer();
                    try
                    {
                        Functions.ProcessWatcher.ProcessStarted += Process_Watcher_ProcessStarted;

                        var result = server.StartServer(@".\ADB\adb.exe", false);
                        if (result != StartServerResult.Started)
                        {
                            Debug.WriteLine("Can't start adb server");
                        }

                        Thread RemoveWatcherThread = new Thread(RemoveWatcher);
                        RemoveWatcherThread.Start();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }
                else
                {
                    Process[] ADBs = Process.GetProcessesByName("adb");
                    if (ADBs.Length > 0)
                    {
                        String MyADBLocation = Path.Combine(Environment.CurrentDirectory, "ADB", "adb.exe");
                        foreach (Process item in ADBs)
                        {
                            if (item.MainModule.FileName == MyADBLocation)
                                Process_Watcher_ProcessStarted(item.ProcessName, item.Id);
                        }
                    }
                }
            }

            ///
        }

        private static void RemoveWatcher()
        {
            Thread.Sleep(1000);
            Functions.ProcessWatcher.ProcessStarted -= Process_Watcher_ProcessStarted;
        }

        private static void Process_Watcher_ProcessStarted(string pProcessName, int pProcessID)
        {
            if (pProcessName == "adb")
                ADBServer = Process.GetProcessById(pProcessID);
        }

        public static void Stop()
        {
            if (ADBServer != null)
            {
                try
                {
                    ADBServer.Kill();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }

        public static void InstallAPK(string apkPath)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = @".\ADB\adb.exe",
                Arguments = $"install \"{apkPath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true, // Redirect standard error to capture error messages
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process process = new Process { StartInfo = startInfo };
            process.Start();

            string errorOutput = process.StandardError.ReadToEnd(); // Read the error output

            process.WaitForExit();

            if (process.ExitCode != 0 || !string.IsNullOrEmpty(errorOutput)) // Check for errors
            {
                // Create a new Exception with the error message and log it using your ErrorLogger class
                Exception adbException = new Exception($"Error installing APK. Exit Code: {process.ExitCode}. Error Message: {errorOutput}");
                ErrorLogger.LogError(adbException);
            }
        }
    }
}