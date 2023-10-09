using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;

namespace OVR_Dash_Manager.Functions
{
    public static class OculusAppChecker
    {
        public static bool IsDebugEnabled { get; set; } = true; // Toggle for debugging

        public static bool IsOculusAppInstalled(string appName)
        {
            try
            {
                List<string> oculusPaths = GetOculusPaths();
                foreach (string oculusPath in oculusPaths)
                {
                    string appPath = Path.Combine(oculusPath, appName);
                    if (Directory.Exists(appPath))
                    {
                        Log($"{appName} found in {oculusPath}!");
                        return true;
                    }
                }
                Log($"{appName} not found.");
                return false;
            }
            catch (Exception ex)
            {
                Log($"An error occurred: {ex.Message}");
                return false;
            }
        }

        private static List<string> GetOculusPaths()
        {
            List<string> oculusPaths = new List<string>();

            // Check the registry for Oculus paths
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Oculus VR, LLC\Oculus\Libraries"))
            {
                if (key != null)
                {
                    foreach (string subKeyName in key.GetSubKeyNames())
                    {
                        using (RegistryKey subKey = key.OpenSubKey(subKeyName))
                        {
                            if (subKey != null)
                            {
                                string path = (string)subKey.GetValue("OriginalPath");
                                if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
                                {
                                    oculusPaths.Add(path);
                                    Log($"Oculus path from registry: {path}");
                                }
                            }
                        }
                    }
                }
            }

            return oculusPaths;
        }

        private static void Log(string message)
        {
            if (IsDebugEnabled)
            {
                Debug.WriteLine(message);
            }
        }
    }
}
