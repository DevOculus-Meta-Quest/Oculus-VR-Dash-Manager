using System.Windows;

namespace OVR_Dash_Manager.Forms.Profile_Manager
{
    /// <summary>
    /// Interaction logic for ProfileManagerHelp.xaml
    /// </summary>
    public partial class ProfileManagerHelp : Window
    {
        public ProfileManagerHelp() => InitializeComponent();

        void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = e.Uri.AbsoluteUri,
                UseShellExecute = true
            };

            System.Diagnostics.Process.Start(psi);
            e.Handled = true;
        }
    }
}