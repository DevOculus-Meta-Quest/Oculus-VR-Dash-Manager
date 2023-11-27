using Microsoft.Win32;
using Newtonsoft.Json.Linq; // You might need to use Newtonsoft.Json or another JSON library
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace OVR_Dash_Manager.Functions.Steam
{
    public class SteamAppDetails
    {
        public string Name { get; set; }
        public string ID { get; set; }
        public string InstallPath { get; set; }
        public string ImagePath { get; set; }
    }

    public static class SteamAppChecker
    {
        // Cache for installed apps
        private static List<string> _installedApps;

        /// <summary>
        /// Checks if a specific Steam app is installed.
        /// </summary>
        /// <param name="appName">The name of the app to check.</param>
        /// <returns>True if the app is installed; otherwise, false.</returns>
        public static bool IsAppInstalled(string appName)
        {
            try
            {
                // Ensure the cache is populated
                if (_installedApps == null)
                {
                    var steamPath = GetSteamPath();

                    if (string.IsNullOrEmpty(steamPath))
                    {
                        ErrorLogger.LogError(new Exception("Steam path not found or invalid."));
                        return false;
                    }

                    var libraryPaths = GetLibraryPaths(steamPath);
                    _installedApps = GetInstalledApps(libraryPaths);
                }

                // Check the cache for the app
                return _installedApps.Contains(appName, StringComparer.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex, "Error checking if app is installed.");
                return false;
            }
        }

        /// <summary>
        /// Public method to get the names of all installed Steam apps.
        /// </summary>
        /// <returns>A list of installed Steam app names.</returns>
        public static List<string> GetInstalledApps()
        {
            // Ensure the cache is populated
            IsAppInstalled("AnyKnownAppName");

            // Return the cached app names
            return _installedApps;
        }

        /// <summary>
        /// Retrieves the installation path of Steam from the registry.
        /// </summary>
        /// <returns>The installation path of Steam.</returns>
        private static string GetSteamPath()
        {
            var steamPath = (string)Registry.GetValue(@"HKEY_CURRENT_USER\Software\Valve\Steam", "SteamPath", null);
            return steamPath;
        }

        /// <summary>
        /// Retrieves all library paths where Steam apps can be installed.
        /// </summary>
        /// <param name="steamPath">The installation path of Steam.</param>
        /// <returns>A list of library paths.</returns>
        private static List<string> GetLibraryPaths(string steamPath)
        {
            var libraryPaths = new List<string> { steamPath };
            var libraryFoldersVdfPath = Path.Combine(steamPath, @"steamapps\libraryfolders.vdf");

            if (File.Exists(libraryFoldersVdfPath))
            {
                var lines = File.ReadAllLines(libraryFoldersVdfPath);

                foreach (string line in lines)
                {
                    var match = Regex.Match(line, "\"path\"\\s+\"([^\"]+)\"");

                    if (match.Success)
                    {
                        var path = match.Groups[1].Value.Replace("\\\\", "\\");

                        if (Directory.Exists(path))
                        {
                            libraryPaths.Add(path);
                        }
                    }
                }
            }

            return libraryPaths;
        }

        /// <summary>
        /// Retrieves the names of all installed Steam apps.
        /// </summary>
        /// <param name="libraryPaths">A list of library paths to check.</param>
        /// <returns>A list of installed Steam app names.</returns>
        private static List<string> GetInstalledApps(List<string> libraryPaths)
        {
            var installedApps = new List<string>();

            foreach (string libraryPath in libraryPaths)
            {
                var appsDirectoryPath = Path.Combine(libraryPath, @"steamapps\common");

                if (Directory.Exists(appsDirectoryPath))
                {
                    var appDirectories = Directory.GetDirectories(appsDirectoryPath);
                    installedApps.AddRange(appDirectories.Select(Path.GetFileName));
                }
            }

            return installedApps;
        }

        /// <summary>
        /// Checks if SteamVR is running a beta version.
        /// </summary>
        /// <returns>True if it's a beta version; otherwise, false.</returns>
        public static bool IsSteamVRBeta()
        {
            try
            {
                var steamPath = GetSteamPath();
                var manifestPath = Path.Combine(steamPath, @"steamapps\appmanifest_250820.acf");

                if (File.Exists(manifestPath))
                {
                    var content = File.ReadAllText(manifestPath);
                    var match = Regex.Match(content, "\"betakey\"[^\"]*\"([^\"]+)\"", RegexOptions.IgnoreCase);

                    if (match.Success)
                    {
                        return !string.IsNullOrEmpty(match.Groups[1].Value);
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex, "Error checking if SteamVR is a beta version.");
                return false;
            }
        }

        /// <summary>
        /// Retrieves details for all installed Steam apps.
        /// </summary>
        /// <returns>A list of SteamAppDetails with information about each installed app.</returns>
        public static List<SteamAppDetails> GetSteamAppDetails()
        {
            var appDetailsList = new List<SteamAppDetails>();
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

                        var appName = jsonObject["canonicalName"]?.ToString();

                        if (appName != null && appName.Contains("_steamapps_") && !appName.EndsWith("_assets"))
                        {
                            var appID = jsonObject["appId"]?.ToString();
                            var installPath = jsonObject["install_path"]?.ToString(); // If install_path is provided

                            var assetFolderName = ConvertAppNameToAssetFolderName(appName);
                            var appAssetsPath = Path.Combine(storeAssetsPath, assetFolderName);

                            var imageFileName = "cover_square_image.jpg";
                            var imagePath = Path.Combine(appAssetsPath, imageFileName);

                            if (!File.Exists(imagePath))
                            {
                                ErrorLogger.LogError(new FileNotFoundException(), $"Image file not found: {imagePath}");
                                imagePath = null; // Set imagePath to null if the image file is not found
                            }

                            var appDetails = new SteamAppDetails
                            {
                                Name = appName,
                                ID = appID,
                                InstallPath = installPath,
                                ImagePath = imagePath
                            };

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
            return appName.Replace(" ", "-").ToLower() + "_assets";
        }
    }
}