using OVR_Dash_Manager.Functions;
using System.Windows; // Make sure you have the correct using directives
using OVR_Dash_Manager.Software; // This line is crucial
using System.Collections.Generic;
using System;

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
                // Assuming GetOculusAppDetails is a static method in the OculusAppChecker class
                var SteamApps = SteamAppChecker.GetSteamAppDetails();

                // Set the ListView's ItemsSource to the list of Oculus apps
                listViewSteamApps.ItemsSource = SteamApps;
            }
            catch (Exception ex)
            {
                // Use your ErrorLogger to log the exception
                ErrorLogger.LogError(ex, "Error loading Oculus apps.");
                MessageBox.Show("Error occurred while loading Oculus apps. Please check the error log for more details.");
            }
        }
    }
}
