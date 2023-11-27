using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace OVR_Dash_Manager.Functions.Oculus
{
    public static class OculusSoftwareFunctions
    {
        public static bool IsOculusInstalled()
        {
            try
            {
                // Assuming OculusAppChecker has a method to check if Oculus is installed
                return OculusAppChecker.IsOculusInstalled();
            }
            catch (Exception ex)
            {
                // Log the error using the ErrorLogger
                ErrorLogger.LogError(ex, "An error occurred while checking if Oculus is installed.");
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
                        .Where(name => !Regex.IsMatch(name, @"^[A-Z]_"))
                        // Replace "-" with spaces in the name
                        .Select(name => name.Replace("-", " "))
                        .ToList();

                    installedApps.AddRange(appDirectories);
                }
            }

            return installedApps;
        }
    }
}