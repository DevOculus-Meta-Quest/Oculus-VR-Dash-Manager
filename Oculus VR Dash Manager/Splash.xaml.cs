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
using System.Windows.Shapes;

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
        }

        private void chkDontShowAgain_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.ShowSplashScreen = !chkDontShowAgain.IsChecked.Value;
            Properties.Settings.Default.Save();
        }
    }
}