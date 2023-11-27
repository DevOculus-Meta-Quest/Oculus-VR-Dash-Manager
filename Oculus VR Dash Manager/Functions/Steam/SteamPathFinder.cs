using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace OVR_Dash_Manager.Functions.Steam
{
    internal class SteamPathFinder
    {
        public static string FindSteamInstallPath()
        {
            try
            {
                string registryKey = @"HKEY_CURRENT_USER\Software\Valve\Steam";
                string steamPath = (string)Registry.GetValue(registryKey, "SteamPath", null);

                if (!string.IsNullOrEmpty(steamPath))
                {
                    Debug.WriteLine($"Steam is installed at: {steamPath}");
                    return steamPath;
                }
                else
                {
                    Debug.WriteLine("Steam installation not found.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex, "Error in FindSteamInstallPath");
                return null;
            }
        }

        public static List<string> FindAllGamePaths()
        {
            try
            {
                var allPaths = new List<string>();
                string mainSteamPath = FindSteamInstallPath() ?? GetSteamPath();

                if (!string.IsNullOrEmpty(mainSteamPath))
                {
                    allPaths.Add(mainSteamPath);
                    string libraryFoldersPath = Path.Combine(mainSteamPath, "steamapps", "libraryfolders.vdf");

                    if (File.Exists(libraryFoldersPath))
                    {
                        var libraryPaths = ParseLibraryFoldersVdf(libraryFoldersPath);
                        allPaths.AddRange(libraryPaths);
                    }
                }

                return allPaths;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex, "Error in FindAllGamePaths");
                return new List<string>();
            }
        }

        private static string GetSteamPath()
        {
            string registryKeyPath = @"SOFTWARE\WOW6432Node\Valve\Steam";
            string registryValueName = "InstallPath";

            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(registryKeyPath))
            {
                if (key != null)
                {
                    object value = key.GetValue(registryValueName);
                    if (value != null)
                    {
                        return value.ToString();
                    }
                }
            }

            // Check for 32-bit system
            registryKeyPath = @"SOFTWARE\Valve\Steam";
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(registryKeyPath))
            {
                if (key != null)
                {
                    object value = key.GetValue(registryValueName);
                    if (value != null)
                    {
                        return value.ToString();
                    }
                }
            }

            return null;
        }

        private static List<string> ParseLibraryFoldersVdf(string filePath)
        {
            try
            {
                var paths = new List<string>();
                var fileContent = File.ReadAllText(filePath);
                var matches = Regex.Matches(fileContent, "\"path\"\\s*\"(.+?)\"");

                foreach (Match match in matches)
                {
                    if (match.Groups.Count > 1)
                    {
                        paths.Add(match.Groups[1].Value);
                    }
                }

                return paths;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex, "Error in ParseLibraryFoldersVdf");
                return new List<string>();
            }
        }
    }
}