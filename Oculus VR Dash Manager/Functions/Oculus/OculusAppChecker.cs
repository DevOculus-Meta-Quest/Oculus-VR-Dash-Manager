using Microsoft.Win32;
using Newtonsoft.Json.Linq; // You might need to use Newtonsoft.Json or another JSON library
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace OVR_Dash_Manager.Functions.Oculus
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
                    var oculusPaths = GetOculusPaths();
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
            var oculusPaths = new List<string>();

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
                                var path = (string)subKey.GetValue("OriginalPath");

                                if (!string.IsNullOrEmpty(path))
                                {
                                    // Append "Software/Software" to the path
                                    var adjustedPath = Path.Combine(path, "Software");

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
                        var installDir = (string)key.GetValue("InstallDir");

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
            var installedApps = new List<string>();

            foreach (string oculusPath in oculusPaths)
            {
                if (Directory.Exists(oculusPath))
                {
                    var appDirectories = Directory.GetDirectories(oculusPath)
                        .Select(Path.GetFileName)
                        // Exclude directories that start with a drive letter pattern
                        .Where(name => !name.StartsWith("C_") && !name.StartsWith("D_") && !name.StartsWith("E_") && !name.StartsWith("F_") && !name.StartsWith("G_") && !name.StartsWith("H_"))
                        .ToList();

                    installedApps.AddRange(appDirectories);
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

                        var appName = jsonObject["canonicalName"]?.ToString().Replace("_assets", "").Replace("-", " ");
                        var appID = jsonObject["appId"]?.ToString();
                        var installPath = jsonObject["install_path"]?.ToString(); // If install_path is provided

                        // Convert the app name to the asset folder name
                        var assetFolderName = ConvertAppNameToAssetFolderName(appName.Replace(" ", "-"));
                        var appAssetsPath = Path.Combine(storeAssetsPath, assetFolderName);

                        // Assuming 'cover_square_image.jpg' is the image you want to use
                        var imageFileName = "cover_square_image.jpg";
                        var imagePath = Path.Combine(appAssetsPath, imageFileName);

                        if (!File.Exists(imagePath))
                        {
                            // If the specific image file does not exist, log an error or handle accordingly
                            ErrorLogger.LogError(new FileNotFoundException(), $"Image file not found: {imagePath}");
                            imagePath = null; // Or set a default image path
                        }

                        var appDetails = new OculusAppDetails
                        {
                            Name = appName,
                            ID = appID,
                            InstallPath = installPath,
                            ImagePath = imagePath
                        };

                        // Exclude apps with names starting with a drive letter pattern
                        if (!Regex.IsMatch(appName, @"^[A-Z]_"))
                        {
                            appDetailsList.Add(appDetails);
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorLogger.LogError(ex, $"Error parsing manifest file: {manifestFile}");
                    }
                }
            }

            return appDetailsList;
        }

        private static string ConvertAppNameToAssetFolderName(string appName)
        {
            // Replace spaces with "-" and append "_assets" to the app name
            return appName.Replace(" ", "-").ToLower() + "_assets";
        }
    }
}