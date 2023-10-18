using System.Windows;

namespace OVR_Dash_Manager.Forms.Profile_Manager
{
    /// <summary>
    /// Interaction logic for frm_ProfileManager.xaml
    /// </summary>
    public partial class frm_ProfileManager : Window
    {
        public frm_ProfileManager()
        {
            InitializeComponent();
            NavigationFrame.Navigate(new Page1());
        }

        private void btn_Page1_Click(object sender, RoutedEventArgs e)
        {
            NavigationFrame.Navigate(new Page1());
        }

        private void btn_Page2_Click(object sender, RoutedEventArgs e)
        {
            NavigationFrame.Navigate(new Page2());
        }

        private void btn_Page3_Click(object sender, RoutedEventArgs e)
        {
            NavigationFrame.Navigate(new Page3());
        }

        private void btn_Page4_Click(object sender, RoutedEventArgs e)
        {
            NavigationFrame.Navigate(new Page4());
        }

        private void btn_Page5_Click(object sender, RoutedEventArgs e)
        {
            NavigationFrame.Navigate(new Page5());
        }

        private void btn_Page6_Click(object sender, RoutedEventArgs e)
        {
            NavigationFrame.Navigate(new Page6());
        }

        private void btn_Home_Click(object sender, RoutedEventArgs e)
        {
            NavigationFrame.Navigate(new Page1()); // Assuming Page1 is the home page
        }
    }
}
