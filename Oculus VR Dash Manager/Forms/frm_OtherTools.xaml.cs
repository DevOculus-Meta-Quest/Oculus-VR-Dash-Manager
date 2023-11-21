using Microsoft.Win32;
using OVR_Dash_Manager.Forms.Dash_Customizer;
using OVR_Dash_Manager.Forms.Profile_Manager;
using OVR_Dash_Manager.Functions;
using OVR_Dash_Manager.Functions.Android;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;

namespace OVR_Dash_Manager.Forms
{
    /// <summary>
    /// Interaction logic for frm_OtherTools.xaml
    /// </summary>
    public partial class frm_OtherTools : Window
    {
        // Path to the log file
        private static string logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "OculusKiller", "OculusKiller.log");

        public frm_OtherTools()
        {
            InitializeComponent();
            CheckLogFile(); // Check if the log file exists when the form loads
            UpdateDeleteLogButtonStatus();
        }

        private void btn_InstallAPK_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "APK files (*.apk)|*.apk",
                Title = "Select an APK file"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                ADB.InstallAPK(openFileDialog.FileName);
            }
        }

        private void btn_ADBFileManager_Click(object sender, RoutedEventArgs e)
        {
            var adbFileManagerWindow = new ADBFileManagerWindow();
            adbFileManagerWindow.ShowDialog();
        }

        private void btn_PNG2DDS_Click(object sender, RoutedEventArgs e)
        {
            var dashCustomizer = new frm_DashCustomizer();
            dashCustomizer.WindowStartupLocation = WindowStartupLocation.CenterScreen; // Center the window
            dashCustomizer.Topmost = true; // Make the window appear on top of other windows
            dashCustomizer.ShowDialog(); // Open the window as a modal dialog box
        }

        private void btn_ProfileManager1_Click(object sender, RoutedEventArgs e)
        {
            // Create an instance of the frm_ProfileManager window and show it
            var profileManager = new frm_ProfileManager();
            profileManager.Owner = this; // 'this' refers to the main window
            profileManager.ShowDialog();
        }

        // Check if the log file exists and enable or disable the button accordingly
        private void CheckLogFile() => btn_OpenLog.IsEnabled = File.Exists(logPath);

        // Handle the button click event to open the log file
        private void btn_OpenLog_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(logPath))
            {
                // Open the log file with the default text editor
                Process.Start(new ProcessStartInfo(logPath) { UseShellExecute = true });
            }
            else
            {
                MessageBox.Show("Log file not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btn_DeleteLog_Click(object sender, RoutedEventArgs e)
        {
            var logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "OculusKiller", "OculusKiller.log");

            if (File.Exists(logPath))
            {
                try
                {
                    File.Delete(logPath);
                    MessageBox.Show("Logfile deleted successfully.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while deleting the logfile: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Logfile does not exist.");
            }

            // Update the button status
            UpdateDeleteLogButtonStatus();
        }

        private void UpdateDeleteLogButtonStatus()
        {
            var logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "OculusKiller", "OculusKiller.log");
            btn_DeleteLog.IsEnabled = File.Exists(logPath);
        }

        private void btn_OculusView_Click(object sender, RoutedEventArgs e)
        {
            // Create a new instance of the OculusView window
            var oculusView = new OculusView();

            // Optional: Start monitoring the Oculus controller when the window is loaded
            oculusView.Loaded += (s, e) => oculusView.StartMonitoringController();

            // Optional: Stop monitoring the Oculus controller when the window is closed
            oculusView.Closed += (s, e) => oculusView.StopMonitoringController();

            // Show the OculusView window
            oculusView.Show();
        }

        private void btn_TestSteam_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var filePath = @"C:\Program Files (x86)\Steam\userdata\201667287\config\shortcuts.vdf";
                var parser = new VdfParser();
                var vdfData = parser.ParseVdf(filePath);

                // Process or display the parsed data
                foreach (var entry in vdfData)
                    Debug.WriteLine($"{entry.Key}: {FormatValue(entry.Value)}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex.Message);
            }
        }

        private string FormatValue(object value)
        {
            if (value is Dictionary<string, object> nestedDict)
            {
                return FormatDictionary(nestedDict);
            }
            else
            {
                return value.ToString();
            }
        }

        private string FormatDictionary(Dictionary<string, object> dict)
        {
            var sb = new StringBuilder();

            foreach (var entry in dict)
                sb.AppendLine($"{entry.Key}: {FormatValue(entry.Value)}");

            return sb.ToString();
        }
    }
}