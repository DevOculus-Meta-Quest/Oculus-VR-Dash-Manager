using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;

namespace OVR_Dash_Manager.Functions
{
    public static class WatchdogManager
    {
        private static Timer _watchdogTimer;

        public static void StartWatchdog()
        {
            // Start a timer that checks if the application is responding every minute
            _watchdogTimer = new Timer(CheckIfApplicationIsResponding, null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
        }

        private static void CheckIfApplicationIsResponding(object state)
        {
            // Get the current process
            var currentProcess = Process.GetCurrentProcess();

            // Check if the process is responding
            if (!currentProcess.Responding)
            {
                // Handle unresponsive state, e.g., restart the application
                RestartApplication();
            }
        }

        private static void RestartApplication()
        {
            // Your restart logic here
            // Example: Restart the application
            var executableLocation = Assembly.GetExecutingAssembly().Location;
            System.Diagnostics.Process.Start(executableLocation);
            Environment.Exit(0);
        }
    }
}