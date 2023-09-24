using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using OVR_Dash_Manager;  // Ensure this namespace is correct

namespace OVR_Dash_Manager.Forms
{
    /// <summary>
    /// Interaction logic for frm_UpdateChecker.xaml
    /// </summary>
    public partial class frm_UpdateChecker : Window
    {
        private GitHubReply ItsKaitlyn03_GitHub;

        public frm_UpdateChecker()
        {
            InitializeComponent();
        }

        private void btn_DashManager_OpenWebsite_Click(object sender, RoutedEventArgs e)
        {
            Functions_Old.OpenURL("https://github.com/KrisIsBackAU/Oculus-VR-Dash-Manager");
        }

        private void btn_ItsKaitlyn03_OpenWebsite_Click(object sender, RoutedEventArgs e)
        {
            Functions_Old.OpenURL("https://github.com/ItsKaitlyn03/OculusKiller");
        }

        private async Task CheckUpdates()
        {
            await Check_DashManager_Update();
            await Check_ItsKaitlyn03_Update();
        }

        private async Task Check_DashManager_Update()
        {
            Github Check = new Github();
            String Version = await Check.GetLatestReleaseVersion("KrisIsBackAU", "Oculus-VR-Dash-Manager");
            String CurrentVersion = typeof(MainWindow).Assembly.GetName().Version.ToString();
            Functions_Old.DoAction(this, new Action(delegate () { lbl_DashManager_LastCheck.Content = DateTime.Now.ToString(); lbl_DashManager_CurrentVersion.Content = CurrentVersion; lbl_DashManager_AvaliableVersion.Content = Version; }));
        }

        private async Task Check_ItsKaitlyn03_Update()
        {
            Dashes.OVR_Dash ItsKaitlyn03 = Dashes.Dash_Manager.GetDash(Dashes.Dash_Type.OculusKiller);

            if (ItsKaitlyn03 == null)
                Functions_Old.DoAction(this, new Action(delegate () { lbl_ItsKaitlyn03_CurrentVersion.Content = "Not Loaded"; }));
            else if (!ItsKaitlyn03.Installed)
                Functions_Old.DoAction(this, new Action(delegate () { lbl_ItsKaitlyn03_CurrentVersion.Content = "Not Downloaded"; }));
            else
            {
                FileVersionInfo Info = FileVersionInfo.GetVersionInfo(Path.Combine(Software.Oculus.Oculus_Dash_Directory, ItsKaitlyn03.DashFileName));
                Functions_Old.DoAction(this, new Action(delegate () { lbl_ItsKaitlyn03_CurrentVersion.Content = Info.FileVersion; }));
            }

            Github Check = new Github();
            ItsKaitlyn03_GitHub = await Check.GetLatestReleaseDetails("ItsKaitlyn03", "OculusKiller");
            Functions_Old.DoAction(this, new Action(delegate () { lbl_ItsKaitlyn03_LastCheck.Content = DateTime.Now.ToString(); lbl_ItsKaitlyn03_AvaliableVersion.Content = ItsKaitlyn03_GitHub.ReleaseVersion; }));

            if (ItsKaitlyn03_GitHub.AssetUrls.ContainsKey("OculusDash.exe"))
                Functions_Old.DoAction(this, new Action(delegate () { btn_ItsKaitlyn03_Download.IsEnabled = true; }));
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lbl_DashManager_LastCheck.Content = "Checking";
            lbl_ItsKaitlyn03_LastCheck.Content = "Checking";

            lbl_DashManager_CurrentVersion.Content = "";
            lbl_DashManager_AvaliableVersion.Content = "";

            lbl_ItsKaitlyn03_CurrentVersion.Content = "";
            lbl_ItsKaitlyn03_AvaliableVersion.Content = "";

            await CheckUpdates();
        }

        private async void btn_ItsKaitlyn03_Download_Click(object sender, RoutedEventArgs e)
        {
            Dashes.OVR_Dash ItsKaitlyn03 = Dashes.Dash_Manager.GetDash(Dashes.Dash_Type.OculusKiller);
            await ItsKaitlyn03.DownloadAsync();  // If Download is async, use await here
            await Check_ItsKaitlyn03_Update();
        }

    }
}
