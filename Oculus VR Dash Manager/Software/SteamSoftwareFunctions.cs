using OVR_Dash_Manager.Functions;
using Newtonsoft.Json; // Ensure Newtonsoft.Json is installed
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace OVR_Dash_Manager.Software
{
    public static class SteamSoftwareFunctions
    {
        public static List<NonSteamAppDetails> GetNonSteamAppDetails()
        {
            List<NonSteamAppDetails> nonSteamApps = new List<NonSteamAppDetails>();
            string vdfFilePath = @"C:\Program Files (x86)\Steam\userdata\201667287\config\shortcuts.vdf";
            string tempFilePath = Path.GetTempFileName(); // Generates a temporary file path

            Debug.WriteLine($"Checking if VDF file exists at {vdfFilePath}");
            if (File.Exists(vdfFilePath))
            {
                WriteParsedDataToTempFile(vdfFilePath, tempFilePath);
                nonSteamApps = ReadDataFromTempFile(tempFilePath);
            }
            else
            {
                ErrorLogger.LogError(new FileNotFoundException(), $"VDF file not found at {vdfFilePath}");
            }

            return nonSteamApps;
        }

        private static void WriteParsedDataToTempFile(string vdfFilePath, string tempFilePath)
        {
            try
            {
                VdfParser parser = new VdfParser();
                var parsedData = parser.ParseVdf(vdfFilePath);

                Debug.WriteLine("Parsed VDF Data: " + JsonConvert.SerializeObject(parsedData)); // Debug print

                string jsonData = JsonConvert.SerializeObject(parsedData);
                File.WriteAllText(tempFilePath, jsonData);

                Debug.WriteLine($"Written JSON data to temp file at {tempFilePath}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error writing to temp file: " + ex.Message);
            }
        }

        private static List<NonSteamAppDetails> ReadDataFromTempFile(string tempFilePath)
        {
            List<NonSteamAppDetails> nonSteamApps = new List<NonSteamAppDetails>();

            try
            {
                string jsonData = File.ReadAllText(tempFilePath);
                Debug.WriteLine("Read JSON data from temp file: " + jsonData); // Debug print

                var parsedData = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonData);

                VdfParser parser = new VdfParser(); // Create an instance of VdfParser
                var shortcutInfos = parser.ExtractSpecificData(parsedData); // Use the instance to call the method

                Debug.WriteLine("Extracted Shortcut Infos: " + shortcutInfos.Count); // Debug print

                foreach (var info in shortcutInfos)
                {
                    NonSteamAppDetails details = new NonSteamAppDetails
                    {
                        Name = info.AppName,
                        ExePath = info.Exe
                    };
                    nonSteamApps.Add(details);
                    Debug.WriteLine($"Added NonSteamApp: {details.Name}, Path: {details.ExePath}"); // Debug print
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error reading from temp file: " + ex.Message);
            }

            return nonSteamApps;
        }

        public class NonSteamAppDetails
        {
            public string Name { get; set; }
            public string ExePath { get; set; }
        }
    }
}
