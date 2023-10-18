using System;
using System.Diagnostics;
using YOVR_Dash_Manager.Functions; // Ensure this using directive is added to access the ErrorLogger

namespace OVR_Dash_Manager.Functions
{
    public static class OculusDebugToolFunctions
    {
        public static string ExecuteCommand(string command, string parameters)
        {
            string output = string.Empty;
            try
            {
                // Define the path to OculusDebugToolCLI.exe
                string toolPath = @"C:\Program Files\Oculus\Support\oculus-diagnostics\OculusDebugToolCLI.exe";

                // Create a new process to run the command
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = toolPath;
                    process.StartInfo.Arguments = $"{command} {parameters}";
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = true;

                    process.Start();

                    // Read the output of the command
                    output = process.StandardOutput.ReadToEnd();

                    process.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                // Log the error using the ErrorLogger
                ErrorLogger.LogError(ex, $"An error occurred while executing the command: {command} {parameters}");
            }

            return output;
        }
    }
}
