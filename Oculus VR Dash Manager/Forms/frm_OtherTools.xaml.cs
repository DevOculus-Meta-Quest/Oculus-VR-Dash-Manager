using Microsoft.Win32;
using OVR_Dash_Manager.Forms.Dash_Customizer;
using OVR_Dash_Manager.Forms.Profile_Manager;
using OVR_Dash_Manager.Software;
using System.Diagnostics;
using System.Windows;

namespace OVR_Dash_Manager.Forms
{
    /// <summary>
    /// Interaction logic for frm_OtherTools.xaml
    /// </summary>
    public partial class frm_OtherTools : Window
    {
        public frm_OtherTools()
        {
            InitializeComponent();
        }

        private void btn_InstallAPK_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "APK files (*.apk)|*.apk",
                Title = "Select an APK file"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                ADB.InstallAPK(openFileDialog.FileName);
            }
        }

        private void btn_SteamAppsShow_Click(object sender, RoutedEventArgs e)
        {
            // Create an instance of frm_SteamApps window
            var steamAppsWindow = new frm_SteamApps();

            // Set the frm_OtherTools window as the owner of steamAppsWindow
            steamAppsWindow.Owner = this;

            // Show the window
            steamAppsWindow.Show();
        }

        private void btn_OculusAppsShow_Click(object sender, RoutedEventArgs e)
        {
            // Create an instance of frm_SteamApps window
            var OculusAppsWindow = new frm_OculusApps();

            // Set the frm_OtherTools window as the owner of steamAppsWindow
            OculusAppsWindow.Owner = this;

            // Show the window
            OculusAppsWindow.Show();
        }

        private void btn_ADBFileManager_Click(object sender, RoutedEventArgs e)
        {
            ADBFileManagerWindow adbFileManagerWindow = new ADBFileManagerWindow();
            adbFileManagerWindow.ShowDialog();
        }

        private void btn_ProfileManager_Click(object sender, RoutedEventArgs e)
        {
            // Create an instance of the frm_ProfileManager window and show it
            frm_ProfileManager profileManager = new frm_ProfileManager();
            profileManager.Owner = this; // 'this' refers to the main window
            profileManager.ShowDialog();
        }

        private void btn_PNG2DDS_Click(object sender, RoutedEventArgs e)
        {
            frm_DashCustomizer dashCustomizer = new frm_DashCustomizer();
            dashCustomizer.WindowStartupLocation = WindowStartupLocation.CenterScreen; // Center the window
            dashCustomizer.Topmost = true; // Make the window appear on top of other windows
            dashCustomizer.ShowDialog(); // Open the window as a modal dialog box
        }

    }
}
