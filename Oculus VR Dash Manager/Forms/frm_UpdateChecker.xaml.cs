using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Text.RegularExpressions;
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

        private async void btnDownloadLatest_Click(object sender, RoutedEventArgs e)
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
            await Check_DashManager_Update(btnDownloadLatest);
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

            btnDownloadLatest.IsEnabled = false; // Disable the download button by default

            await CheckDashManagerUpdates();
            await CheckUpdates(); // This is the method for the other program
        }

        private async Task CheckDashManagerUpdates()
        {
            await Check_DashManager_Update(btnDownloadLatest);
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
            Dashes.OVR_Dash ItsKaitlyn03 = Dashes.Dash_Manager.GetDash(Dashes.Dash_Type.OculusKiller);

            if (ItsKaitlyn03 == null)
                Functions_Old.DoAction(this, new Action(delegate () { lbl_CurrentVersion.Content = "Not Loaded"; }));
            else if (!ItsKaitlyn03.Installed)
                Functions_Old.DoAction(this, new Action(delegate () { lbl_CurrentVersion.Content = "Not Downloaded"; }));
            else
            {
                FileVersionInfo Info = FileVersionInfo.GetVersionInfo(Path.Combine(Software.Oculus.Oculus_Dash_Directory, ItsKaitlyn03.DashFileName));
                Functions_Old.DoAction(this, new Action(delegate () { lbl_CurrentVersion.Content = Info.FileVersion; }));
            }

            Github Check = new Github();
            GitHub = await Check.GetLatestReleaseInfoAsync("DevOculus-Meta-Quest", "OculusKiller");
            Functions_Old.DoAction(this, new Action(delegate () { lbl_LastCheck.Content = DateTime.Now.ToString(); lbl_AvaliableVersion.Content = GitHub.ReleaseVersion; }));

            if (GitHub.AssetUrls.ContainsKey("OculusDash.exe"))
                Functions_Old.DoAction(this, new Action(delegate () { btn_Download.IsEnabled = true; }));
        }

        private async void btn_Download_Click(object sender, RoutedEventArgs e)
        {
            Dashes.OVR_Dash ItsKaitlyn03 = Dashes.Dash_Manager.GetDash(Dashes.Dash_Type.OculusKiller);
            await ItsKaitlyn03.DownloadAsync();  // If Download is async, use await here
            await Check_Update();
        }
    }
}