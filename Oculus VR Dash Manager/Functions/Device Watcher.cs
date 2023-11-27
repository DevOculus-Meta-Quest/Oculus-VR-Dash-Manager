using System;
using System.Diagnostics;
using System.Management;

namespace OVR_Dash_Manager.Functions
{
    public static class DeviceWatcher
    {
        // Delegate (if using non-generic pattern).
        public delegate void NewDevice();

        // Event to be invoked when a new device is connected.
        public static event NewDevice DeviceConnected;

        // ManagementEventWatcher instance to monitor device connection events.
        private static ManagementEventWatcher _connected;

        // Flags to indicate whether the setup is done and whether the watcher is running.
        private static bool _isSetup;

        private static bool _running;

        // Timestamp to track the last connection event.
        private static DateTime _lastConnectionTime;

        // Setup method to initialize the ManagementEventWatcher instance.
        private static void Setup()
        {
            if (_isSetup) return;

            try
            {
                _isSetup = true;
                if (_connected != null) return;

                var query = new WqlEventQuery
                {
                    EventClassName = "Win32_DeviceChangeEvent"
                };

                _connected = new ManagementEventWatcher(query);
                _connected.EventArrived += Handle_DeviceConnected;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("Error during creation of ManagementEventWatcher for Win32_DeviceChangeEvent");
            }
        }

        // Method to start monitoring device connection events.
        public static void Start()
        {
            Setup();

            if (_connected == null || _running) return;

            try
            {
                _connected.Start();
                _running = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        // Method to stop monitoring device connection events.
        public static void Stop()
        {
            if (_connected == null || !_running) return;

            try
            {
                _connected.Stop();
                _running = false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        // Event handler to process device connection events.
        private static void Handle_DeviceConnected(object sender, EventArrivedEventArgs e)
        {
            // If nothing is subscribed to the event
            if (DeviceConnected == null) return;

            // Only take action if it's a connection event
            if (int.Parse(e.NewEvent.GetPropertyValue("EventType").ToString()) != 2) return;

            // Limits event spam to once per second
            if (DateTime.Now - _lastConnectionTime < TimeSpan.FromSeconds(1)) return;
            _lastConnectionTime = DateTime.Now;

            // Invoke the DeviceConnected event.
            DeviceConnected();
        }
    }
}