using System;
using System.Collections.Generic;
using System.Management;

namespace OVR_Dash_Manager.Functions
{
    public static class USB_Devices_Functions
    {
        /// <summary>
        /// Retrieves a list of USB devices connected to the system.
        /// </summary>
        /// <returns>A list of USBDeviceInfo objects representing the connected USB devices.</returns>
        public static List<USBDeviceInfo> GetUSBDevices()
        {
            List<USBDeviceInfo> PluggedInDevices = new List<USBDeviceInfo>();

            // Querying the system for USB devices using WMI
            using (ManagementObjectSearcher Searcher = new ManagementObjectSearcher(@"Select * From Win32_PnPEntity WHERE DeviceID LIKE '%VID_2833%'"))
                PluggedInDevices.AddRange(ReadSearcher(Searcher.Get()));

            return PluggedInDevices;
        }

        /// <summary>
        /// Processes the ManagementObjectCollection to extract USB device information.
        /// </summary>
        /// <param name="Devices">The collection of ManagementObjects representing USB devices.</param>
        /// <returns>A list of USBDeviceInfo objects.</returns>
        private static List<USBDeviceInfo> ReadSearcher(ManagementObjectCollection Devices)
        {
            // Dictionary to map device IDs to human-readable names
            Dictionary<string, string> DeviceIDs = new Dictionary<string, string>
            {
            { "VID_2833&PID_2031", "Rift CV1" },
            { "VID_2833&PID_3031", "Rift CV1" },
            { "VID_2833&PID_0137", "Quest Headset" },
            { "VID_2833&PID_0201", "Camera DK2" },
            { "VID_2833&PID_0211", "Rift CV1 Sensor" },
            { "VID_2833&PID_0330", "Rift CV1 Audio" },
            { "VID_2833&PID_1031", "Rift CV1" },
            { "VID_2833&PID_2021", "Rift DK2" },
            { "VID_2833&PID_0001", "Rift Developer Kit 1" },
            { "VID_2833&PID_0021", "Rift DK2" },
            { "VID_2833&PID_0031", "Rift CV1" },
            { "VID_2833&PID_0101", "Latency Tester" },
            { "VID_2833&PID_0183", "Quest" },
            { "VID_2833&PID_0182", "Quest" },
            { "VID_2833&PID_0186", "Quest" },
            { "VID_2833&PID_0083", "Quest" },
            { "VID_2833&PID_0186&MI_00", "Quest XRSP" },
            { "VID_2833&PID_0186&MI_01", "Quest ADB" },
            { "VID_2833&PID_0183&MI_00", "Quest XRSP" },
            { "VID_2833&PID_0183&MI_01", "Quest ADB" },
            };

            List<USBDeviceInfo> PluggedInDevices = new List<USBDeviceInfo>();

            foreach (ManagementObject oDevice in Devices)
            {
                try
                {
                    string DeviceID = TryGetProperty(oDevice, "DeviceID").ToString();
                    string DeviceCaption = TryGetProperty(oDevice, "Caption").ToString();

                    string[] Data = DeviceID.Split('\\');
                    string Type = "";
                    string Serial = "";
                    string MaskedSerial = "";

                    if (Data.Length == 3)
                    {
                        Serial = Data[2];

                        if (Serial.Contains("&"))
                            Serial = "";

                        if (!DeviceIDs.TryGetValue(Data[1], out Type))
                            Type = "Unknown - " + Data[1];

                        if (DeviceCaption.StartsWith("USB Comp"))
                            DeviceCaption = Type;

                        if (DeviceCaption.Length > 0)
                            Type = DeviceCaption;

                        if (Serial.Length > 5)
                        {
                            MaskedSerial = new string('*', Serial.Length - 5) + Serial.Substring(Serial.Length - 5);
                        }
                        else
                        {
                            MaskedSerial = Serial;  // If Serial is less than 5 characters, don't mask it
                        }

                        PluggedInDevices.Add(new USBDeviceInfo(DeviceID, Type, MaskedSerial, Serial));
                    }
                }
                catch (Exception)
                {
                    // Handle exceptions as needed
                }
            }

            return PluggedInDevices;
        }

        /// <summary>
        /// Attempts to retrieve a property value from a ManagementObject.
        /// </summary>
        /// <param name="wmiObj">The ManagementObject.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The value of the property, or null if an error occurs.</returns>
        private static object TryGetProperty(ManagementObject wmiObj, string propertyName)
        {
            object retval;
            try
            {
                retval = wmiObj.GetPropertyValue(propertyName);
            }
            catch (ManagementException)
            {
                retval = null;
            }
            return retval;
        }
    }

    /// <summary>
    /// Represents information about a USB device.
    /// </summary>
    public class USBDeviceInfo
    {
        /// <summary>
        /// Initializes a new instance of the USBDeviceInfo class.
        /// </summary>
        /// <param name="DeviceID">The device ID.</param>
        /// <param name="Type">The type of device.</param>
        /// <param name="MaskedSerial">The masked serial number.</param>
        /// <param name="FullSerial">The full serial number.</param>
        public USBDeviceInfo(string DeviceID, string Type, string MaskedSerial, string FullSerial)
        {
            this.DeviceID = DeviceID;
            this.Type = Type;
            this.MaskedSerial = MaskedSerial;
            this.FullSerial = FullSerial;
        }

        public string DeviceID { get; private set; }
        public string Type { get; private set; }
        public string MaskedSerial { get; private set; }
        public string FullSerial { get; private set; }
    }
}