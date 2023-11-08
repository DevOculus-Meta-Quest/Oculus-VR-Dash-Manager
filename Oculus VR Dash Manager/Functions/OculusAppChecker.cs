using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq; // You might need to use Newtonsoft.Json or another JSON library

namespace OVR_Dash_Manager.Functions
{
    public class OculusAppDetails
    {
        public string Name { get; set; }
        public string ID { get; set; }
        public string InstallPath { get; set; }
        public string ImagePath { get; set; }
    }

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

        /// <summary>
        /// Retrieves details for all installed Oculus apps.
        /// </summary>
        /// <returns>A list of OculusAppDetails with information about each installed app.</returns>
        public static List<OculusAppDetails> GetOculusAppDetails()
        {
            var appDetailsList = new List<OculusAppDetails>();
            var manifestsPath = @"C:\Program Files\Oculus\CoreData\Manifests";
            var storeAssetsPath = @"C:\Program Files\Oculus\CoreData\Software\StoreAssets";

            if (Directory.Exists(manifestsPath))
            {
                var manifestFiles = Directory.GetFiles(manifestsPath, "*.json");

                foreach (var manifestFile in manifestFiles)
                {
                    try
                    {
                        var jsonData = File.ReadAllText(manifestFile);
                        var jsonObject = JObject.Parse(jsonData);

                        // Assuming the app's ID is used as a directory name under StoreAssets
                        var appID = jsonObject["appId"]?.ToString();
                        var appAssetsPath = Path.Combine(storeAssetsPath, appID);

                        // Assuming 'cover_square_image.jpg' is the image you want to use
                        var imagePath = Path.Combine(appAssetsPath, "cover_square_image.jpg");

                        var appDetails = new OculusAppDetails
                        {
                            Name = jsonObject["canonicalName"]?.ToString(),
                            ID = appID,
                            InstallPath = jsonObject["install_path"]?.ToString(), // If install_path is provided
                            ImagePath = File.Exists(imagePath) ? imagePath : null
                        };

                        appDetailsList.Add(appDetails);
                    }
                    catch (Exception ex)
                    {
                        ErrorLogger.LogError(ex, $"Error parsing manifest file: {manifestFile}");
                    }
                }
            }

            return appDetailsList;
        }
    }
}