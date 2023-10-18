using OVR_Dash_Manager.Functions;
using System;
using System.Windows;
using System.Windows.Controls;
using YOVR_Dash_Manager.Functions;

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

        private void btnApplyProfile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string profileName = "YourProfileName"; // You might get this from a TextBox or another control
                string profileData = ProfileManagementFunctions.LoadProfile(profileName);

                if (ProfileManagementFunctions.ApplyProfile(profileData))
                {
                    MessageBox.Show("Profile applied successfully!");
                }
                else
                {
                    MessageBox.Show("Failed to apply profile.");
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex, "Failed to apply profile.");
            }
        }

        private int currentPageNumber = 1;
        private int totalPageNumber = 8; // Set this to your total number of pages

        private void btnFirst_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage(1);
        }

        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            if (currentPageNumber > 1)
            {
                NavigateToPage(currentPageNumber - 1);
            }
        }

        private void btnPage_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            int pageNumber = int.Parse(btn.Content.ToString());
            NavigateToPage(pageNumber);
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (currentPageNumber < totalPageNumber)
            {
                NavigateToPage(currentPageNumber + 1);
            }
        }

        private void btnLast_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage(totalPageNumber);
        }

        private void NavigateToPage(int pageNumber)
        {
            currentPageNumber = pageNumber;

            switch (pageNumber)
            {
                case 1:
                    NavigationFrame.Navigate(new Page1());
                    break;
                case 2:
                    NavigationFrame.Navigate(new Page2());
                    break;
                case 3:
                    NavigationFrame.Navigate(new Page3());
                    break;
                case 4:
                    NavigationFrame.Navigate(new Page4());
                    break;
                case 5:
                    NavigationFrame.Navigate(new Page5());
                    break;
                case 6:
                    NavigationFrame.Navigate(new Page6());
                    break;
                case 7:
                    NavigationFrame.Navigate(new Page7());
                    break;
                case 8:
                    NavigationFrame.Navigate(new Page8());
                    break;
                // Add more cases as needed
                default:
                    MessageBox.Show("Page not found.");
                    break;
            }
        }

    }
}
