using System;
using System.Windows;
using System.Windows.Controls;

namespace OVR_Dash_Manager.Forms
{
    /// <summary>
    /// Interaction logic for frm_Settings.xaml
    /// </summary>
    public partial class frm_Settings : Window
    {
        public frm_Settings() => InitializeComponent();

        void Window_Loaded(object sender, RoutedEventArgs e) => UpdateCheckBoxes();

        void UpdateCheckBoxes()
        {
            foreach (UIElement item in gd_Settings.Children)
            {
                if (item is CheckBox box)
                {
                    if (box.Tag != null)
                        box.IsChecked = Get_Properties_Setting(box.Tag.ToString());
                }
            }
        }

        void Update_Properties_Setting(string SettingName, bool Value, CheckBox DisplayBox = null)
        {
            var Setting = false;

            try
            {
                Setting = (bool)Properties.Settings.Default[SettingName];
            }
            catch (Exception)
            {
                return;
            }

            if (Setting != Value)
            {
                Properties.Settings.Default[SettingName] = Value;

                if (DisplayBox != null)
                    DisplayBox.IsChecked = Value;

                Properties.Settings.Default.Save();
            }
        }

        bool Get_Properties_Setting(string SettingName)
        {
            var Setting = false;

            try
            {
                Setting = (bool)Properties.Settings.Default[SettingName];
            }
            catch (Exception)
            {
                return false;
            }

            return Setting;
        }

        void chkbx_AlwaysOnTop_Checked(object sender, RoutedEventArgs e)
        {
            Update_Properties_Setting(chkbx_AlwaysOnTop.Tag.ToString(), true, chkbx_AlwaysOnTop);
        }

        void chkbx_AlwaysOnTop_Unchecked(object sender, RoutedEventArgs e)
        {
            Update_Properties_Setting(chkbx_AlwaysOnTop.Tag.ToString(), false, chkbx_AlwaysOnTop);
        }

        void chkbx_FastSwitch_Checked(object sender, RoutedEventArgs e)
        {
            Update_Properties_Setting(chkbx_FastSwitch.Tag.ToString(), true, chkbx_FastSwitch);
        }

        void chkbx_FastSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            Update_Properties_Setting(chkbx_FastSwitch.Tag.ToString(), false, chkbx_FastSwitch);
        }

        void chkbx_ShutdownServices_Checked(object sender, RoutedEventArgs e)
        {
            Update_Properties_Setting(chkbx_ShutdownServices.Tag.ToString(), true, chkbx_ShutdownServices);
        }

        void chkbx_ShutdownServices_Unchecked(object sender, RoutedEventArgs e)
        {
            Update_Properties_Setting(chkbx_ShutdownServices.Tag.ToString(), false, chkbx_ShutdownServices);
        }

        void chkbx_StartOculusClientOnLaunch_Checked(object sender, RoutedEventArgs e)
        {
            Update_Properties_Setting(chkbx_StartOculusClientOnLaunch.Tag.ToString(), true, chkbx_StartOculusClientOnLaunch);
        }

        void chkbx_StartOculusClientOnLaunch_Unchecked(object sender, RoutedEventArgs e)
        {
            Update_Properties_Setting(chkbx_StartOculusClientOnLaunch.Tag.ToString(), false, chkbx_StartOculusClientOnLaunch);
        }

        void chkbx_SteamVRFocusFix_Checked(object sender, RoutedEventArgs e)
        {
            Update_Properties_Setting(chkbx_SteamVRFocusFix.Tag.ToString(), true, chkbx_SteamVRFocusFix);
        }

        void chkbx_SteamVRFocusFix_Unchecked(object sender, RoutedEventArgs e)
        {
            Update_Properties_Setting(chkbx_SteamVRFocusFix.Tag.ToString(), false, chkbx_SteamVRFocusFix);
        }

        void chkbx_ExitLink_OnUserClose_Steam_Checked(object sender, RoutedEventArgs e)
        {
            Update_Properties_Setting(chkbx_ExitLink_OnUserClose_Steam.Tag.ToString(), true, chkbx_ExitLink_OnUserClose_Steam);
        }

        void chkbx_ExitLink_OnUserClose_Steam_Unchecked(object sender, RoutedEventArgs e)
        {
            Update_Properties_Setting(chkbx_ExitLink_OnUserClose_Steam.Tag.ToString(), false, chkbx_ExitLink_OnUserClose_Steam);
        }

        void chkbx_CloseOculusClientOnLaunch_Checked(object sender, RoutedEventArgs e)
        {
            Update_Properties_Setting(chkbx_CloseOculusClientOnLaunch.Tag.ToString(), true, chkbx_CloseOculusClientOnLaunch);
        }

        void chkbx_CloseOculusClientOnLaunch_Unchecked(object sender, RoutedEventArgs e)
        {
            Update_Properties_Setting(chkbx_CloseOculusClientOnLaunch.Tag.ToString(), false, chkbx_CloseOculusClientOnLaunch);
        }

        void chkbx_MinimizeOculusClientOnClientLaunch_Checked(object sender, RoutedEventArgs e)
        {
            Update_Properties_Setting(chkbx_MinimizeOculusClientOnClientLaunch.Tag.ToString(), true, chkbx_MinimizeOculusClientOnClientLaunch);
        }

        void chkbx_MinimizeOculusClientOnClientLaunch_Unchecked(object sender, RoutedEventArgs e)
        {
            Update_Properties_Setting(chkbx_MinimizeOculusClientOnClientLaunch.Tag.ToString(), false, chkbx_MinimizeOculusClientOnClientLaunch);
        }
    }
}