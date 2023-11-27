using OVR_Dash_Manager.Functions.Oculus;
using OVR_Dash_Manager.Functions.Steam;
using System;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace OVR_Dash_Manager.Functions.Dashes
{
    public static class Dash_Manager
    {
        private static OVR_Dash Oculus_Dash;
        private static OVR_Dash SteamVR_Dash;
        private static MainWindow MainForm;

        /// <summary>
        /// Passes the main form instance to the Dash_Manager.
        /// </summary>
        /// <param name="Form">Instance of the main form.</param>
        public static void PassMainForm(MainWindow Form) => MainForm = Form;

        /// <summary>
        /// Fixes the task view issue on the main form.
        /// </summary>
        public static void MainForm_FixTaskViewIssue()
        {
            if (MainForm != null)
                UIManager.DoAction(MainForm, new Action(delegate () { MainForm.Cancel_TaskView_And_Focus(); }));
        }

        /// <summary>
        /// Checks the runtime on the main form.
        /// </summary>
        public static void MainForm_CheckRunTime()
        {
            if (MainForm != null)
                UIManager.DoAction(MainForm, new Action(delegate () { MainForm.CheckRunTime(); }));
        }

        /// <summary>
        /// Generates the dashes.
        /// </summary>
        public static async Task GenerateDashesAsync()
        {
            OculusRunning.Check_Current_Dash();

            Oculus_Dash = new OVR_Dash("Official Oculus Dash", "OculusDash_Normal.exe", ProcessToStop: "vrmonitor");
            SteamVR_Dash = new OVR_Dash("DevOculus-Meta-Quest - Oculus Killer", "Oculus_Killer.exe", "Oculus Killer", "DevOculus-Meta-Quest", "OculusKiller", "OculusDash.exe");

            OculusRunning.Setup();
            OculusRunning.Check_Oculus_Is_Installed();

            await CheckInstalled();
        }

        /// <summary>
        /// Checks if the dashes are installed.
        /// </summary>
        private static async Task CheckInstalled()
        {
            Oculus_Dash.CheckInstalled();
            SteamVR_Dash.CheckInstalled();

            if (!SteamVR_Dash.Installed)
                await SteamVR_Dash.DownloadAsync();

            OculusRunning.Check_Current_Dash();

            if (!Oculus_Dash.Installed && OculusRunning.Normal_Dash)
            {
                // Copy Default Oculus Dash if not already done
                File.Copy(OculusRunning.Oculus_Dash_File, Path.Combine(OculusRunning.Oculus_Dash_Directory, Oculus_Dash.DashFileName), true);
                Oculus_Dash.CheckInstalled();
            }
            else if (OculusRunning.Normal_Dash)
            {
                // Check if Oculus Updated and check is Oculus Dash has changed by "Length"
                var CurrentDash = new FileInfo(Path.Combine(OculusRunning.Oculus_Dash_Directory, Oculus_Dash.DashFileName));
                var OculusDashFile = new FileInfo(OculusRunning.Oculus_Dash_File);

                // Update File
                if (CurrentDash.Length != OculusDashFile.Length)
                    File.Copy(OculusRunning.Oculus_Dash_File, Path.Combine(OculusRunning.Oculus_Dash_Directory, Oculus_Dash.DashFileName), true);
            }
        }

        /// <summary>
        /// Checks if a specific dash is installed.
        /// </summary>
        /// <param name="Dash">Type of dash to check.</param>
        /// <returns>True if installed, false otherwise.</returns>
        public static bool IsInstalled(Dash_Type Dash)
        {
            return Dash switch
            {
                Dash_Type.Normal => Oculus_Dash?.Installed ?? false,
                Dash_Type.OculusKiller => SteamVR_Dash?.Installed ?? false,
                _ => false,
            };
        }

        /// <summary>
        /// Sets the active dash.
        /// </summary>
        /// <param name="Dash">Type of dash to set active.</param>
        /// <returns>True if activated, false otherwise.</returns>
        public static bool SetActiveDash(Dash_Type Dash)
        {
            var activated = false;

            switch (Dash)
            {
                case Dash_Type.Normal:
                    SteamRunning.ManagerCalledExit = true;
                    activated = Properties.Settings.Default.FastSwitch ? Oculus_Dash.Activate_Dash_Fast_v2() : Oculus_Dash.Activate_Dash();
                    break;

                case Dash_Type.OculusKiller:
                    activated = Properties.Settings.Default.FastSwitch ? SteamVR_Dash.Activate_Dash_Fast_v2() : SteamVR_Dash.Activate_Dash();
                    break;

                default:
                    break;
            }

            return activated;
        }

        public static Dash_Type CheckWhosDash(string File_ProductName)
        {
            if (SteamVR_Dash.IsThisYourDash(File_ProductName))
                return Dash_Type.OculusKiller;

            if (string.IsNullOrEmpty(File_ProductName))
                return Dash_Type.Normal;

            return Dash_Type.Unknown;
        }

        public static string GetDashName(Dash_Type dash)
        {
            switch (dash)
            {
                case Dash_Type.Unknown:
                    return "Unknown";

                case Dash_Type.Normal:
                    // Check if DisplayName is not null or empty
                    return string.IsNullOrEmpty(Oculus_Dash.DisplayName) ? "No Name Found" : Oculus_Dash.DisplayName;

                case Dash_Type.OculusKiller:
                    // Check if DisplayName is not null or empty
                    return string.IsNullOrEmpty(SteamVR_Dash.DisplayName) ? "No Name Found" : SteamVR_Dash.DisplayName;

                default:
                    return "No Name Found";
            }
        }

        public static bool ActivateFastTransition(Dash_Type Dash)
        {
            var Activated = false;

            if (Dash != Dash_Type.Exit)
            {
                Debug.WriteLine("Starting Fast Activation: " + Dash.ToString());

                for (int i = 0; i < 10; i++)
                    if (AttemptFastSwitch(Dash))
                        break;
            }
            else
            {
                SteamRunning.Close_SteamVR_ResetLink();

                // ServiceController Service = new ServiceController("OVRService");
                // Boolean OVRServiceRunning = Running(Service.Status);

                // try
                // {
                //    Debug.WriteLine("Stopping OVRService");

                //    Service.Stop();
                //    if (OVRServiceRunning)
                //        Service.Start();
                // }
                // catch (Exception ex)
                // {
                //    Debug.WriteLine(ex.Message);
                // }
            }

            return Activated;
        }

        private static bool AttemptFastSwitch(Dash_Type Dash)
        {
            var Activated = false;

            Activated = SetActiveDash(Dash);

            return Activated;
        }

        private static bool IsServiceRunningOrPending(ServiceControllerStatus status)
        {
            switch (status)
            {
                case ServiceControllerStatus.Running:
                case ServiceControllerStatus.Paused:
                case ServiceControllerStatus.StopPending:
                case ServiceControllerStatus.StartPending:
                    return true;

                case ServiceControllerStatus.Stopped:
                default:
                    return false;
            }
        }

        public static OVR_Dash GetDash(Dash_Type Dash)
        {
            switch (Dash)
            {
                case Dash_Type.Exit:
                    break;

                case Dash_Type.Unknown:
                    break;

                case Dash_Type.Normal:
                    return Oculus_Dash;

                case Dash_Type.OculusKiller:
                    return SteamVR_Dash;

                default:
                    break;
            }

            return null;
        }
    }
}