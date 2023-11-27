using System.Windows;

namespace OVR_Dash_Manager
{
    /// <summary>
    /// Interaction logic for Splash.xaml
    /// </summary>
    public partial class Splash : Window
    {
        public Splash()
        {
            InitializeComponent();

            // Center the window on the screen
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // Keep the window always on top
            this.Topmost = true;
        }

        private void chkDontShowAgain_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.ShowSplashScreen = !chkDontShowAgain.IsChecked.Value;
            Properties.Settings.Default.Save();
        }
    }
}