using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OVR_Dash_Manager.Functions
{
    public static class OculusAppChecker
    {
        // Cache for installed apps
        private static List<string> _installedApps;

        /// <summary>
        /// Checks if a specific Oculus app is installed.
        /// </summary>
        /// <param name="appName">The name of the app to check.</param>
        /// <returns>True if the app is installed; otherwise, false.</returns>
        public static bool IsOculusAppInstalled(string appName)
        {
            try
            {
                // Ensure the cache is populated
                if (_installedApps == null)
                {
                    List<string> oculusPaths = GetOculusPaths();
                    _installedApps = GetInstalledApps(oculusPaths);
                }

                // Check the cache for the app
                return _installedApps.Contains(appName, StringComparer.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex, "Error checking if Oculus app is installed.");
                return false;
            }
        }

        /// <summary>
        /// Public method to get the names of all installed Oculus apps.
        /// </summary>
        /// <returns>A list of installed Oculus app names.</returns>
        public static List<string> GetInstalledApps()
        {
            // Ensure the cache is populated
            IsOculusAppInstalled("AnyKnownAppName");

            // Return the cached app names
            return _installedApps;
        }

        /// <summary>
        /// Retrieves all paths where Oculus apps are installed.
        /// </summary>
        /// <returns>A list of Oculus app paths.</returns>
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
                                if (!string.IsNullOrEmpty(path))
                                {
                                    // Append "Software/Software" to the path
                                    string adjustedPath = Path.Combine(path, "Software");
                                    if (Directory.Exists(adjustedPath))
                                    {
                                        oculusPaths.Add(adjustedPath);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return oculusPaths;
        }

        public static bool IsOculusInstalled()
        {
            try
            {
                // Check the registry for Oculus installation
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"Software\Oculus VR, LLC\Oculus"))
                {
                    if (key != null)
                    {
                        string installDir = (string)key.GetValue("InstallDir");
                        if (!string.IsNullOrEmpty(installDir) && Directory.Exists(installDir))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex, "Error checking if Oculus is installed.");
                return false;
            }
        }

        /// <summary>
        /// Retrieves the names of all installed Oculus apps.
        /// </summary>
        /// <param name="oculusPaths">A list of paths to check.</param>
        /// <returns>A list of installed Oculus app names.</returns>
        private static List<string> GetInstalledApps(List<string> oculusPaths)
        {
            List<string> installedApps = new List<string>();

            foreach (string oculusPath in oculusPaths)
            {
                if (Directory.Exists(oculusPath))
                {
                    var appDirectories = Directory.GetDirectories(oculusPath);
                    installedApps.AddRange(appDirectories.Select(Path.GetFileName));
                }
            }

            return installedApps;
        }
    }
}