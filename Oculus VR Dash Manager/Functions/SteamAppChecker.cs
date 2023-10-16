using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using YOVR_Dash_Manager.Functions;

namespace OVR_Dash_Manager.Functions
{
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
                    string steamPath = GetSteamPath();
                    if (string.IsNullOrEmpty(steamPath))
                    {
                        ErrorLogger.LogError(new Exception("Steam path not found or invalid."));
                        return false;
                    }

                    List<string> libraryPaths = GetLibraryPaths(steamPath);
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
            string steamPath = (string)Microsoft.Win32.Registry.GetValue(@"HKEY_CURRENT_USER\Software\Valve\Steam", "SteamPath", null);
            return steamPath;
        }

        /// <summary>
        /// Retrieves all library paths where Steam apps can be installed.
        /// </summary>
        /// <param name="steamPath">The installation path of Steam.</param>
        /// <returns>A list of library paths.</returns>
        private static List<string> GetLibraryPaths(string steamPath)
        {
            List<string> libraryPaths = new List<string> { steamPath };
            string libraryFoldersVdfPath = Path.Combine(steamPath, @"steamapps\libraryfolders.vdf");

            if (File.Exists(libraryFoldersVdfPath))
            {
                string[] lines = File.ReadAllLines(libraryFoldersVdfPath);
                foreach (string line in lines)
                {
                    Match match = Regex.Match(line, "\"path\"\\s+\"([^\"]+)\"");
                    if (match.Success)
                    {
                        string path = match.Groups[1].Value.Replace("\\\\", "\\");
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
            List<string> installedApps = new List<string>();

            foreach (string libraryPath in libraryPaths)
            {
                string appsDirectoryPath = Path.Combine(libraryPath, @"steamapps\common");

                if (Directory.Exists(appsDirectoryPath))
                {
                    var appDirectories = Directory.GetDirectories(appsDirectoryPath);
                    installedApps.AddRange(appDirectories.Select(Path.GetFileName));
                }
            }

            return installedApps;
        }
    }
}
