using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

// Disable the warning.
#pragma warning disable SYSLIB0014

namespace OVR_Dash_Manager.Forms
{
    /// <summary>
    /// Interaction logic for frm_UpdateChecker.xaml
    /// </summary>
    public partial class frm_UpdateChecker : Window
    {
        private GitHubReply GitHub;

        public frm_UpdateChecker()
        {
            InitializeComponent();
        }

        public async Task<bool> CheckDashManagerUpdates()
        {
            await Check_DashManager_Update(btnDownloadLatestDashManager);
            return btnDownloadLatestDashManager.IsEnabled; // Assuming the button is enabled only when an update is available
        }

        public async Task<bool> CheckOculusKillerUpdates()
        {
            await Check_Update(); // This is the method that checks updates for OculusKiller
            return btn_DownloadLatestOculusKiller.IsEnabled; // Assuming the button is enabled only when an update is available
        }

        private async void btnDownloadLatestDashManager_Click(object sender, RoutedEventArgs e)
        {
            Github Check = new Github();

            // Now that the method is marked as async, you can use the await keyword
            GitHubReply gitHubReply = await Check.GetLatestReleaseInfoAsync("DevOculus-Meta-Quest", "Oculus-VR-Dash-Manager");
            if (gitHubReply != null && gitHubReply.AssetUrls != null)
            {
                string zipUrl = gitHubReply.AssetUrls.Values.FirstOrDefault(url => url.EndsWith(".zip"));

                if (!string.IsNullOrEmpty(zipUrl))
                {
                    using (WebClient webClient = new WebClient())
                    {
                        // Define where you want to save the downloaded file
                        string savePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "latest_release.zip");

                        webClient.DownloadFileCompleted += (s, eventArgs) =>
                        {
                            MessageBox.Show("Download completed! File saved to: " + savePath);
                        };

                        webClient.DownloadFileAsync(new Uri(zipUrl), savePath);
                    }
                }
                else
                {
                    MessageBox.Show("No zip file found in the latest release.");
                }
            }
            else
            {
                MessageBox.Show("Could not retrieve the latest release information.");
            }
        }

        private void btn_DashManager_OpenWebsite_Click(object sender, RoutedEventArgs e)
        {
            Functions_Old.OpenURL("https://github.com/DevOculus-Meta-Quest/Oculus-VR-Dash-Manager");
        }

        private void btn_OpenWebsite_Click(object sender, RoutedEventArgs e)
        {
            Functions_Old.OpenURL("https://github.com/DevOculus-Meta-Quest/OculusKiller");
        }

        private async Task CheckUpdates()
        {
            await Check_DashManager_Update(btnDownloadLatestDashManager);
            await Check_Update();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lbl_DashManager_LastCheck.Content = "Checking";
            lbl_LastCheck.Content = "Checking";

            lbl_DashManager_CurrentVersion.Content = "";
            lbl_DashManager_AvaliableVersion.Content = "";

            lbl_CurrentVersion.Content = "";
            lbl_AvaliableVersion.Content = "";

            btnDownloadLatestDashManager.IsEnabled = false; // Disable the download button by default

            await CheckDashManagerUpdates();
            await CheckUpdates(); // This is the method for the other program
        }

        private async Task Check_DashManager_Update(Button downloadButton)
        {
            Github Check = new Github();
            String Version = await Check.GetLatestReleaseNameAsync("DevOculus-Meta-Quest", "Oculus-VR-Dash-Manager");

            // Remove non-numeric characters except the dot from the version string
            Version = Regex.Replace(Version, @"[^\d.]", "");

            String CurrentVersion = typeof(MainWindow).Assembly.GetName().Version.ToString();

            Functions_Old.DoAction(this, new Action(delegate ()
            {
                lbl_DashManager_LastCheck.Content = DateTime.Now.ToString();
                lbl_DashManager_CurrentVersion.Content = CurrentVersion;
                lbl_DashManager_AvaliableVersion.Content = Version;

                // Enable the download button if a new version is available
                if (IsNewVersion(Version, CurrentVersion))
                {
                    downloadButton.IsEnabled = true;
                }
            }));
        }

