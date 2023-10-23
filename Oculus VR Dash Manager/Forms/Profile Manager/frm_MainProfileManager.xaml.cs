using System.Diagnostics;
using System.Windows;

namespace OVR_Dash_Manager.Forms.Profile_Manager
{
    /// <summary>
    /// Interaction logic for frm_MainProfileManager.xaml
    /// </summary>
    public partial class frm_MainProfileManager : Window
    {
        public frm_MainProfileManager()
        {
            InitializeComponent();
            Debug.WriteLine("frm_MainProfileManager initialized."); // Debug log
        }

        private void btn_ProfileManagerGH_Click(object sender, RoutedEventArgs e)
        {
            // Create an instance of the frm_ProfileManager window and show it
            frm_ProfileManagerGH profileManager = new frm_ProfileManagerGH();
            profileManager.Owner = this; // 'this' refers to the main window
            profileManager.ShowDialog();
        }

        private void btn_ProfileManager_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}