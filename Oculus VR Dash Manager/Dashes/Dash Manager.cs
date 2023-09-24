using System;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace OVR_Dash_Manager.Dashes
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
        public static void PassMainForm(MainWindow Form)
        {
            MainForm = Form;
        }

        /// <summary>
        /// Fixes the task view issue on the main form.
        /// </summary>
        public static void MainForm_FixTaskViewIssue()
        {
            if (MainForm != null)
                Functions_Old.DoAction(MainForm, new Action(delegate () { MainForm.Cancel_TaskView_And_Focus(); }));
        }

        /// <summary>
        /// Checks the runtime on the main form.
        /// </summary>
        public static void MainForm_CheckRunTime()
        {
            if (MainForm != null)
                Functions_Old.DoAction(MainForm, new Action(delegate () { MainForm.CheckRunTime(); }));
        }

        /// <summary>
        /// Emulates the release mode.
        /// </summary>
        /// <returns>True if in debug mode, false otherwise.</returns>
        public static bool EmulateReleaseMode()
        {
            return MainForm?.Debug_EmulateReleaseMode ?? false;
        }

        /// <summary>
        /// Generates the dashes.
        /// </summary>
        public static async Task GenerateDashesAsync()
        {
            Software.Oculus.Check_Current_Dash();

            Oculus_Dash = new OVR_Dash("Official Oculus Dash", "OculusDash_Normal.exe", ProcessToStop: "vrmonitor");
            SteamVR_Dash = new OVR_Dash("Oculus Killer", "Oculus_Killer.exe", "Oculus Killer", "OculusKiller", "OculusDash.exe");

            Software.Oculus.Setup();
            Software.Oculus.Check_Oculus_Is_Installed();

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

            Software.Oculus.Check_Current_Dash();

            if (!Oculus_Dash.Installed && Software.Oculus.Normal_Dash)
            {
                // Copy Default Oculus Dash if not already done
                File.Copy(Software.Oculus.Oculus_Dash_File, Path.Combine(Software.Oculus.Oculus_Dash_Directory, Oculus_Dash.DashFileName), true);
                Oculus_Dash.CheckInstalled();
            }
            else if (Software.Oculus.Normal_Dash)
            {
                // Check if Oculus Updated and check is Oculus Dash has changed by "Length"
                FileInfo CurrentDash = new FileInfo(Path.Combine(Software.Oculus.Oculus_Dash_Directory, Oculus_Dash.DashFileName));
                FileInfo OculusDashFile = new FileInfo(Software.Oculus.Oculus_Dash_File);

                // Update File
                if (CurrentDash.Length != OculusDashFile.Length)
                    File.Copy(Software.Oculus.Oculus_Dash_File, Path.Combine(Software.Oculus.Oculus_Dash_Directory, Oculus_Dash.DashFileName), true);
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
        private static bool SetActiveDash(Dash_Type Dash)
        {
            bool activated = false;

            switch (Dash)
            {
                case Dash_Type.Normal:
                    Software.Steam.ManagerCalledExit = true;
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

        public static Dash_Type CheckWhosDash(String File_ProductName)
        {
            if (SteamVR_Dash.IsThisYourDash(File_ProductName))
                return Dash_Type.OculusKiller;

            if (String.IsNullOrEmpty(File_ProductName))
                return Dash_Type.Normal;

            return Dash_Type.Unknown;
        }

        public static String GetDashName(Dash_Type Dash)
        {
            switch (Dash)
            {
                case Dash_Type.Unknown:
                    return "Unknown";

                case Dash_Type.Normal:
                    return Oculus_Dash.DisplayName;

                case Dash_Type.OculusKiller:
                    return SteamVR_Dash.DisplayName;

                default:
                    return "No Name Found";
            }
        }

        public static Boolean Activate_FastTransition(Dash_Type Dash)
        {
            Boolean Activated = false;

            if (Dash != Dash_Type.Exit)
            {
                Debug.WriteLine("Starting Fast Activation: " + Dash.ToString());

                for (int i = 0; i < 10; i++)
                {
                    if (AttemptFastSwitch(Dash))
                        break;
                }
            }
            else
            {
                Software.Steam.Close_SteamVR_ResetLink();

                //ServiceController Service = new ServiceController("OVRService");
                //Boolean OVRServiceRunning = Running(Service.Status);

                //try
                //{
                //    Debug.WriteLine("Stopping OVRService");

                //    Service.Stop();
                //    if (OVRServiceRunning)
                //        Service.Start();
                //}
                //catch (Exception ex)
                //{
                //    Debug.WriteLine(ex.Message);
                //}
            }
            return Activated;
        }

        private static Boolean AttemptFastSwitch(Dash_Type Dash)
        {
            Boolean Activated = false;

            Activated = SetActiveDash(Dash);

            return Activated;
        }

        public static Boolean Activate(Dash_Type Dash)
        {
            Debug.WriteLine("Starting Activation: " + Dash.ToString());

            Boolean Activated = false;

            Boolean OVRServiceRunning = (Service_Manager.GetState("OVRService") == "Running");
            Boolean OVRService_WasRunning = false;
            Boolean CanAccess = true;

            if (OVRServiceRunning)
            {
                OVRService_WasRunning = true;
                try
                {
                    Debug.WriteLine("Stopping OVRService");
                    Software.Steam.ManagerCalledExit = true;
                    Service_Manager.StopService("OVRService");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    CanAccess = false;
                }
            }

            if (CanAccess)
            {
                Debug.WriteLine("Checking OVRService");

                OVRServiceRunning = (Service_Manager.GetState("OVRService") == "Running");

                if (!OVRServiceRunning)
                {
                    Debug.WriteLine("Activating Dash");
                    Activated = SetActiveDash(Dash);
                }
                else
                    Debug.WriteLine("!!!!!! OVRService Can Not Be Stopped");

                if (OVRService_WasRunning)
                {
                    Debug.WriteLine("Restarting OVRService");
                    Service_Manager.StartService("OVRService");
                }
            }
            else
                Debug.WriteLine("!!!!!! OVRService Can Not Be Accessed");

            return Activated;
        }

        private static Boolean Running(ServiceControllerStatus Status)
        {
            switch (Status)
            {
                case ServiceControllerStatus.Running:
                    return true;

                case ServiceControllerStatus.Stopped:
                    return false;

                case ServiceControllerStatus.Paused:
                    return true;

                case ServiceControllerStatus.StopPending:
                    return true;

                case ServiceControllerStatus.StartPending:
                    return true;

                default:
                    return false;
            }
        }

        public static bool Oculus_Official_Dash_Installed()
        {
            return Oculus_Dash.Installed;
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