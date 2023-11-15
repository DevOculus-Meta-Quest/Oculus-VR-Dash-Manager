using OVR_Dash_Manager.Functions;
using System;
using System.Windows; // Make sure you have the correct using directives

namespace OVR_Dash_Manager.Forms
{
    /// <summary>
    /// Interaction logic for frm_SteamApps.xaml
    /// </summary>
    public partial class frm_OculusApps : Window
    {
        public frm_OculusApps()
        {
            InitializeComponent();
            LoadOculusApps();
        }

        private void LoadOculusApps()
        {
            try
            {
                // Assuming GetOculusAppDetails is a static method in the OculusAppChecker class
                var oculusApps = OculusAppChecker.GetOculusAppDetails();

                // Set the ListView's ItemsSource to the list of Oculus apps
                listViewOculusApps.ItemsSource = oculusApps;
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