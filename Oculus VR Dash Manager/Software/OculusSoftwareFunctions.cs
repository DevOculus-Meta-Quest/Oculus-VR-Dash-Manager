using OVR_Dash_Manager.Functions;
using System;
using System.Collections.Generic;
using YOVR_Dash_Manager.Functions; // Ensure this using directive is added to access the OculusAppChecker

namespace OVR_Dash_Manager.Software
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

        public static string[] GetInstalledOculusGames()
        {
            try
            {
                // Assuming OculusAppChecker has a method to get installed Oculus apps
                List<string> installedApps = OculusAppChecker.GetInstalledApps();

                // Return the app names as a string array
                return installedApps.ToArray();
            }
            catch (Exception ex)
            {
                // Log the error using the ErrorLogger
                ErrorLogger.LogError(ex, "An error occurred while retrieving the list of installed Oculus games.");
                return new string[0]; // Return an empty array in case of an error
            }
        }
    }
}