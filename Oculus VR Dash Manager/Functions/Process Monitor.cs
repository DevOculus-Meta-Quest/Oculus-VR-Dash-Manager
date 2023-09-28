using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;

namespace OVR_Dash_Manager.Functions
{
    public static class ProcessWatcher
    {
        public delegate void ProcessEventHandler(string processName, int processId);

        public static event ProcessEventHandler ProcessStarted;

        public static event ProcessEventHandler ProcessExited;

        private static readonly ManagementEventWatcher ProcessStartEventWatcher;
        private static readonly ManagementEventWatcher ProcessStopEventWatcher;

        private static readonly HashSet<string> IgnoredExeNames = new HashSet<string>();

        static ProcessWatcher()
        {
            // Initialize the ManagementEventWatchers with the appropriate WQL queries
            ProcessStartEventWatcher = new ManagementEventWatcher("SELECT * FROM __InstanceCreationEvent WITHIN 1 WHERE TargetInstance ISA 'Win32_Process'");
            ProcessStopEventWatcher = new ManagementEventWatcher("SELECT * FROM __InstanceDeletionEvent WITHIN 1 WHERE TargetInstance ISA 'Win32_Process'");

            ProcessStartEventWatcher.EventArrived += OnProcessStarted;
            ProcessStopEventWatcher.EventArrived += OnProcessExited;
        }

        public static void IgnoreExeName(string exeName) => IgnoredExeNames.Add(exeName);

        public static void RemoveIgnoreExeName(string exeName) => IgnoredExeNames.Remove(exeName);

        public static bool Start()
        {
            try
            {
                ProcessStartEventWatcher.Start();
                ProcessStopEventWatcher.Start();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        public static bool Stop()
        {
            try
            {
                ProcessStartEventWatcher.Stop();
                ProcessStopEventWatcher.Stop();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        public static void Dispose()
        {
            Stop();
            IgnoredExeNames.Clear();
            ProcessStartEventWatcher.Dispose();
            ProcessStopEventWatcher.Dispose();
        }

        private static void OnProcessStarted(object sender, EventArrivedEventArgs e) => HandleEvent(e, ProcessStarted);

        private static void OnProcessExited(object sender, EventArrivedEventArgs e) => HandleEvent(e, ProcessExited);

        private static void HandleEvent(EventArrivedEventArgs e, ProcessEventHandler handler)
        {
            if (handler == null) return;

            var targetInstance = (ManagementBaseObject)e.NewEvent["TargetInstance"];
            var name = targetInstance["Name"]?.ToString();
            var id = Convert.ToInt32(targetInstance["Handle"]?.ToString());

            if (!IgnoredExeNames.Contains(name))
                handler(name, id);
        }
    }
}