        private bool IsNewVersion(string newVersion, string currentVersion)
        {
            var newVersionParts = newVersion.Split('.');
            var currentVersionParts = currentVersion.Split('.');

            for (int i = 0; i < newVersionParts.Length && i < currentVersionParts.Length; i++)
            {
                int newVersionPart = int.Parse(newVersionParts[i]);
                int currentVersionPart = int.Parse(currentVersionParts[i]);

                if (newVersionPart > currentVersionPart)
                {
                    return true;
                }
                else if (newVersionPart < currentVersionPart)
                {
                    return false;
                }
            }

            return newVersionParts.Length > currentVersionParts.Length;
        }

        private async Task Check_Update()
        {
            Github Check = new Github();
            String Version = await Check.GetLatestReleaseNameAsync("DevOculus-Meta-Quest", "OculusKiller");

            // Remove non-numeric characters except the dot from the version string
            Version = Regex.Replace(Version, @"[^\d.]", "");

            Dashes.OVR_Dash OculusKillerMod = Dashes.Dash_Manager.GetDash(Dashes.Dash_Type.OculusKiller);

            if (OculusKillerMod != null && OculusKillerMod.Installed)
            {
                // Ensure this path correctly points to the OculusKiller executable
                FileVersionInfo Info = FileVersionInfo.GetVersionInfo(Path.Combine(Software.Oculus.Oculus_Dash_Directory, OculusKillerMod.DashFileName));

                // Using FileVersion to get the version information
                Functions_Old.DoAction(this, new Action(delegate () { lbl_CurrentVersion.Content = Info.FileVersion; }));
            }
            else
            {
                Functions_Old.DoAction(this, new Action(delegate () { lbl_CurrentVersion.Content = "Not Available"; }));
            }

            Functions_Old.DoAction(this, new Action(delegate ()
            {
                lbl_LastCheck.Content = DateTime.Now.ToString();
                lbl_AvaliableVersion.Content = Version;

                // Enable the download button if a new version is available
                if (IsNewVersion(Version, lbl_CurrentVersion.Content.ToString()))
                {
                    btn_DownloadLatestOculusKiller.IsEnabled = true;
                }
            }));
        }

        private async void btn_DownloadLatestOculusKiller_Click(object sender, RoutedEventArgs e)
        {
            Github Check = new Github();
            GitHubReply gitHubReply = await Check.GetLatestReleaseInfoAsync("DevOculus-Meta-Quest", "OculusKiller");

            if (gitHubReply != null && gitHubReply.AssetUrls != null)
            {
                string exeUrl = gitHubReply.AssetUrls.Values.FirstOrDefault(url => url.EndsWith(".exe"));

                if (!string.IsNullOrEmpty(exeUrl))
                {
                    MessageBoxResult result = MessageBox.Show("A new version of OculusKiller is available. It will replace the old version. Do you want to proceed?", "Update Available", MessageBoxButton.YesNo);

                    if (result == MessageBoxResult.Yes)
                    {
                        string oldFilePath = @"C:\Program Files\Oculus\Support\oculus-dash\dash\bin\Oculus_Killer.exe";

                        try
                        {
                            // Delete the old file
                            if (File.Exists(oldFilePath))
                            {
                                File.Delete(oldFilePath);
                            }
                        }
                        catch (IOException)
                        {
                            MessageBox.Show("The old version of OculusKiller is currently in use. Please close any application that might be using it and try updating again.");
                            return;
                        }

                        using (WebClient webClient = new WebClient())
                        {
                            webClient.DownloadFileCompleted += async (s, eventArgs) =>
                            {
                                // Move and rename the downloaded file to the specific directory
                                File.Move("OculusKiller_latest.exe", oldFilePath);
                                MessageBox.Show("Update completed! The new version has been installed.");

                                // Refresh the displayed current version
                                await CheckUpdates();
                            };

                            // Download the new version to the current directory
                            webClient.DownloadFileAsync(new Uri(exeUrl), "OculusKiller_latest.exe");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("No executable file found in the latest release.");
                }
            }
            else
            {
                MessageBox.Show("Could not retrieve the latest release information.");
            }
        }
    }
}