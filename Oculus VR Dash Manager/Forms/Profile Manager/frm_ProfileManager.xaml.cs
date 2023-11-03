using ImageMagick;
using Microsoft.Win32;
using Newtonsoft.Json;
using OVR_Dash_Manager.Functions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using YOVR_Dash_Manager.Functions;
using RegistryKeyType = OVR_Dash_Manager.Functions.RegistryKeyType;



namespace OVR_Dash_Manager.Forms.Profile_Manager
{
    /// <summary>
    /// Interaction logic for frm_ProfileManager.xaml
    /// </summary>
    public partial class frm_ProfileManager : Window
    {
        private Github github;
        private OculusDebugToolFunctions oculusDebugToolFunctions;
        private readonly frm_ProfileManager profileManager; // Moved outside of the constructor

        public frm_ProfileManager()
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

                        // Execute the downloaded script file
                        oculusDebugToolFunctions.ExecuteCommandWithFile(tempFilePath);

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

        public async Task<string> DownloadAndSaveTempFileAsync(string downloadUrl)
        {
            using HttpClient httpClient = new HttpClient();

            HttpResponseMessage response = await httpClient.GetAsync(downloadUrl);

            if (response.IsSuccessStatusCode)
            {
                string fileContent = await response.Content.ReadAsStringAsync();

                string tempFilePath = System.IO.Path.GetTempFileName();

                await File.WriteAllTextAsync(tempFilePath, fileContent);

                return tempFilePath;
            }
            else
            {
                // Handle error (e.g., throw an exception or log the error)
                return null;
            }
        }

        private async void btnExecuteCommand_Click(object sender, EventArgs e)
        {
        }

        private void ProfileManagerHelp_Click(object sender, RoutedEventArgs e)
        {
            // Create an instance of the ProfileManagerHelp window
            Profile_Manager.ProfileManagerHelp helpWindow = new Profile_Manager.ProfileManagerHelp();

            // Set the Topmost property to make sure it appears above all other windows
            helpWindow.Topmost = true;

            // Show the help window as a dialog
            helpWindow.ShowDialog();
        }

        private void cb_DistortionCurvature_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            string selectedValue = ((ComboBoxItem)comboBox.SelectedItem).Content.ToString();

            string keyLocation = @"Software\Oculus\RemoteHeadset";
            string keyName = "DistortionCurve";

            // Use the RegistryFunctions to interact with the registry
            using (var key = RegistryFunctions.GetRegistryKey(OVR_Dash_Manager.Functions.RegistryKeyType.CurrentUser, keyLocation))
            {
                if (key != null)
                {
                    switch (selectedValue)
                    {
                        case "Low":
                            // Set the value to 0 for Low
                            OVR_Dash_Manager.Functions.RegistryFunctions.SetKeyValue(key, keyName, 0, RegistryValueKind.DWord);
                            break;
                        case "High":
                            // Set the value to 1 for High
                            OVR_Dash_Manager.Functions.RegistryFunctions.SetKeyValue(key, keyName, 1, RegistryValueKind.DWord);
                            break;
                        case "Default":
                            // Remove the value for Default
                            key.DeleteValue(keyName, throwOnMissingValue: false);
                            break;
                        default:
                            // Handle any other unexpected case
                            break;
                    }
                }
                else
                {
                    // Log the error using ErrorLogger
                    string errorMessage = "Unable to open the registry key at " + keyLocation;
                    ErrorLogger.LogError(new Exception(errorMessage));

                    // Optionally, attempt to create the key if it should exist
                    // This depends on the application's requirements and permissions
                    try
                    {
                        RegistryKey newKey = RegistryFunctions.CreateRegistryKey(OVR_Dash_Manager.Functions.RegistryKeyType.CurrentUser, keyLocation);
                        if (newKey != null)
                        {
                            ErrorLogger.LogError(new Exception("Registry key created at " + keyLocation));
                            newKey.Close(); // Don't forget to close the key after you're done
                        }
                        else
                        {
                            ErrorLogger.LogError(new Exception("Failed to create registry key at " + keyLocation));
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log the exception if the key creation fails
                        ErrorLogger.LogError(ex, "Failed to create registry key.");
                    }
                }
            }
        }

