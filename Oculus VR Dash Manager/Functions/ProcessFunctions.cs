using System.Diagnostics;
using System.IO;
using System.Security.Principal;

namespace OVR_Dash_Manager.Functions
{
    public static class ProcessFunctions
    {
        // Method to check if the current process is elevated
        public static bool IsCurrentProcessElevated()
        {
            using (var identity = WindowsIdentity.GetCurrent())
            {
                var principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        // Method to get the directory of the current executable
        public static string GetCurrentExecutableDirectory()
        {
            var currentProcess = Process.GetCurrentProcess();
            return Path.GetDirectoryName(currentProcess.MainModule.FileName);
        }

        // Method to get the directory of the current assembly
        public static string GetCurrentAssemblyDirectory()
        {
            var assemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
            return Path.GetDirectoryName(assemblyLocation);
        }

        // Method to start a process with given path and arguments
        public static Process StartProcess(string path, string arguments = "")
        {
            if (File.Exists(path))
            {
                return Process.Start(path, arguments);
            }
            else
            {
                // Try to build full URL or return the same input
                var url = StringManipulationUtilities.GetFullUrl(path);
                return Process.Start(url, arguments);
            }
        }

        // Additional helper methods can be added here if needed
    }
}