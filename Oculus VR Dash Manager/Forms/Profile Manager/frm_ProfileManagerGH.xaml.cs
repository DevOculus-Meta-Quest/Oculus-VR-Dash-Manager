using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using YOVR_Dash_Manager.Functions; // Ensure you have the correct using directive

namespace OVR_Dash_Manager.Forms.Profile_Manager
{
    /// <summary>
    /// Interaction logic for frm_ProfileManagerGH.xaml
    /// </summary>
    public partial class frm_ProfileManagerGH : Window
    {
        private Github github;
        private OculusDebugToolFunctions oculusDebugToolFunctions;
        private readonly frm_ProfileManager profileManager; // Moved outside of the constructor

        public frm_ProfileManagerGH()
        {
            InitializeComponent();
            this.profileManager = profileManager; // Save the reference to the frm_ProfileManager
            github = new Github();
            oculusDebugToolFunctions = new OculusDebugToolFunctions();
            LoadScriptsAsync();
        }

        private async void LoadScriptsAsync()
        {
            try
            {
                string jsonResponse = await github.GetFilesFromDirectoryAsync("DevOculus-Meta-Quest", "OVRDM-Profile-Scripts", "OVRDM-Profile-Scripts");

                // Deserialize the JSON response
                var files = JsonConvert.DeserializeObject<List<GitHubFile>>(jsonResponse);

                scriptsListView.Items.Clear();
                foreach (var file in files)
                {
                    scriptsListView.Items.Add(file.name); // Assuming each file object has a 'name' property
                }
            }
            catch (HttpRequestException httpEx)
            {
                // Log or display more detailed error information
                ErrorLogger.LogError(httpEx, $"HTTP Request Failed: {httpEx.Message}");
                MessageBox.Show($"An HTTP error occurred while loading the scripts: {httpEx.Message}");
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex, "An error occurred while loading the scripts.");
                MessageBox.Show("An error occurred while loading the scripts. Check the error log for details.");
            }
        }

        // Define a class to represent the file objects in the JSON response
        public class GitHubFile
        {
            public string name { get; set; }
            // Add other necessary properties here
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadScriptsAsync();
        }

        private async void scriptsListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedItem = scriptsListView.SelectedItem as string;
            if (!string.IsNullOrEmpty(selectedItem))
            {
                try
                {
                    string scriptUrl = await github.GetFileDownloadUrlAsync("DevOculus-Meta-Quest", "OVRDM-Profile-Scripts", $"OVRDM-Profile-Scripts/{selectedItem}");

                    if (scriptUrl != null)
                    {
                        // Download the script content and save it to a temporary file
                        string tempFilePath = await DownloadAndSaveTempFileAsync(scriptUrl);

                        // Read and display the contents of the temporary file
                        string fileContents = File.ReadAllText(tempFilePath);
                        Debug.WriteLine(fileContents); // Output to console
                        MessageBox.Show(fileContents); // Or display in a message box

                        // Execute the downloaded script file
                        await oculusDebugToolFunctions.ExecuteCommandWithFileAsync(tempFilePath);
                        MessageBox.Show($"Executed the script: {selectedItem}");

                        // Optionally, delete the temporary file after execution
                        File.Delete(tempFilePath);
                    }
                    else
                    {
                        MessageBox.Show("Error: Script not found on GitHub.");
                    }
                }
                catch (Exception ex)
                {
                    ErrorLogger.LogError(ex, "An error occurred while executing the script.");
                    MessageBox.Show("An error occurred while executing the script. Check the error log for details.");
                }
            }
        }

        private async Task<string> DownloadAndSaveTempFileAsync(string downloadUrl)
        {
            using HttpClient httpClient = new HttpClient();
            string fileContent = await httpClient.GetStringAsync(downloadUrl);

            // Creating a temporary file with a specific extension if needed
            string tempFilePath = Path.GetTempFileName();

            // Writing the content fetched from the URL into the temporary file with UTF-8 encoding
            await File.WriteAllTextAsync(tempFilePath, fileContent, Encoding.UTF8);

            return tempFilePath;
        }

        private async void btnExecuteCommand_Click(object sender, EventArgs e)
        {
        }
    }
}