        private void cb_VideoCodec_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            string selectedValue = ((ComboBoxItem)comboBox.SelectedItem).Content.ToString();

            string keyLocation = @"Software\Oculus\RemoteHeadset";
            string keyName = "HVEC";

            // Use the RegistryFunctions to interact with the registry
            using (var key = OVR_Dash_Manager.Functions.RegistryFunctions.GetRegistryKey(OVR_Dash_Manager.Functions.RegistryKeyType.CurrentUser, keyLocation))
            {
                if (key != null)
                {
                    switch (selectedValue)
                    {
                        case "Default":
                            // Remove the key for Default
                            key.DeleteValue(keyName, throwOnMissingValue: false);
                            break;
                        case "H.264":
                            // Set the value to 0 for H.264
                            OVR_Dash_Manager.Functions.RegistryFunctions.SetKeyValue(key, keyName, 0, RegistryValueKind.DWord);
                            break;
                        case "H.265":
                            // Set the value to 1 for H.265
                            OVR_Dash_Manager.Functions.RegistryFunctions.SetKeyValue(key, keyName, 1, RegistryValueKind.DWord);
                            break;
                        default:
                            // Handle any other unexpected case
                            break;
                    }
                }
                else
                {
                    // Log the error using ErrorLogger
                    string errorMessage = "Unable to open the registry key at " + keyLocation;
                    ErrorLogger.LogError(new Exception(errorMessage));

                    // Optionally, attempt to create the key if it should exist
                    // This depends on the application's requirements and permissions
                    try
                    {
                        RegistryKey newKey = OVR_Dash_Manager.Functions.RegistryFunctions.CreateRegistryKey(OVR_Dash_Manager.Functions.RegistryKeyType.CurrentUser, keyLocation);
                        if (newKey != null)
                        {
                            ErrorLogger.LogError(new Exception("Registry key created at " + keyLocation));
                            newKey.Close(); // Don't forget to close the key after you're done
                        }
                        else
                        {
                            ErrorLogger.LogError(new Exception("Failed to create registry key at " + keyLocation));
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log the exception if the key creation fails
                        ErrorLogger.LogError(ex, "Failed to create registry key.");
                    }
                }
            }
        }

        private void cb_slicedEncoding_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            string selectedValue = ((ComboBoxItem)comboBox.SelectedItem).Content.ToString();

            string keyLocation = @"Software\Oculus\RemoteHeadset";
            string keyName = "NumSlices";

            // Use the RegistryFunctions to interact with the registry
            using (var key = OVR_Dash_Manager.Functions.RegistryFunctions.GetRegistryKey(OVR_Dash_Manager.Functions.RegistryKeyType.CurrentUser, keyLocation))
            {
                if (key != null)
                {
                    switch (selectedValue)
                    {
                        case "Default":
                            // Remove the key for Default
                            key.DeleteValue(keyName, throwOnMissingValue: false);
                            break;
                        case "On":
                            // Set the value to 5 for On
                            OVR_Dash_Manager.Functions.RegistryFunctions.SetKeyValue(key, keyName, 5, RegistryValueKind.DWord);
                            break;
                        case "Off":
                            // Set the value to 1 for Off
                            OVR_Dash_Manager.Functions.RegistryFunctions.SetKeyValue(key, keyName, 1, RegistryValueKind.DWord);
                            break;
                        default:
                            // Handle any other unexpected case
                            break;
                    }
                }
                else
                {
                    // Log the error using ErrorLogger
                    string errorMessage = "Unable to open the registry key at " + keyLocation;
                    ErrorLogger.LogError(new Exception(errorMessage));

                    // Optionally, attempt to create the key if it should exist
                    // This depends on the application's requirements and permissions
                    try
                    {
                        RegistryKey newKey = OVR_Dash_Manager.Functions.RegistryFunctions.CreateRegistryKey(OVR_Dash_Manager.Functions.RegistryKeyType.CurrentUser, keyLocation);
                        if (newKey != null)
                        {
                            ErrorLogger.LogError(new Exception("Registry key created at " + keyLocation));
                            newKey.Close(); // Don't forget to close the key after you're done
                        }
                        else
                        {
                           ErrorLogger.LogError(new Exception("Failed to create registry key at " + keyLocation));
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log the exception if the key creation fails
                        ErrorLogger.LogError(ex, "Failed to create registry key.");
                    }
                }
            }
        }

