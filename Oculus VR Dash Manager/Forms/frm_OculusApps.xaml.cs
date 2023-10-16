using OVR_Dash_Manager.Functions;
using System.Collections.Generic;
using System.Windows;

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
            // Retrieve the cached app names using the public method
            List<string> installedApps = OculusAppChecker.GetInstalledApps();

            // Add the app names to the ListView
            foreach (var app in installedApps)
            {
                listViewOculusApps.Items.Add(app);
            }
        }
    }
}