using System;
using System.IO;

namespace OVR_Dash_Manager.Functions
{
    internal static class EnvLoader // Changed class name to EnvLoader
    {
        public static void LoadEnvVariables(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("The .env file was not found.");

            foreach (var line in File.ReadAllLines(filePath))
            {
                var parts = line.Split('=', StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length != 2)
                    continue; // or handle the error

                Environment.SetEnvironmentVariable(parts[0].Trim(), parts[1].Trim());
            }
        }
    }
}
