using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace OVR_Dash_Manager.Forms.Profile_Manager
{
    public partial class Page7 : Page
    {
        private frm_ProfileManager profileManager;

        public Page7(frm_ProfileManager profileManager)
        {
            InitializeComponent();
            this.profileManager = profileManager;
        }

        public void PopulateUI(Dictionary<string, object> profileData)
        {
            if (profileData != null)
            {
            }
            else
            {
                MessageBox.Show("No profile data available to populate the UI.");
            }
        }
    }
}