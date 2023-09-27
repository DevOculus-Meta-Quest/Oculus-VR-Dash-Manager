using System;
using System.Diagnostics;
using System.IO;

namespace OVR_Dash_Manager.Functions.ProcessFunctions
{
    public static class ProcessFunctions
    {
        public static string GetCurrentProcessDirectory()
        {
            // Get the full path of the executing assembly (including the assembly name and extension)
            string assemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;

            // Extract the directory path from the full assembly path
            string directoryPath = Path.GetDirectoryName(assemblyLocation);

            return directoryPath;
        }
    }
}
