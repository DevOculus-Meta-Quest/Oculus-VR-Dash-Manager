using AdvancedSharpAdbClient;
using Microsoft.Win32;
using System.IO;
using System.Linq;

namespace OVR_Dash_Manager.Software
{
    public static class Oculus_Link
    {
        public static void StartLinkOnDevice()
        {
            if (Properties.Settings.Default.QuestPolling)
            {
                ADB.Start();  // Ensure ADB is started

                // Allow time for quest to register with ADB server
                System.Threading.Thread.Sleep(1000);

                var connectedDevices = USB_Devices_Functions.GetUSBDevices();

                foreach (var device in connectedDevices)
                {
                    if (string.IsNullOrEmpty(device.FullSerial) || device.Type != "Quest") continue;

                    var client = new AdbClient();
                    var adbDevices = client.GetDevices();

                    // Ensure adb only interacts with quest device serial nos
                    foreach (var adbDevice in adbDevices.Where(adbDevice => device.FullSerial == adbDevice.Serial))
                    {
                        // Only start quest link if adb has been authorized
                        if (adbDevice.State == DeviceState.Online)
                        {
                            client.StartApp(adbDevice, "com.oculus.xrstreamingclient");
                        }
                    }
                }
            }
        }

        public static void ResetLink()
        {
            if (Service_Manager.GetState("OVRService") == "Running")
            {
                Steam.ManagerCalledExit = true;

                Service_Manager.StopService("OVRService");
                Service_Manager.StartService("OVRService");

                Steam.ManagerCalledExit = true;
            }
        }

        public static void StopLink()
        {
            if (Service_Manager.GetState("OVRService") == "Running")
            {
                Steam.ManagerCalledExit = true;

                Service_Manager.StopService("OVRService");

                Steam.ManagerCalledExit = true;
            }
        }

        public static void StartLink()
        {
            if (Service_Manager.GetState("OVRService") != "Running")
            {
                Service_Manager.StartService("OVRService");
            }
        }

        public static void SetToOculusRunTime()
        {
            if (Oculus.Oculus_Is_Installed)
            {
                // Update the following line to include the correct namespace for RegistryKeyType
                var runTimeKey = Functions.RegistryFunctions.GetRegistryKey(OVR_Dash_Manager.Functions.RegistryKeyType.LocalMachine, @"SOFTWARE\Khronos\OpenXR\1");

                if (runTimeKey != null)
                {
                    var oculusRunTimePath = Path.Combine(Oculus.Oculus_Main_Directory, "Support\\oculus-runtime\\oculus_openxr_64.json");

                    if (File.Exists(oculusRunTimePath))
                    {
                        // Specify the value kind as ExpandString when setting a REG_EXPAND_SZ value
                        Functions.RegistryFunctions
                            .SetKeyValue(runTimeKey, "ActiveRuntime", oculusRunTimePath, RegistryValueKind.ExpandString);
                    }

                    Functions.RegistryFunctions.CloseKey(runTimeKey);

                    Dashes.Dash_Manager.MainForm_CheckRunTime();
                }
            }
        }
    }
}