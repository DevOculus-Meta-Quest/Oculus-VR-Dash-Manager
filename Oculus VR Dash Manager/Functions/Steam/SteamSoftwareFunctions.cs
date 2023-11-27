using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace OVR_Dash_Manager.Functions.Steam
{
    public static class SteamSoftwareFunctions
    {
        public static List<NonSteamAppDetails> GetNonSteamAppDetails()
        {
            var nonSteamApps = new List<NonSteamAppDetails>();
            var steamUserDataPath = @"C:\Program Files (x86)\Steam\userdata";
            var oculusStoreAssetsPath = @"C:\Program Files\Oculus\CoreData\Software\StoreAssets";

            Debug.WriteLine($"Searching for VDF files in {steamUserDataPath}");

            if (Directory.Exists(steamUserDataPath))
            {
                foreach (var userDirectory in Directory.GetDirectories(steamUserDataPath))
                {
                    var vdfFilePath = Path.Combine(userDirectory, @"config\shortcuts.vdf");
                    var tempFilePath = Path.GetTempFileName();

                    Debug.WriteLine($"Checking if VDF file exists at {vdfFilePath}");

                    if (File.Exists(vdfFilePath))
                    {
                        WriteParsedDataToTempFile(vdfFilePath, tempFilePath);
                        var apps = ReadDataFromTempFile(tempFilePath);
                        foreach (var app in apps)
                        {
                            app.ImagePath = FindImagePath(oculusStoreAssetsPath, app.Name);
                            Debug.WriteLine($"Image path for {app.Name}: {app.ImagePath}");
                        }
                        nonSteamApps.AddRange(apps);
                    }
                    else
                    {
                        ErrorLogger.LogError(new FileNotFoundException(), $"VDF file not found at {vdfFilePath}");
                    }
                }
            }
            else
            {
                ErrorLogger.LogError(new DirectoryNotFoundException(), $"Steam userdata directory not found at {steamUserDataPath}");
            }

            return nonSteamApps;
        }

        private static void WriteParsedDataToTempFile(string vdfFilePath, string tempFilePath)
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

        private static List<NonSteamAppDetails> ReadDataFromTempFile(string tempFilePath)
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

        private static string FindImagePath(string basePath, string appName)
        {
            var searchName = appName.Replace(" ", "");
            var directories = Directory.GetDirectories(basePath, $"*{searchName}*", SearchOption.AllDirectories);

            foreach (var dir in directories)
            {
                var imagePath = Path.Combine(dir, "cover_square_image.jpg");
                if (File.Exists(imagePath))
                {
                    Debug.WriteLine($"Found image for {appName} at {imagePath}");
                    return imagePath;
                }
            }

            Debug.WriteLine($"Image not found for {appName}");
            return "Image Not Found";
        }

        public class NonSteamAppDetails
        {
            public string Name { get; set; }
            public string ExePath { get; set; }
            public string ImagePath { get; set; }
        }
    }
}