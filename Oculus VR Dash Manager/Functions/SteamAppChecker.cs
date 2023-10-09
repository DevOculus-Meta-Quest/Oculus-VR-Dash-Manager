using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace OVR_Dash_Manager.Functions
{
    public static class SteamAppChecker
    {
        public static bool IsDebugEnabled { get; set; } = true; // Toggle for debugging

        public static bool IsAppInstalled(string appName)
        {
            try
            {
                string steamPath = GetSteamPath();
                if (string.IsNullOrEmpty(steamPath))
                {
                    Log("Steam path not found or invalid.");
                    return false;
                }

                List<string> libraryPaths = GetLibraryPaths(steamPath);
                return CheckAppInLibraryPaths(libraryPaths, appName);
            }
            catch (Exception ex)
            {
                Log($"An error occurred: {ex.Message}");
                return false;
            }
        }

        private static string GetSteamPath()
        {
            string steamPath = (string)Microsoft.Win32.Registry.GetValue(@"HKEY_CURRENT_USER\Software\Valve\Steam", "SteamPath", null);
            Log($"Steam path: {steamPath}");
            return steamPath;
        }

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

        private static bool CheckAppInLibraryPaths(List<string> libraryPaths, string appName)
        {
            foreach (string libraryPath in libraryPaths)
            {
                Log($"Checking library path: {libraryPath}");
                string appPath = Path.Combine(libraryPath, $@"steamapps\common\{appName}");
                if (Directory.Exists(appPath))
                {
                    Log($"{appName} found!");
                    return true;
                }
            }
            Log($"{appName} not found.");
            return false;
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
