using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OVR_Dash_Manager.Forms.Profile_Manager
{
    public partial class Page4 : Page
    {
        private frm_ProfileManager profileManager;

        public Page4(frm_ProfileManager profileManager)
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
