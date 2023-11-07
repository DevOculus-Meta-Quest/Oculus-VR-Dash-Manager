using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

public class SteamGame
{
    public string Title { get; set; }
    public string InstallPath { get; set; }
    public string ImagePath { get; set; }
    public string Version { get; set; }
    // Add other properties as needed
}

public class SteamGameRetriever
{
    private readonly List<string> _steamLibraryFolders = new List<string>();

    public SteamGameRetriever(string steamDirectory)
    {
        _steamLibraryFolders.Add(Path.Combine(steamDirectory, "steamapps")); // Default library
        LoadAdditionalLibraryFolders(steamDirectory);
    }

    private void LoadAdditionalLibraryFolders(string steamDirectory)
    {
        var libraryFoldersPath = Path.Combine(steamDirectory, "steamapps", "libraryfolders.vdf");
        if (File.Exists(libraryFoldersPath))
        {
            var libraryFoldersText = File.ReadAllText(libraryFoldersPath);
            var regex = new Regex("\"path\"\\s*\"(.*?)\"");
            var matches = regex.Matches(libraryFoldersText);

            foreach (Match match in matches)
            {
                var libraryPath = match.Groups[1].Value.Replace("\\\\", "\\"); // Unescape backslashes
                _steamLibraryFolders.Add(Path.Combine(libraryPath, "steamapps"));
            }
        }
    }

    public List<SteamGame> GetInstalledSteamGames()
    {
        var games = new List<SteamGame>();

        foreach (var libraryFolder in _steamLibraryFolders)
        {
            var acfFiles = Directory.GetFiles(libraryFolder, "*.acf");

            foreach (var file in acfFiles)
            {
                var keyValuePairs = ReadAcfFile(file);
                var game = new SteamGame
                {
                    Title = keyValuePairs.GetValueOrDefault("name"),
                    InstallPath = Path.Combine(libraryFolder, "common", keyValuePairs.GetValueOrDefault("installdir")),
                    Version = keyValuePairs.GetValueOrDefault("buildid"),
                    // ImagePath needs to be determined, possibly from another source as ACF files don't contain image paths
                };

                // Add additional logic here to determine if the game is a VR game
                // For now, we add all games
                games.Add(game);
            }
        }

        return games;
    }

    private Dictionary<string, string> ReadAcfFile(string filePath)
    {
        var keyValuePairs = new Dictionary<string, string>();
        var acfText = File.ReadAllText(filePath);
        var regex = new Regex("\"(.*?)\"\\s*\"(.*?)\"");
        var matches = regex.Matches(acfText);

        foreach (Match match in matches)
        {
            keyValuePairs[match.Groups[1].Value] = match.Groups[2].Value;
        }

        return keyValuePairs;
    }
}
