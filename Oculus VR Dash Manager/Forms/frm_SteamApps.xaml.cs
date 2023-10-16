using OVR_Dash_Manager.Functions;
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
            // Retrieve the cached app names using the public method
            List<string> installedApps = SteamAppChecker.GetInstalledApps();

            // Add the app names to the ListView
            foreach (var app in installedApps)
            {
                listViewSteamApps.Items.Add(app);
            }
        }
    }
}
