using OVR_Dash_Manager.Functions;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OVR_Dash_Manager.Software
{
    public static class SteamSoftwareFunctions
    {
        public static List<SteamAppDetails> GetNonSteamAppDetails()
        {
            List<SteamAppDetails> nonSteamApps = new List<SteamAppDetails>();
            string steamUserDataPath = @"C:\Program Files (x86)\Steam\userdata";
            var userDirectories = Directory.GetDirectories(steamUserDataPath);

            foreach (var userDir in userDirectories)
            {
                string vdfFilePath = Path.Combine(userDir, @"config\shortcuts.vdf");
                if (File.Exists(vdfFilePath))
                {
                    VdfParser parser = new VdfParser();
                    // Assuming ParseVdf returns Dictionary<string, object>
                    Dictionary<string, object> vdfData = parser.ParseVdf(vdfFilePath);

                    foreach (var entry in vdfData)
                    {
                        var appDetails = entry.Value as Dictionary<string, object>; // Cast to the correct type

                        var appName = appDetails["AppName"] as string;
                        var exePath = appDetails["Exe"] as string;
                        // Extract other properties as needed

                        nonSteamApps.Add(new SteamAppDetails
                        {
                            Name = appName,
                            InstallPath = exePath,
                            // Other properties as needed
                        });
                    }
                }
            }

            return nonSteamApps;
        }

    }
}
