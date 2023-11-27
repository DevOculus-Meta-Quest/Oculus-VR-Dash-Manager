using System;
using System.Diagnostics;

namespace OVR_Dash_Manager.Functions
{
    internal class WebUtilities
    {
        public static void OpenURL(string url)
        {
            try
            {
                var ps = new ProcessStartInfo(url)
                {
                    UseShellExecute = true,
                    Verb = "open"
                };

                Process.Start(ps);
            }
            catch (Exception ex)
            {
                // Log the exception with your ErrorLogger
                ErrorLogger.LogError(ex, $"Failed to open URL: {url}");

                // Optionally: Inform the user about the error
                // MessageBox.Show("Failed to open the URL. Please check your internet connection and try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}