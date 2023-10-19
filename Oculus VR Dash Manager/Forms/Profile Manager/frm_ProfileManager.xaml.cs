using OVR_Dash_Manager.Functions;
using System;
using System.Collections.Generic;
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
        private Dictionary<string, object> profileData = new Dictionary<string, object>();
        private int currentPageNumber = 1;
        private int totalPageNumber = 8; // Set this to your total number of pages

        public frm_ProfileManager()
        {
            InitializeComponent();
            LoadProfile("defaultProfile"); // Load the saved profile data with a default or placeholder profile name
            NavigationFrame.Navigate(new Page1(this)); // Navigating to Page1 during initialization
        }

        public void UpdateProfileData(string key, object value)
        {
            profileData[key] = value;
        }

        private void SaveProfile(string profileName)
        {
            string jsonProfile = JsonFunctions.SerializeClass(profileData);

            // Getting the directory of the executing assembly
            string directoryPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            // Creating the Profiles directory if it doesn't exist
            string profilesDirectory = System.IO.Path.Combine(directoryPath, "Profiles");
            if (!System.IO.Directory.Exists(profilesDirectory))
            {
                System.IO.Directory.CreateDirectory(profilesDirectory);
            }

            // Saving the jsonProfile to a file within the Profiles directory
            string filePath = System.IO.Path.Combine(profilesDirectory, profileName + ".json");
            System.IO.File.WriteAllText(filePath, jsonProfile);
        }

        private void LoadProfile(string profileName)
        {
            // Getting the directory of the executing assembly
            string directoryPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            // Getting the file path within the Profiles directory
            string filePath = System.IO.Path.Combine(directoryPath, "Profiles", profileName + ".json");

            if (System.IO.File.Exists(filePath))
            {
                string jsonProfile = System.IO.File.ReadAllText(filePath);
                profileData = JsonFunctions.DeserializeClass<Dictionary<string, object>>(jsonProfile);

                // Assuming NavigationFrame.Content holds the current page
                Page currentPage = NavigationFrame.Content as Page;
                if (currentPage != null)
                {
                    switch (currentPage.GetType().Name)
                    {
                        case "Page1":
                            ((Page1)currentPage).PopulateUI(profileData);
                            break;
                        case "Page2":
                            ((Page2)currentPage).PopulateUI(profileData);
                            break;
                        case "Page3":
                            ((Page3)currentPage).PopulateUI(profileData);
                            break;
                        case "Page4":
                            ((Page4)currentPage).PopulateUI(profileData);
                            break;
                        case "Page5":
                            ((Page5)currentPage).PopulateUI(profileData);
                            break;
                        case "Page6":
                            ((Page6)currentPage).PopulateUI(profileData);
                            break;
                        case "Page7":
                            ((Page7)currentPage).PopulateUI(profileData);
                            break;
                        case "Page8":
                            ((Page8)currentPage).PopulateUI(profileData);
                            break;
                        default:
                            MessageBox.Show("Page not recognized for loading profile.");
                            break;
                    }
                }
            }
            else
            {
                MessageBox.Show("Profile not found.");
            }
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

        // This is a method that you might call when a Save button is clicked.
        private void btnSaveProfile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string profileName = "YourProfileName"; // You might get this from a TextBox or another control
                SaveProfile(profileName);
                MessageBox.Show("Profile saved successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save profile.");
                ErrorLogger.LogError(ex, "Failed to save profile.");
            }
        }

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
                    NavigationFrame.Navigate(new Page1(this)); // Passing this instance
                    break;
                case 2:
                    NavigationFrame.Navigate(new Page2(this)); // Passing this instance
                    break;
                case 3:
                    NavigationFrame.Navigate(new Page3(this)); // Passing this instance
                    break;
                case 4:
                    NavigationFrame.Navigate(new Page4(this)); // Passing this instance
                    break;
                case 5:
                    NavigationFrame.Navigate(new Page5(this)); // Passing this instance
                    break;
                case 6:
                    NavigationFrame.Navigate(new Page6(this)); // Passing this instance
                    break;
                case 7:
                    NavigationFrame.Navigate(new Page7(this)); // Passing this instance
                    break;
                case 8:
                    NavigationFrame.Navigate(new Page8(this)); // Passing this instance
                    break;
                // ... (similar changes for other cases)
                default:
                    MessageBox.Show("Page not found.");
                    break;
            }
        }
    }
}
