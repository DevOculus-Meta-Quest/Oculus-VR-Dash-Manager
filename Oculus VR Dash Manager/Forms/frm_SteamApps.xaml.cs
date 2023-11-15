using OVR_Dash_Manager.Functions;
using OVR_Dash_Manager.Software;
using System;
using System.Threading.Tasks;
using System.Windows; // Ensure correct using directives

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

        private async void LoadSteamApps()
        {
            try
            {
                await Task.Run(() =>
                {
                    var steamApps = SteamAppChecker.GetSteamAppDetails(); // Assuming this method exists
                    var nonSteamApps = SteamSoftwareFunctions.GetNonSteamAppDetails();

                    Dispatcher.Invoke(() =>
                    {
                        listViewSteamApps.ItemsSource = steamApps;
                        listViewNonSteamApps.ItemsSource = nonSteamApps;
                    });
                });
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex, "Error loading Steam apps");
            }
        }
    }
}