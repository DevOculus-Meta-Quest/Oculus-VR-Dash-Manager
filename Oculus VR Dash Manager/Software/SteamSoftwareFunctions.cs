using OVR_Dash_Manager.Functions;
using System;
using System.Collections.Generic;

namespace OVR_Dash_Manager.Software
{
    public static class SteamSoftwareFunctions
    {
        public static string[] GetInstalledSteamGames()
        {
            try
            {
                // Get all installed Steam apps using the SteamAppChecker function
                List<string> installedApps = SteamAppChecker.GetInstalledApps();

                // Return the app names as a string array
                return installedApps.ToArray();
            }
            catch (Exception ex)
            {
                // Log the error using the ErrorLogger
                ErrorLogger.LogError(ex, "An error occurred while retrieving the list of installed Steam games.");
                return new string[0]; // Return an empty array in case of an error
            }
        }
    }
}