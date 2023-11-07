using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics; // Ensure this is included for Debug.WriteLine

namespace OVR_Dash_Manager.Software
{
    internal class SteamGameManager
    {
        public class SteamGameDetails
        {
            public string Name { get; set; }
            public string ID { get; set; }
            public string Path { get; set; }
            // Company and Version are not typically available in the manifest files
            // If needed, they would have to be fetched from another source
        }

        public List<SteamGameDetails> GetInstalledGames()
        {
            var gamesList = new List<SteamGameDetails>();
            string steamMainFolder = @"C:\Program Files (x86)\Steam"; // Default path, adjust if necessary
            string libraryFoldersFile = Path.Combine(steamMainFolder, @"steamapps\libraryfolders.vdf");
            var libraryFolders = new List<string> { steamMainFolder };

            // Read the library folders file to get all Steam library paths
            if (File.Exists(libraryFoldersFile))
            {
                var libraryFoldersContent = File.ReadAllText(libraryFoldersFile);
                var matches = Regex.Matches(libraryFoldersContent, "\"path\"\\s*\"([^\"]+)\"");
                foreach (Match match in matches)
                {
                    libraryFolders.Add(match.Groups[1].Value.Replace(@"\\", @"\"));
                }
            }

            // Search each library folder for installed games
            foreach (var libraryFolder in libraryFolders)
            {
                string steamAppsFolder = Path.Combine(libraryFolder, "steamapps");
                if (Directory.Exists(steamAppsFolder))
                {
                    foreach (var filePath in Directory.GetFiles(steamAppsFolder, "appmanifest_*.acf"))
                    {
                        var gameDetails = new SteamGameDetails();
                        var fileContent = File.ReadAllText(filePath);
                        gameDetails.ID = Regex.Match(fileContent, "\"appid\"\\s*\"(\\d+)\"").Groups[1].Value;
                        gameDetails.Name = Regex.Match(fileContent, "\"name\"\\s*\"([^\"]+)\"").Groups[1].Value;
                        gameDetails.Path = Regex.Match(fileContent, "\"installdir\"\\s*\"([^\"]+)\"").Groups[1].Value;

                        // Construct the full path to the installed game directory
                        gameDetails.Path = Path.Combine(steamAppsFolder, "common", gameDetails.Path);

                        gamesList.Add(gameDetails);
                    }
                }
            }
            return gamesList;
        }
    }
}
