using Microsoft.Win32;
using System;
using System.IO;

namespace OVR_Dash_Manager.Functions
{
    public static class FrameworkChecker
    {
        public static bool IsDotNet48Installed()
        {
            try
            {
                using (var ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32)
                    .OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\"))
                {
                    if (ndpKey != null && ndpKey.GetValue("Release") != null)
                    {
                        int releaseKey = Convert.ToInt32(ndpKey.GetValue("Release"));
                        if (releaseKey >= 528040) // Release key for .NET Framework 4.8
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex, "Error checking .NET Framework 4.8 installation status");
            }
            return false;
        }

        public static bool IsDotNet5OrAboveInstalled()
        {
            try
            {
                // Check for .NET 5+ runtime directories
                string programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                string dotNetRuntimeDir = Path.Combine(programFiles, "dotnet", "shared", "Microsoft.NETCore.App");

                if (Directory.Exists(dotNetRuntimeDir))
                {
                    var directories = Directory.GetDirectories(dotNetRuntimeDir);
                    foreach (var dir in directories)
                    {
                        if (dir.Contains("5.") || dir.Contains("6."))
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex, "Error checking .NET 5+ runtime installation status");
            }
            return false;
        }
    }
}