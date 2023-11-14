using OVR_Dash_Manager.Functions;
using System.Windows; // Make sure you have the correct using directives
using OVR_Dash_Manager.Software; // This line is crucial
using System.Collections.Generic;
using System;
using System.Linq;

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
            try
            {
                var steamApps = SteamAppChecker.GetSteamAppDetails(); // List of SteamAppDetails
                var nonSteamApps = SteamSoftwareFunctions.GetNonSteamAppDetails(); // List of NonSteamAppDetails

                // You can now use steamApps and nonSteamApps separately or combine them
                // For example, setting them to a ListView's ItemsSource
                listViewSteamApps.ItemsSource = steamApps;
                listViewNonSteamApps.ItemsSource = nonSteamApps;
            }
            catch (Exception ex)
            {
                // Error handling
            }
        }
    }
}
