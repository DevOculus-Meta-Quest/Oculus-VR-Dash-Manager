using OVR_Dash_Manager.Software; // This line is crucial
using System.Collections.Generic;
using System.Windows;

namespace OVR_Dash_Manager.Forms
{
    /// <summary>
    /// Interaction logic for frm_SteamApps.xaml
    /// </summary>
    public partial class frm_SteamApps : Window
    {
        public frm_SteamApps()
        {
            InitializeComponent();
            LoadSteamApps();
        }

        private void LoadSteamApps()
        {
            // Instantiate the SteamGameManager class
            var steamGameManager = new SteamGameManager();

            // Retrieve the list of installed Steam games
            List<SteamGameManager.SteamGameDetails> installedGames = steamGameManager.GetInstalledGames();

            // Add the game details to the ListView
            foreach (var game in installedGames)
            {
                listViewSteamApps.Items.Add(new
                {
                    Name = game.Name, // Ensure this matches the binding in XAML
                    ID = game.ID, // Ensure this matches the binding in XAML
                    Path = game.Path, // Ensure this matches the binding in XAML
                    ImagePath = game.ImagePath ?? "PathToDefaultImage" // Provide a default image path if null
                });
            }
        }
    }
}
