using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;

namespace OVR_Dash_Manager.Forms.Profile_Manager
{
    public partial class Page2 : Page
    {
        private frm_ProfileManager profileManager;

        public Page2(frm_ProfileManager profileManager)
        {
            InitializeComponent();
            this.profileManager = profileManager;

            // Adding event handlers for checkboxes and buttons
            chkDisableTracking.Checked += ControlStateChanged;
            chkDisableTracking.Unchecked += ControlStateChanged;
            chkEnableTracking.Checked += ControlStateChanged;
            chkEnableTracking.Unchecked += ControlStateChanged;
            txtTraceFilePath.TextChanged += ControlStateChanged;
            btnReportState.Click += ControlStateChanged;
            // ... (add event handlers for other buttons as needed)
        }

        private void ControlStateChanged(object sender, RoutedEventArgs e)
        {
            profileManager.UpdateProfileData("Page2Controls", GetPageControlsState());
        }

        public void PopulateUI(Dictionary<string, object> profileData)
        {
            if (profileData != null && profileData.ContainsKey("Page2Controls"))
            {
                var page2Controls = (Dictionary<string, object>)profileData["Page2Controls"];
                chkDisableTracking.IsChecked = (bool?)page2Controls["chkDisableTracking"];
                chkEnableTracking.IsChecked = (bool?)page2Controls["chkEnableTracking"];
                txtTraceFilePath.Text = (string)page2Controls["txtTraceFilePath"];
                // ... (set other controls as needed)
            }
        }

        public Dictionary<string, object> GetPageControlsState()
        {
            return new Dictionary<string, object>
            {
                { "chkDisableTracking", chkDisableTracking.IsChecked },
                { "chkEnableTracking", chkEnableTracking.IsChecked },
                { "txtTraceFilePath", txtTraceFilePath.Text },
                // ... (add other controls as needed)
            };
        }
    }
}
