using OVR_Dash_Manager.Functions.Oculus;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace OVR_Dash_Manager.Functions.Dashes
{
    public class OVR_Dash
    {
        public OVR_Dash(string DisplayName, string DashFileName, string ProductName = "", string RepoName = "", string ProjectName = "", string AssetName = "", string ProcessToStop = "")
        {
            this.DisplayName = DisplayName;
            this.DashFileName = DashFileName;
            this.ProductName = ProductName;
            this.RepoName = RepoName;
            this.ProjectName = ProjectName;
            this.AssetName = AssetName;
            this.ProcessToStop = ProcessToStop;
        }

        public string DisplayName { get; } = "Dash";
        public string DashFileName { get; set; } = "Dash.exe";
        public string ProductName { get; set; } = string.Empty;
        public string RepoName { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public string AssetName { get; set; } = string.Empty;
        public string ProcessToStop { get; set; } = string.Empty;

        private long _Size = -1;
        private bool _DashActive;
        private bool _NeedUpdate;
        private bool _Installed;

        public bool DashActive
        {
            get { return _DashActive; }
            private set { _DashActive = value; }
        }

        public bool NeedUpdate
        {
            get { return _NeedUpdate; }
            private set { _NeedUpdate = value; }
        }

        public bool Installed
        {
            get { return _Installed; }
            private set { _Installed = value; }
        }

        private string _CurrentVersion;

        public string CurrentVersion
        {
            get { return _CurrentVersion; }
            private set { _CurrentVersion = value; }
        }

        private string _AvaliableVersion;

        public string AvaliableVersion
        {
            get { return _AvaliableVersion; }
            private set { _AvaliableVersion = value; }
        }

        public bool IsThisYourDash(string Dash_ProductName) => Dash_ProductName == ProductName;

        public void CheckInstalled()
        {
            if (Directory.Exists(OculusRunning.Oculus_Dash_Directory))
            {
                var DashPath = Path.Combine(OculusRunning.Oculus_Dash_Directory, DashFileName);

                if (File.Exists(DashPath))
                    _Installed = true;
            }
        }

        public async Task CheckUpdateAsync()
        {
            if (_Installed)
            {
                if (!string.IsNullOrEmpty(RepoName) && !string.IsNullOrEmpty(ProjectName) && !string.IsNullOrEmpty(AssetName))
                {
                    var DashPath = Path.Combine(OculusRunning.Oculus_Dash_Directory, DashFileName);
                    var CurrentDash = new FileInfo(DashPath);
                    _Size = CurrentDash.Length;

                    var github = new Github();

                    var size = await github.GetLatestSizeAsync(RepoName, ProjectName, AssetName);

                    if (size != _Size)
                        _NeedUpdate = true;
                }
            }
        }

        public async Task DownloadAsync()
        {
            var Temp_DashPath = Path.Combine(OculusRunning.Oculus_Dash_Directory, DashFileName + ".tmp");
            var DashPath = Path.Combine(OculusRunning.Oculus_Dash_Directory, DashFileName);
            var Active_NeedUpdateTo = false;
            var ShouldUpdate = true;

            if (_DashActive)
            {
                try
                {
                    // try and remove live version of this dash (we always have our backup in case)
                    File.Delete(OculusRunning.Oculus_Dash_File);
                    Active_NeedUpdateTo = true;
                }
                catch (Exception)
                {
                    ShouldUpdate = false;
                }
            }

            if (ShouldUpdate)
            {
                // Show message box before starting the download
                MessageBox.Show($"It appears this is your first time running this application, or the 'Oculus_Killer.exe' file is missing. To ensure smooth operation, 'OculusDash.exe' will be downloaded and saved as 'Oculus_Killer.exe' in {OculusRunning.Oculus_Dash_Directory}. This process is a one-time setup and should not recur once completed. The application will be ready for use immediately after this initial setup.", "Initial Setup Notification");

                var Repo = new Github();

                try
                {
                    // Assuming RepoName, ProjectName, AssetName are class-level variables, and Temp_DashPath is the temporary file path
                    await Repo.DownloadAsync(RepoName, ProjectName, AssetName, Temp_DashPath);
                }
                catch (Exception)
                {
                    // Handle exception (e.g., log the error)
                }

                if (File.Exists(Temp_DashPath))
                {
                    try
                    {
                        File.Move(Temp_DashPath, DashPath);
                        _Installed = true;

                        if (Active_NeedUpdateTo)
                            File.Copy(DashPath, OculusRunning.Oculus_Dash_File, true);
                    }
                    catch (Exception ex)
                    {
                        // try and update unless its already running, but just checked that .. hmm
                        Debug.WriteLine(ex.Message);
                    }
                }
            }

            if (_DashActive)
            {
                if (!File.Exists(OculusRunning.Oculus_Dash_File))
                    File.Copy(DashPath, OculusRunning.Oculus_Dash_File, true); // Copy backup file back in case something failed above
            }
        }

        public bool Activate_Dash()
        {
            var Activated = false;

            if (Installed)
            {
                try
                {
                    if (!string.IsNullOrEmpty(ProcessToStop))
                    {
                        Debug.WriteLine("Attempting to kill: " + ProcessToStop);
                        CloseProcessBeforeIfRequired();
                    }
                }
                catch (Exception)
                {
                    Debug.WriteLine("Unable to kill required process: " + ProcessToStop);
                    throw;
                }

                var DashPath = Path.Combine(OculusRunning.Oculus_Dash_Directory, DashFileName);

                try
                {
                    Activated = SwitchFiles(DashPath, OculusRunning.Oculus_Dash_File);
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("because it is being used by another process"))
                        Activated = false;
                    else
                        throw;
                }
            }

            return Activated;
        }

        public bool Activate_Dash_Fast()
        {
            var Activated = false;

            if (Installed)
            {
                var Dashes = Process.GetProcessesByName("OculusDash");

                if (Dashes.Length > 0)
                {
                    var RunningDash = Dashes[0];

                    if (!RunningDash.HasExited)
                    {
                        Debug.WriteLine("Killing: " + RunningDash.Id);
                        RunningDash.Kill();
                    }
                }

                var DashPath = Path.Combine(OculusRunning.Oculus_Dash_Directory, DashFileName);

                try
                {
                    Activated = SwitchFiles(DashPath, OculusRunning.Oculus_Dash_File);
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("because it is being used by another process"))
                        Activated = false;
                    else
                        throw;
                }

                try
                {
                    if (!string.IsNullOrEmpty(ProcessToStop))
                    {
                        Debug.WriteLine("Attempting to kill: " + ProcessToStop);
                        CloseProcessBeforeIfRequired();
                    }
                }
                catch (Exception)
                {
                    Debug.WriteLine("Unable to kill required process: " + ProcessToStop);
                    throw;
                }
            }

            return Activated;
        }

        public bool Activate_Dash_Fast_v2()
        {
            var Activated = false;

            if (Installed)
            {
                var DashPath = Path.Combine(OculusRunning.Oculus_Dash_Directory, DashFileName);

                try
                {
                    if (File.Exists($"{OculusRunning.Oculus_Dash_File}.delete"))
                    {
                        File.Delete($"{OculusRunning.Oculus_Dash_File}.delete");
                        Debug.WriteLine("Removed Old Dash File");
                    }

                    if (File.Exists(OculusRunning.Oculus_Dash_File))
                    {
                        Debug.WriteLine("Moving Active Dash");

                        File.Move(OculusRunning.Oculus_Dash_File, $"{OculusRunning.Oculus_Dash_File}.delete");
                    }

                    Debug.WriteLine("Switching in New Dash");
                    Activated = SwitchFiles(DashPath, OculusRunning.Oculus_Dash_File);
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("because it is being used by another process"))
                        Activated = false;
                    else
                        throw;
                }

                var Dashes = Process.GetProcessesByName("OculusDash");

                if (Dashes.Length > 0)
                {
                    var RunningDash = Dashes[0];

                    if (!RunningDash.HasExited)
                    {
                        Debug.WriteLine("Killing Dash: " + RunningDash.Id);
                        RunningDash.Kill();

                        if (File.Exists($"{OculusRunning.Oculus_Dash_File}.delete"))
                        {
                            RunningDash.WaitForExit();
                            File.Delete($"{OculusRunning.Oculus_Dash_File}.delete");
                            Debug.WriteLine("Removed Old Dash File");
                        }
                    }
                }

                try
                {
                    if (File.Exists($"{OculusRunning.Oculus_Dash_File}.delete"))
                    {
                        File.Delete($"{OculusRunning.Oculus_Dash_File}.delete");
                        Debug.WriteLine("Removed Old Dash File");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Failed removing old file: " + ex.Message);
                }

                try
                {
                    if (!string.IsNullOrEmpty(ProcessToStop))
                    {
                        Debug.WriteLine("Attempting to kill: " + ProcessToStop);
                        CloseProcessBeforeIfRequired();
                    }
                }
                catch (Exception)
                {
                    Debug.WriteLine("Unable to kill required process: " + ProcessToStop);
                    throw;
                }
            }

            return Activated;
        }

        private bool SwitchFiles(string NewFile, string OldFile)
        {
            var Activated = false;

            if (File.Exists(NewFile))
            {
                File.Copy(NewFile, OldFile, true);

                var Source = new FileInfo(NewFile);
                var Target = new FileInfo(OldFile);

                if (Source.Length == Target.Length)
                    Activated = true;
            }

            return Activated;
        }

        private void CloseProcessBeforeIfRequired()
        {
            if (!string.IsNullOrEmpty(ProcessToStop))
            {
                var SteamVR = Process.GetProcessesByName(ProcessToStop);

                foreach (Process VR in SteamVR)
                    VR.CloseMainWindow();
            }
        }
    }
}