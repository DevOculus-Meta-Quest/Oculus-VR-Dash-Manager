using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace OVR_Dash_Manager.Forms.Profile_Manager
{
    public partial class Page2 : Page
    {
        private frm_ProfileManager profileManager;

        public Page2(frm_ProfileManager profileManager)
        {
            InitializeComponent();
            this.profileManager = profileManager;
        }

        public Dictionary<string, object> GetPageControlsState()
        {
            return new Dictionary<string, object>
            {
                { "chkDisableTracking", chkDisableTracking.IsChecked },
                { "chkEnableTracking", chkEnableTracking.IsChecked },
                { "txtTraceFilePath", txtTraceFilePath.Text }
            };
        }

        public void PopulateUI(Dictionary<string, object> profileData)
        {
            if (profileData != null)
            {
                profileData.TryGetValue("chkDisableTracking", out object value_chkDisableTracking);
                chkDisableTracking.IsChecked = value_chkDisableTracking as bool?;

                profileData.TryGetValue("chkEnableTracking", out object value_chkEnableTracking);
                chkEnableTracking.IsChecked = value_chkEnableTracking as bool?;

                profileData.TryGetValue("txtTraceFilePath", out object value_txtTraceFilePath);
                txtTraceFilePath.Text = value_txtTraceFilePath as string;
            }
        }

        private void Control_Changed(object sender, RoutedEventArgs e)
        {
            var controlData = new Dictionary<string, object>
            {
                { "chkDisableTracking", chkDisableTracking.IsChecked },
                { "chkEnableTracking", chkEnableTracking.IsChecked },
                { "txtTraceFilePath", txtTraceFilePath.Text }
            };

            profileManager.UpdateProfileData("Page2", controlData);
        }

        private void Button_Clicked(object sender, RoutedEventArgs e)
        {
            Button clickedButton = (Button)sender;
            string buttonContent = clickedButton.Content.ToString();

            switch (buttonContent)
            {
                case "Reset":
                    // Your code for Reset button
                    break;
                case "Select":
                    // Your code for Select button
                    break;
                    // Add cases for other buttons as needed
            }

            // You might want to update profile data here as well
            profileManager.UpdateProfileData("Page2", GetPageControlsState());
        }
    }
}