        private void txt_EncodeResolutionWidth_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // This regex will match any non-digit characters.
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);

            TextBox textBox = sender as TextBox;
            if (!e.Handled && textBox != null)
            {
                // Ensure that the LostFocus event is only attached once.
                textBox.LostFocus -= Txt_EncodeResolutionWidth_LostFocus; // Detach the event in case it was already attached.
                textBox.LostFocus += Txt_EncodeResolutionWidth_LostFocus; // Attach the event.
            }
        }

        private void Txt_EncodeResolutionWidth_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                if (int.TryParse(textBox.Text, out int value))
                {
                    string keyLocation = @"Software\Oculus\RemoteHeadset";
                    string keyName = "EncodeWidth";

                    using (var key = OVR_Dash_Manager.Functions.RegistryFunctions.GetRegistryKey(OVR_Dash_Manager.Functions.RegistryKeyType.CurrentUser, keyLocation))
                    {
                        if (key != null)
                        {
                            if (value == 0)
                            {
                                // Remove the key if the value is 0
                                key.DeleteValue(keyName, throwOnMissingValue: false);
                            }
                            else if (value >= 1 && value <= 9999)
                            {
                                // Set the value if it's within the valid range
                                OVR_Dash_Manager.Functions.RegistryFunctions.SetKeyValue(key, keyName, value, RegistryValueKind.DWord);
                            }
                            // If the value is not numeric or not in range, do not update the registry.
                        }
                        else
                        {
                            // Log the error using ErrorLogger
                            string errorMessage = "Unable to open the registry key at " + keyLocation;
                            ErrorLogger.LogError(new Exception(errorMessage));
                        }
                    }
                }
                // Unsubscribe from the LostFocus event
                textBox.LostFocus -= Txt_EncodeResolutionWidth_LostFocus;
            }
        }

        private void cb_EncodeDynamicBitrate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            string keyLocation = @"Software\Oculus\RemoteHeadset";
            string keyName = "DBR";

            if (comboBox != null)
            {
                // Assuming the ComboBox items are strings. If they are ComboBoxItems, you'll need to extract the string value.
                string selectedValue = comboBox.SelectedItem as string;
                if (selectedValue == null && comboBox.SelectedItem is ComboBoxItem selectedItem)
                {
                    selectedValue = selectedItem.Content.ToString();
                }

                using (var key = OVR_Dash_Manager.Functions.RegistryFunctions.GetRegistryKey(OVR_Dash_Manager.Functions.RegistryKeyType.CurrentUser, keyLocation))
                {
                    if (key != null)
                    {
                        switch (selectedValue)
                        {
                            case "Default":
                                // Remove the key if the selection is "Default"
                                key.DeleteValue(keyName, throwOnMissingValue: false);
                                break;
                            case "Enabled":
                                // Set the key value to 1 if "Enabled" is selected
                                OVR_Dash_Manager.Functions.RegistryFunctions.SetKeyValue(key, keyName, 1, RegistryValueKind.DWord);
                                break;
                            case "Disabled":
                                // Set the key value to 0 if "Disabled" is selected
                                OVR_Dash_Manager.Functions.RegistryFunctions.SetKeyValue(key, keyName, 0, RegistryValueKind.DWord);
                                break;
                            default:
                                // Log the error or handle the unexpected value
                                string errorMessage = $"Unexpected selection: {selectedValue}";
                                ErrorLogger.LogError(new Exception(errorMessage));
                                break;
                        }
                    }
                    else
                    {
                        // Log the error using ErrorLogger
                        string errorMessage = "Unable to open the registry key at " + keyLocation;
                        ErrorLogger.LogError(new Exception(errorMessage));
                    }
                }
            }
        }

        private void txt_DynamicBitrateMax_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            string keyLocation = @"Software\Oculus\RemoteHeadset";
            string keyName = "DBRMax";

            // Check if the input is numeric and within the allowed range
            if (!long.TryParse(e.Text, out long value) || value < 0 || value > 9999999999)
            {
                // If not, handle the event as handled so the TextBox does not process the input
                e.Handled = true;
                return;
            }

            // If the input is "0", remove the key, otherwise set the new value
            if (value == 0)
            {
                // Remove the key if the input is "0"
                using (var key = OVR_Dash_Manager.Functions.RegistryFunctions.GetRegistryKey(OVR_Dash_Manager.Functions.RegistryKeyType.CurrentUser, keyLocation))
                {
                    key?.DeleteValue(keyName, throwOnMissingValue: false);
                }
            }
            else
            {
                // Set the key value to the input value if it's within the valid range
                int newValue = (int)value; // Cast long to int, assuming the value is within int range.
                using (var key = OVR_Dash_Manager.Functions.RegistryFunctions.GetRegistryKey(OVR_Dash_Manager.Functions.RegistryKeyType.CurrentUser, keyLocation))
                {
                    if (key != null)
                    {
                        // Pass the integer value and specify the value kind as DWORD.
                        OVR_Dash_Manager.Functions.RegistryFunctions.SetKeyValue(key, keyName, newValue, RegistryValueKind.DWord);
                    }
                    else
                    {
                        // Log the error using ErrorLogger
                        string errorMessage = "Unable to open the registry key at " + keyLocation;
                        ErrorLogger.LogError(new Exception(errorMessage));
                    }
                }
            }
        }

        private void txt_EncodeBitrate_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            string keyLocation = @"Software\Oculus\RemoteHeadset";
            string keyName = "BitrateMbps";

            // Check if the input is numeric and within the allowed range
            if (!int.TryParse(e.Text, out int value) || value < 0 || value > 500)
            {
                // If not, handle the event as handled so the TextBox does not process the input
                e.Handled = true;
                return;
            }

            // If the input is "0", remove the key, otherwise set the new value
            if (value == 0)
            {
                // Remove the key if the input is "0"
                using (var key = OVR_Dash_Manager.Functions.RegistryFunctions.GetRegistryKey(OVR_Dash_Manager.Functions.RegistryKeyType.CurrentUser, keyLocation))
                {
                    key?.DeleteValue(keyName, throwOnMissingValue: false);
                }
            }
            else
            {
                // Set the key value to the input value if it's within the valid range
                using (var key = OVR_Dash_Manager.Functions.RegistryFunctions.GetRegistryKey(OVR_Dash_Manager.Functions.RegistryKeyType.CurrentUser, keyLocation))
                {
                    if (key != null)
                    {
                        // Pass the integer value and specify the value kind as DWORD since bitrate is typically an integer value.
                        OVR_Dash_Manager.Functions.RegistryFunctions.SetKeyValue(key, keyName, value, RegistryValueKind.DWord);
                    }
                    else
                    {
                        // Log the error using ErrorLogger
                        string errorMessage = "Unable to open the registry key at " + keyLocation;
                        ErrorLogger.LogError(new Exception(errorMessage));
                    }
                }
            }
        }

        private void txt_DynamicBitrateOffset_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            string keyLocation = @"Software\Oculus\RemoteHeadset";
            string keyName = "DBROffsetMbps";

            // Check if the input is numeric and within the allowed range
            if (!int.TryParse(e.Text, out int value) || value < 0 || value > 999999999)
            {
                // If not, handle the event as handled so the TextBox does not process the input
                e.Handled = true;
                return;
            }

            // If the input is "0", remove the key, otherwise set the new value
            if (value == 0)
            {
                // Remove the key if the input is "0"
                using (var key = OVR_Dash_Manager.Functions.RegistryFunctions.GetRegistryKey(OVR_Dash_Manager.Functions.RegistryKeyType.CurrentUser, keyLocation))
                {
                    key?.DeleteValue(keyName, throwOnMissingValue: false);
                }
            }
            else
            {
                // Set the key value to the input value if it's within the valid range
                string newValue = value.ToString();
                using (var key = OVR_Dash_Manager.Functions.RegistryFunctions.GetRegistryKey(OVR_Dash_Manager.Functions.RegistryKeyType.CurrentUser, keyLocation))
                {
                    if (key != null)
                    {
                        OVR_Dash_Manager.Functions.RegistryFunctions.SetKeyValue(key, keyName, newValue, RegistryValueKind.String);

                    }
                    else
                    {
                        // Log the error using ErrorLogger
                        string errorMessage = "Unable to open the registry key at " + keyLocation;
                        ErrorLogger.LogError(new Exception(errorMessage));
                    }
                }
            }
        }

        private void cb_LinkSharpening_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            string keyLocation = @"Software\Oculus\RemoteHeadset";
            string keyName = "LinkSharpeningEnabled";

            // Check the selected item and set the registry value accordingly
            if (comboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string selectedValue = selectedItem.Content.ToString();
                int valueToSet = selectedValue switch
                {
                    "Disabled" => 0, // Assuming 0 is the correct value for Disabled
                    "Normal" => 1,   // Assuming 1 is the correct value for Normal
                    "Quality" => 2,  // Assuming 2 is the correct value for Quality
                    _ => throw new InvalidOperationException($"Invalid selection for Link Sharpening: {selectedValue}")
                };

                // Update the registry with the new value
                using (var key = OVR_Dash_Manager.Functions.RegistryFunctions.GetRegistryKey(OVR_Dash_Manager.Functions.RegistryKeyType.CurrentUser, keyLocation))
                {
                    if (key != null)
                    {
                        bool result = OVR_Dash_Manager.Functions.RegistryFunctions.SetKeyValue(key, keyName, valueToSet, RegistryValueKind.DWord); // Corrected line
                        if (result)
                        {
                            Debug.WriteLine($"Successfully set {keyName} to {valueToSet}");
                        }
                        else
                        {
                            Debug.WriteLine($"Failed to set {keyName} to {valueToSet}");
                        }
                    }
                    else
                    {
                        // Log the error using ErrorLogger
                        string errorMessage = $"Unable to open the registry key at {keyLocation}";
                        ErrorLogger.LogError(new Exception(errorMessage));
                        Debug.WriteLine(errorMessage);
                    }
                }
            }
            else
            {
                string errorMessage = "The selected item is not a ComboBoxItem.";
                ErrorLogger.LogError(new Exception(errorMessage));
                Debug.WriteLine(errorMessage);
            }
        }

        private void cb_LocalDimming_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            string keyLocation = @"Software\Oculus\RemoteHeadset";
            string keyName = "LocalDimming";

            // Assuming the ComboBox items are ComboBoxItem objects
            if (comboBox.SelectedItem is ComboBoxItem selectedComboBoxItem)
            {
                string selectedItemContent = selectedComboBoxItem.Content.ToString();
                Debug.WriteLine($"Selected item content: '{selectedItemContent}'");

                int? valueToSet = selectedItemContent switch
                {
                    "Enabled" => 1,
                    "Disabled" => 0,
                    _ => null // Use null for unexpected values
                };

                if (valueToSet != null)
                {
                    // Update the registry with the new value
                    using (var key = OVR_Dash_Manager.Functions.RegistryFunctions.GetRegistryKey(OVR_Dash_Manager.Functions.RegistryKeyType.CurrentUser, keyLocation))
                    {
                        if (key != null)
                        {
                            try
                            {
                                // Attempt to set the value and log the result
                                key.SetValue(keyName, valueToSet.Value, RegistryValueKind.DWord);
                                Debug.WriteLine($"Successfully set {keyName} to {valueToSet.Value}");
                            }
                            catch (Exception ex)
                            {
                                // Log the exception using ErrorLogger
                                string errorMessage = $"Failed to set {keyName} to {valueToSet.Value}: {ex.Message}";
                                ErrorLogger.LogError(ex, errorMessage);
                                Debug.WriteLine(errorMessage);
                            }
                        }
                        else
                        {
                            // Log the error using ErrorLogger
                            string errorMessage = "Unable to open the registry key at " + keyLocation;
                            ErrorLogger.LogError(new Exception(errorMessage));
                            Debug.WriteLine(errorMessage);
                        }
                    }
                }
                else
                {
                    // Log the error using ErrorLogger
                    string errorMessage = $"Invalid selection for Local Dimming: {selectedItemContent}";
                    ErrorLogger.LogError(new Exception(errorMessage));
                    Debug.WriteLine(errorMessage);
                }
            }
            else
            {
                // Log the error using ErrorLogger
                string errorMessage = "The selected item is not a ComboBoxItem.";
                ErrorLogger.LogError(new Exception(errorMessage));
                Debug.WriteLine(errorMessage);
            }
        }
    }
}
