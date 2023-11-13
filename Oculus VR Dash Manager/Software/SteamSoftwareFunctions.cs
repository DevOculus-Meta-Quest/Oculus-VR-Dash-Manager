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
                    var vdfData = parser.ParseVdf(vdfFilePath);

                    foreach (var entry in vdfData)
                    {
                        if (entry.Value is Dictionary<string, object> appDetails)
                        {
                            SteamAppDetails details = new SteamAppDetails();
                            details.Name = appDetails.ContainsKey("AppName") ? appDetails["AppName"].ToString() : "Unknown";
                            details.ExePath = appDetails.ContainsKey("Exe") ? appDetails["Exe"].ToString() : "Unknown";
                            // Add other properties as needed

                            nonSteamApps.Add(details);
                        }
                    }
                }
            }

            return nonSteamApps;
        }

        public class SteamAppDetails
        {
            public string Name { get; set; }
            public string ExePath { get; set; }
            // Add other properties as needed
        }
    }
}
