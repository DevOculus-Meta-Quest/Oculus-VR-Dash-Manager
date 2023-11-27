using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace OVR_Dash_Manager.Functions.Steam
{
    internal class SteamGameManager
    {
        public class SteamGameDetails
        {
            public string Name { get; set; }
            public string ID { get; set; }
            public string Path { get; set; }
            public string ImagePath { get; set; } // Property for the image path
        }

        public List<SteamGameDetails> GetInstalledGames()
        {
            var gamesList = new List<SteamGameDetails>();
            var steamMainFolder = @"C:\Program Files (x86)\Steam"; // Default path, adjust if necessary
            var libraryFoldersFile = Path.Combine(steamMainFolder, @"steamapps\libraryfolders.vdf");
            var libraryFolders = new List<string> { steamMainFolder };

            // Read the library folders file to get all Steam library paths
            if (File.Exists(libraryFoldersFile))
            {
                var libraryFoldersContent = File.ReadAllText(libraryFoldersFile);
                var matches = Regex.Matches(libraryFoldersContent, "\"path\"\\s*\"([^\"]+)\"");

                foreach (Match match in matches)
                    libraryFolders.Add(match.Groups[1].Value.Replace(@"\\", @"\"));
            }

            // Search each library folder for installed games
            foreach (var libraryFolder in libraryFolders)
            {
                var steamAppsFolder = Path.Combine(libraryFolder, "steamapps");

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

                        // Add the game details to the list
                        gamesList.Add(gameDetails);
                    }
                }
            }

            // Get the image path for each game
            foreach (var game in gamesList)
                game.ImagePath = GetImagePathForGame(game.ID);

            return gamesList;
        }

        private string GetImagePathForGame(string gameId)
        {
            var imageCacheDirectory = @"C:\Program Files (x86)\Steam\appcache\librarycache"; // Set to your image cache directory
            var searchPattern = gameId + "_library_600x900.*"; // Updated to match the format
            var imageFiles = Directory.GetFiles(imageCacheDirectory, searchPattern);

            if (imageFiles.Length > 0)
            {
                // Return the first found image path
                return imageFiles[0];
            }
            else
            {
                // If no image is found, you can return a default image path or null
                return null;
            }
        }
    }
}