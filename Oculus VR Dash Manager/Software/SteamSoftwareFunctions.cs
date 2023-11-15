using Newtonsoft.Json; // Ensure Newtonsoft.Json is installed
using Newtonsoft.Json.Linq;
using OVR_Dash_Manager.Functions;
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
            var nonSteamApps = new List<NonSteamAppDetails>();
            var vdfFilePath = @"C:\Program Files (x86)\Steam\userdata\201667287\config\shortcuts.vdf";
            var tempFilePath = Path.GetTempFileName(); // Generates a temporary file path

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

        static void WriteParsedDataToTempFile(string vdfFilePath, string tempFilePath)
        {
            try
            {
                var parser = new VdfParser();
                var parsedData = parser.ParseVdf(vdfFilePath);

                Debug.WriteLine("Parsed VDF Data: " + JsonConvert.SerializeObject(parsedData)); // Debug print

                var jsonData = JsonConvert.SerializeObject(parsedData);
                File.WriteAllText(tempFilePath, jsonData);

                Debug.WriteLine($"Written JSON data to temp file at {tempFilePath}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error writing to temp file: " + ex.Message);
            }
        }

        static List<NonSteamAppDetails> ReadDataFromTempFile(string tempFilePath)
        {
            var nonSteamApps = new List<NonSteamAppDetails>();

            try
            {
                var jsonData = File.ReadAllText(tempFilePath);
                Debug.WriteLine("Read JSON data from temp file: " + jsonData); // Debug print

                var parsedData = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonData);

                if (parsedData.ContainsKey("shortcuts"))
                {
                    var shortcuts = parsedData["shortcuts"] as JObject; // Cast to JObject

                    foreach (var shortcutEntry in shortcuts)
                    {
                        var shortcutDetails = shortcutEntry.Value.ToObject<Dictionary<string, object>>(); // Convert each shortcut to a dictionary

                        var details = new NonSteamAppDetails
                        {
                            Name = shortcutDetails["AppName"].ToString(),
                            ExePath = shortcutDetails["Exe"].ToString()
                        };

                        nonSteamApps.Add(details);
                        Debug.WriteLine($"Added NonSteamApp: {details.Name}, Path: {details.ExePath}"); // Debug print
                    }
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