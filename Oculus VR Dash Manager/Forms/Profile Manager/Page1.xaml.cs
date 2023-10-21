using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using OVR_Dash_Manager.Forms.Profile_Manager;

namespace OVR_Dash_Manager.Forms.Profile_Manager
{
    public partial class Page1 : Page
    {
        private frm_ProfileManager profileManager;

        public Page1(frm_ProfileManager profileManager)
        {
            InitializeComponent();
            this.profileManager = profileManager;
        }

        private void CheckBox_Changed(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            bool isChecked = checkBox.IsChecked.GetValueOrDefault();

            // Creating a dictionary to pass as the second argument
            var controlData = new Dictionary<string, object>
    {
        { checkBox.Name, isChecked }
    };

            profileManager.UpdateProfileData("Page1", controlData);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            ComboBoxItem selectedItem = (ComboBoxItem)comboBox.SelectedItem;

            // Creating a dictionary to pass as the second argument
            var controlData = new Dictionary<string, object>
    {
        { comboBox.Name, selectedItem.Content.ToString() }
    };

            profileManager.UpdateProfileData("Page1", controlData);
        }

        public void PopulateUI(Dictionary<string, object> profileData)
        {
            if (profileData != null)
            {
                profileData.TryGetValue("pass_aswiad", out object value_aswiad);
                pass_aswiad.IsChecked = value_aswiad as bool?;

                profileData.TryGetValue("pass_cpufilter", out object value_cpufilter);
                pass_cpufilter.IsChecked = value_cpufilter as bool?;

                profileData.TryGetValue("pass_gpufilter", out object value_gpufilter);
                pass_gpufilter.IsChecked = value_gpufilter as bool?;

                profileData.TryGetValue("pass_low", out object value_low);
                pass_low.IsChecked = value_low as bool?;

                profileData.TryGetValue("pass_lowfilter", out object value_lowfilter);
                pass_lowfilter.IsChecked = value_lowfilter as bool?;

                profileData.TryGetValue("pass_medianfilter", out object value_medianfilter);
                pass_medianfilter.IsChecked = value_medianfilter as bool?;

                profileData.TryGetValue("pass_super", out object value_super);
                pass_super.IsChecked = value_super as bool?;

                profileData.TryGetValue("pass_vis", out object value_vis);
                pass_vis.IsChecked = value_vis as bool?;

                profileData.TryGetValue("rift_get_debug_hmd", out object value_rift_get_debug_hmd);
                rift_get_debug_hmd.IsChecked = value_rift_get_debug_hmd as bool?;

                profileData.TryGetValue("cmb_pass_guard", out object value_cmb_pass_guard);
                cmb_pass_guard.SelectedIndex = Convert.ToInt32(value_cmb_pass_guard);

                profileData.TryGetValue("cmb_pass_mixreality", out object value_cmb_pass_mixreality);
                cmb_pass_mixreality.SelectedIndex = Convert.ToInt32(value_cmb_pass_mixreality);

                profileData.TryGetValue("cmb_pass_depth", out object value_cmb_pass_depth);
                cmb_pass_depth.SelectedIndex = Convert.ToInt32(value_cmb_pass_depth);

                profileData.TryGetValue("cmb_pass_filter", out object value_cmb_pass_filter);
                cmb_pass_filter.SelectedIndex = Convert.ToInt32(value_cmb_pass_filter);

                profileData.TryGetValue("cmb_pass_hud", out object value_cmb_pass_hud);
                cmb_pass_hud.SelectedIndex = Convert.ToInt32(value_cmb_pass_hud);

                profileData.TryGetValue("cmb_pass_iad", out object value_cmb_pass_iad);
                cmb_pass_iad.SelectedIndex = Convert.ToInt32(value_cmb_pass_iad);
            }
            else
            {
                MessageBox.Show("No profile data available to populate the UI.");
            }
        }

        public Dictionary<string, object> GetPageControlsState()
        {
            return new Dictionary<string, object>
            {
                { "pass_aswiad", pass_aswiad.IsChecked },
                { "pass_cpufilter", pass_cpufilter.IsChecked },
                { "pass_gpufilter", pass_gpufilter.IsChecked },
                { "pass_low", pass_low.IsChecked },
                { "pass_lowfilter", pass_lowfilter.IsChecked },
                { "pass_medianfilter", pass_medianfilter.IsChecked },
                { "pass_super", pass_super.IsChecked },
                { "pass_vis", pass_vis.IsChecked },
                { "rift_get_debug_hmd", rift_get_debug_hmd.IsChecked },
                { "cmb_pass_guard", cmb_pass_guard.SelectedIndex },
                { "cmb_pass_mixreality", cmb_pass_mixreality.SelectedIndex },
                { "cmb_pass_depth", cmb_pass_depth.SelectedIndex },
                { "cmb_pass_filter", cmb_pass_filter.SelectedIndex },
                { "cmb_pass_hud", cmb_pass_hud.SelectedIndex },
                { "cmb_pass_iad", cmb_pass_iad.SelectedIndex }
            };
        }

    }
}
