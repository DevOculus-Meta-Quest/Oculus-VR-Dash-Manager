using OVR_Dash_Manager.Functions;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using YOVR_Dash_Manager.Functions;

namespace OVR_Dash_Manager.Forms.Profile_Manager
{
    public partial class frm_ProfileManager : Window
    {
        private Dictionary<string, object> profileData = new Dictionary<string, object>();
        private int currentPageNumber = 1;
        private int totalPageNumber = 8;

        public frm_ProfileManager()
        {
            InitializeComponent();
            LoadProfile("defaultProfile");
            NavigateToPage(1); // Initial navigation to Page1
        }

        // This method updates the profileData dictionary when changes are made in the UI.
        public void UpdateProfileData(string pageKey, Dictionary<string, object> pageControlsData)
        {
            // Check if the profileData dictionary already contains data for the specified page
            if (profileData.ContainsKey(pageKey))
            {
                // If it does, update the existing data
                var existingData = profileData[pageKey] as Dictionary<string, object>;
                foreach (var item in pageControlsData)
                {
                    existingData[item.Key] = item.Value;
                }
            }
            else
            {
                // If it doesn't, add a new entry for the page
                profileData[pageKey] = pageControlsData;
            }
        }


        private void SaveProfile(string profileName)
        {
            string jsonProfile = JsonFunctions.SerializeClass(profileData);

            string directoryPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string profilesDirectory = System.IO.Path.Combine(directoryPath, "Profiles");
            if (!System.IO.Directory.Exists(profilesDirectory))
            {
                System.IO.Directory.CreateDirectory(profilesDirectory);
            }

            string filePath = System.IO.Path.Combine(profilesDirectory, profileName + ".json");
            System.IO.File.WriteAllText(filePath, jsonProfile);
        }

        private void LoadProfile(string profileName)
        {
            string directoryPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string filePath = System.IO.Path.Combine(directoryPath, "Profiles", profileName + ".json");

            if (System.IO.File.Exists(filePath))
            {
                string jsonProfile = System.IO.File.ReadAllText(filePath);
                profileData = JsonFunctions.DeserializeClass<Dictionary<string, object>>(jsonProfile);

                Page currentPage = NavigationFrame.Content as Page;
                if (currentPage != null)
                {
                    dynamic currentPageDynamic = Convert.ChangeType(currentPage, currentPage.GetType());
                    currentPageDynamic.PopulateUI(profileData);
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
            // Save the current page's data before navigating away
            if (NavigationFrame.Content is Page currentPage)
            {
                dynamic currentPageDynamic = Convert.ChangeType(currentPage, currentPage.GetType());
                var currentPageData = currentPageDynamic.GetPageControlsState();
                foreach (var item in currentPageData)
                {
                    profileData[item.Key] = item.Value;
                }
            }

            currentPageNumber = pageNumber;

            switch (pageNumber)
            {
                case 1:
                    var page1 = new Page1(this);
                    page1.PopulateUI(profileData); // Load the saved profile data
                    NavigationFrame.Navigate(page1);
                    break;
                case 2:
                    var page2 = new Page2(this);
                    page2.PopulateUI(profileData); // Load the saved profile data
                    NavigationFrame.Navigate(page2);
                    break;
                // ... (similar code for other pages like Page3, Page4, etc.)

                default:
                    MessageBox.Show("Page not found.");
                    break;
            }
        }
    }
}
