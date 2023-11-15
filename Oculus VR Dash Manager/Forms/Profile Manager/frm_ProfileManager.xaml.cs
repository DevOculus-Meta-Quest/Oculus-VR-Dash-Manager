using Microsoft.Win32;
using Newtonsoft.Json;
using OVR_Dash_Manager.Functions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
            profileManager = profileManager; // Save the reference to the frm_ProfileManager
            github = new Github();
            oculusDebugToolFunctions = new OculusDebugToolFunctions();
            LoadScriptsAsync();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
                {
                    LoadRegistrySettings();
                }), System.Windows.Threading.DispatcherPriority.ContextIdle);
        }

        private void LoadRegistrySettings()
        {
            var keyLocation = @"Software\Oculus\RemoteHeadset";

            using (var key = OVR_Dash_Manager.Functions.RegistryFunctions.GetRegistryKey(OVR_Dash_Manager.Functions.RegistryKeyType.CurrentUser, keyLocation))
            {
                if (key != null)
                {
                    // Distortion Curvature
                    var distortionCurvatureValue = key.GetValue("DistortionCurve", "Default").ToString();
                    SetComboBoxSelection(cb_DistortionCurvature, distortionCurvatureValue);

                    // Video Codec
                    var videoCodecValue = key.GetValue("HEVC", "Default").ToString();
                    SetComboBoxSelection(cb_VideoCodec, videoCodecValue);

                    // Sliced Encoding
                    var slicedEncodingValue = key.GetValue("NumSlices", "Default").ToString();
                    SetComboBoxSelection(cb_slicedEncoding, slicedEncodingValue);

                    // Encode Resolution Width
                    var encodeResolutionWidthValue = key.GetValue("EncodeWidth", "0").ToString();
                    txt_EncodeResolutionWidth.Text = encodeResolutionWidthValue;

                    // Encode Dynamic Bitrate
                    var encodeDynamicBitrateValue = key.GetValue("DBR", "Default").ToString();
                    SetComboBoxSelection(cb_EncodeDynamicBitrate, encodeDynamicBitrateValue);

                    // Dynamic Bitrate Max
                    var dynamicBitrateMaxValue = key.GetValue("DBRMax", "0").ToString();
                    txt_DynamicBitrateMax.Text = dynamicBitrateMaxValue;

                    // Encode Bitrate (Mbps)
                    var encodeBitrateValue = key.GetValue("BitrateMbps", "0").ToString();
                    txt_EncodeBitrate.Text = encodeBitrateValue;

                    // Dynamic Bitrate Offset (Mbps)
                    var dynamicBitrateOffsetValue = key.GetValue("DBROffsetMbps", "0").ToString();
                    txt_DynamicBitrateOffset.Text = dynamicBitrateOffsetValue;

                    // Link Sharpening
                    var linkSharpeningValue = key.GetValue("LinkSharpeningEnabled", "Disabled").ToString();
                    SetComboBoxSelection(cb_LinkSharpening, linkSharpeningValue);

                    // Local Dimming
                    var localDimmingValue = key.GetValue("LocalDimming", "Disabled").ToString();
                    SetComboBoxSelection(cb_LocalDimming, localDimmingValue);

                    // ... Add additional controls and registry keys as needed
                }
                else
                {
                    // Log the error or inform the user that the registry key could not be opened
                    ErrorLogger.LogError(new Exception("Unable to open the registry key at " + keyLocation));
                }
            }
        }

        private void SetComboBoxSelection(ComboBox comboBox, string registryValue)
        {
            var valueToSelect = registryValue;

            // Translate registry value to ComboBox item content if necessary
            switch (comboBox.Name)
            {
                case "cb_DistortionCurvature":
                    // Map the registry values to the ComboBox items
                    valueToSelect = registryValue switch
                    {
                        "0" => "Low",
                        "1" => "High",
                        _ => "Default" // Default or unknown value
                    };
                    break;

                case "cb_VideoCodec":
                    // Map the registry values to the ComboBox items
                    valueToSelect = registryValue switch
                    {
                        "0" => "H.264",
                        "1" => "H.265",
                        _ => "Default" // Default or unknown value
                    };
                    break;

                case "cb_slicedEncoding":
                    // Map the registry values to the ComboBox items
                    valueToSelect = registryValue switch
                    {
                        "5" => "On",
                        "1" => "Off",
                        _ => "Default" // Default or unknown value
                    };
                    break;

                case "cb_LinkSharpening":
                    // Update the switch case to match the correct registry values
                    valueToSelect = registryValue switch
                    {
                        "1" => "Disabled",
                        "2" => "Normal",
                        "3" => "Quality",
                        _ => "Disabled" // Default or unknown value
                    };
                    break;

                case "cb_LocalDimming":
                    // Map the registry values to the ComboBox items
                    valueToSelect = registryValue switch
                    {
                        "0" => "Disabled",
                        "1" => "Enabled",
                        _ => "Disabled" // Default or unknown value
                    };
                    break;
                    // Add cases for other ComboBoxes if needed
            }

            // Now find and select the ComboBoxItem with the matching content
            var itemFound = false;

            foreach (ComboBoxItem item in comboBox.Items)
            {
                if (item.Content.ToString() == valueToSelect)
                {
                    // Output the information to the Output window in Visual Studio
                    Debug.WriteLine($"Setting {comboBox.Name} to {valueToSelect}");

                    comboBox.SelectedItem = item;
                    itemFound = true;
                    break; // Exit the loop once the match is found and selected
                }
            }

            // If no match was found, select the default item and log an error
            if (!itemFound)
            {
                comboBox.SelectedIndex = 0; // Default to the first item if no match is found
                ErrorLogger.LogError(new Exception($"No matching ComboBoxItem found for value '{valueToSelect}' in ComboBox '{comboBox.Name}'."));
            }
        }

        private async void LoadScriptsAsync()
        {
            try
            {
                var jsonResponse = await github.GetFilesFromDirectoryAsync("DevOculus-Meta-Quest", "OVRDM-Profile-Scripts", "OVRDM-Profile-Scripts");

                // Deserialize the JSON response
                var files = JsonConvert.DeserializeObject<List<GitHubFile>>(jsonResponse);

                scriptsListView.Items.Clear();

                foreach (var file in files)
                    scriptsListView.Items.Add(file.name);
                // Assuming each file object has a 'name' property
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

        private void RefreshButton_Click(object sender, RoutedEventArgs e) => LoadScriptsAsync();

        private async void scriptsListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedItem = scriptsListView.SelectedItem as string;

            if (!string.IsNullOrEmpty(selectedItem))
            {
                try
                {
                    var scriptUrl = await github.GetFileDownloadUrlAsync("DevOculus-Meta-Quest", "OVRDM-Profile-Scripts", $"OVRDM-Profile-Scripts/{selectedItem}");

                    if (scriptUrl != null)
                    {
                        // Download the script content and save it to a temporary file
                        var tempFilePath = await DownloadAndSaveTempFileAsync(scriptUrl);

                        // Read and display the contents of the temporary file
                        var fileContents = File.ReadAllText(tempFilePath);

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
            using var httpClient = new HttpClient();

            var response = await httpClient.GetAsync(downloadUrl);

            if (response.IsSuccessStatusCode)
            {
                var fileContent = await response.Content.ReadAsStringAsync();

                var tempFilePath = System.IO.Path.GetTempFileName();

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
            var helpWindow = new Profile_Manager.ProfileManagerHelp();

            // Set the Topmost property to make sure it appears above all other windows
            helpWindow.Topmost = true;

            // Show the help window as a dialog
            helpWindow.ShowDialog();
        }

        private void cb_DistortionCurvature_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            // Check if SelectedItem is not null before accessing its Content
            if (comboBox.SelectedItem != null)
            {
                var selectedValue = ((ComboBoxItem)comboBox.SelectedItem).Content.ToString();

                var keyLocation = @"Software\Oculus\RemoteHeadset";
                var keyName = "DistortionCurve";

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
                        var errorMessage = "Unable to open the registry key at " + keyLocation;
                        ErrorLogger.LogError(new Exception(errorMessage));
                        // Optionally, attempt to create the key if it should exist
                        // This depends on the application's requirements and permissions
                        try
                        {
                            var newKey = RegistryFunctions.CreateRegistryKey(OVR_Dash_Manager.Functions.RegistryKeyType.CurrentUser, keyLocation);

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
            else
            {
                // Handle the case when no item is selected, or set a default value
                // This could be logging an error, setting a default value, etc.
                ErrorLogger.LogError(new Exception("No item is selected in cb_DistortionCurvature."));
            }
        }

        private void cb_VideoCodec_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            // Check if SelectedItem is not null before accessing its Content
            if (comboBox.SelectedItem != null)
            {
                var selectedValue = ((ComboBoxItem)comboBox.SelectedItem).Content.ToString();

                var keyLocation = @"Software\Oculus\RemoteHeadset";
                var keyName = "HEVC";

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
                        var errorMessage = "Unable to open the registry key at " + keyLocation;
                        ErrorLogger.LogError(new Exception(errorMessage));

                        // Optionally, attempt to create the key if it should exist
                        // This depends on the application's requirements and permissions
                        try
                        {
                            var newKey = OVR_Dash_Manager.Functions.RegistryFunctions.CreateRegistryKey(OVR_Dash_Manager.Functions.RegistryKeyType.CurrentUser, keyLocation);

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
            else
            {
                // Handle the case when no item is selected, or set a default value
                // This could be logging an error, setting a default value, etc.
                ErrorLogger.LogError(new Exception("No item is selected in cb_VideoCodec."));
            }
        }

        private void cb_slicedEncoding_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            // Check if SelectedItem is not null before accessing its Content
            if (comboBox.SelectedItem != null)
            {
                var selectedValue = ((ComboBoxItem)comboBox.SelectedItem).Content.ToString();

                var keyLocation = @"Software\Oculus\RemoteHeadset";
                var keyName = "NumSlices";

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
                        var errorMessage = "Unable to open the registry key at " + keyLocation;
                        ErrorLogger.LogError(new Exception(errorMessage));

                        // Optionally, attempt to create the key if it should exist
                        // This depends on the application's requirements and permissions
                        try
                        {
                            var newKey = OVR_Dash_Manager.Functions.RegistryFunctions.CreateRegistryKey(OVR_Dash_Manager.Functions.RegistryKeyType.CurrentUser, keyLocation);

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
            else
            {
                // Handle the case when no item is selected, or set a default value
                // This could be logging an error, setting a default value, etc.
                ErrorLogger.LogError(new Exception("No item is selected in cb_slicedEncoding."));
            }
        }

        private void txt_EncodeResolutionWidth_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // This regex will match any non-digit characters.
            var regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);

            var textBox = sender as TextBox;

            if (!e.Handled && textBox != null)
            {
                // Ensure that the LostFocus event is only attached once.
                textBox.LostFocus -= Txt_EncodeResolutionWidth_LostFocus; // Detach the event in case it was already attached.
                textBox.LostFocus += Txt_EncodeResolutionWidth_LostFocus; // Attach the event.
            }
        }

        private void Txt_EncodeResolutionWidth_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;

            if (textBox != null)
            {
                if (int.TryParse(textBox.Text, out int value))
                {
                    var keyLocation = @"Software\Oculus\RemoteHeadset";
                    var keyName = "EncodeWidth";

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
                            var errorMessage = "Unable to open the registry key at " + keyLocation;
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
            var comboBox = sender as ComboBox;
            var keyLocation = @"Software\Oculus\RemoteHeadset";
            var keyName = "DBR";

            if (comboBox != null)
            {
                // Assuming the ComboBox items are strings. If they are ComboBoxItems, you'll need to extract the string value.
                var selectedValue = comboBox.SelectedItem as string;

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
                                var errorMessage = $"Unexpected selection: {selectedValue}";
                                ErrorLogger.LogError(new Exception(errorMessage));
                                break;
                        }
                    }
                    else
                    {
                        // Log the error using ErrorLogger
                        var errorMessage = "Unable to open the registry key at " + keyLocation;
                        ErrorLogger.LogError(new Exception(errorMessage));
                    }
                }
            }
        }

        private void txt_DynamicBitrateMax_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // This regex will match any non-digit characters.
            var regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);

            var textBox = sender as TextBox;

            if (!e.Handled && textBox != null)
            {
                // Ensure that the LostFocus event is only attached once.
                textBox.LostFocus -= Txt_DynamicBitrateMax_LostFocus; // Detach the event in case it was already attached.
                textBox.LostFocus += Txt_DynamicBitrateMax_LostFocus; // Attach the event.
            }
        }

        private void Txt_DynamicBitrateMax_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;

            if (textBox != null)
            {
                var keyLocation = @"Software\Oculus\RemoteHeadset";
                var keyName = "DBRMax";

                if (long.TryParse(textBox.Text, out long value))
                {
                    using (var key = OVR_Dash_Manager.Functions.RegistryFunctions.GetRegistryKey(OVR_Dash_Manager.Functions.RegistryKeyType.CurrentUser, keyLocation))
                    {
                        if (key != null)
                        {
                            if (value == 0)
                            {
                                // Remove the key if the value is 0
                                key.DeleteValue(keyName, throwOnMissingValue: false);
                            }
                            else if (value > 0 && value <= 9999999999)
                            {
                                // Set the key value to the input value if it's within the valid range
                                // Ensure the value is within the DWORD range before casting
                                if (value <= int.MaxValue)
                                {
                                    var newValue = (int)value;
                                    OVR_Dash_Manager.Functions.RegistryFunctions.SetKeyValue(key, keyName, newValue, RegistryValueKind.DWord);
                                }
                                else
                                {
                                    // Handle the case where the value is larger than int.MaxValue
                                    // You might want to log this or show a message to the user
                                }
                            }
                            // If the value is not in range, do not update the registry.
                        }
                        else
                        {
                            // Log the error using ErrorLogger
                            var errorMessage = "Unable to open the registry key at " + keyLocation;
                            ErrorLogger.LogError(new Exception(errorMessage));
                        }
                    }
                }

                // Unsubscribe from the LostFocus event
                textBox.LostFocus -= Txt_DynamicBitrateMax_LostFocus;
            }
        }

        private void txt_EncodeBitrate_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // This regex will match any non-digit characters.
            var regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);

            var textBox = sender as TextBox;

            if (!e.Handled && textBox != null)
            {
                // Ensure that the LostFocus event is only attached once.
                textBox.LostFocus -= Txt_EncodeBitrate_LostFocus; // Detach the event in case it was already attached.
                textBox.LostFocus += Txt_EncodeBitrate_LostFocus; // Attach the event.
            }
        }

        private void Txt_EncodeBitrate_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;

            if (textBox != null)
            {
                var keyLocation = @"Software\Oculus\RemoteHeadset";
                var keyName = "BitrateMbps";

                if (int.TryParse(textBox.Text, out int value))
                {
                    using (var key = OVR_Dash_Manager.Functions.RegistryFunctions.GetRegistryKey(OVR_Dash_Manager.Functions.RegistryKeyType.CurrentUser, keyLocation))
                    {
                        if (key != null)
                        {
                            if (value == 0)
                            {
                                // Remove the key if the value is 0
                                key.DeleteValue(keyName, throwOnMissingValue: false);
                            }
                            else if (value > 0 && value <= 500)
                            {
                                // Set the key value to the input value if it's within the valid range
                                OVR_Dash_Manager.Functions.RegistryFunctions.SetKeyValue(key, keyName, value, RegistryValueKind.DWord);
                            }
                            // If the value is not in range, do not update the registry.
                        }
                        else
                        {
                            // Log the error using ErrorLogger
                            var errorMessage = "Unable to open the registry key at " + keyLocation;
                            ErrorLogger.LogError(new Exception(errorMessage));
                        }
                    }
                }

                // Unsubscribe from the LostFocus event
                textBox.LostFocus -= Txt_EncodeBitrate_LostFocus;
            }
        }

        private void txt_DynamicBitrateOffset_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // This regex will match any non-digit characters.
            var regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);

            var textBox = sender as TextBox;

            if (!e.Handled && textBox != null)
            {
                // Ensure that the LostFocus event is only attached once.
                textBox.LostFocus -= Txt_DynamicBitrateOffset_LostFocus; // Detach the event in case it was already attached.
                textBox.LostFocus += Txt_DynamicBitrateOffset_LostFocus; // Attach the event.
            }
        }

        private void Txt_DynamicBitrateOffset_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;

            if (textBox != null)
            {
                var keyLocation = @"Software\Oculus\RemoteHeadset";
                var keyName = "DBROffsetMbps";

                if (int.TryParse(textBox.Text, out int value))
                {
                    using (var key = OVR_Dash_Manager.Functions.RegistryFunctions.GetRegistryKey(OVR_Dash_Manager.Functions.RegistryKeyType.CurrentUser, keyLocation))
                    {
                        if (key != null)
                        {
                            if (value == 0)
                            {
                                // Remove the key if the value is 0
                                key.DeleteValue(keyName, throwOnMissingValue: false);
                            }
                            else if (value > 0 && value <= 999999999)
                            {
                                // Set the key value to the input value if it's within the valid range
                                OVR_Dash_Manager.Functions.RegistryFunctions.SetKeyValue(key, keyName, value, RegistryValueKind.DWord);
                            }
                            // If the value is not in range, do not update the registry.
                        }
                        else
                        {
                            // Log the error using ErrorLogger
                            var errorMessage = "Unable to open the registry key at " + keyLocation;
                            ErrorLogger.LogError(new Exception(errorMessage));
                        }
                    }
                }

                // Unsubscribe from the LostFocus event
                textBox.LostFocus -= Txt_DynamicBitrateOffset_LostFocus;
            }
        }

        private void cb_LinkSharpening_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            var keyLocation = @"Software\Oculus\RemoteHeadset";
            var keyName = "LinkSharpeningEnabled";

            // Check the selected item and set the registry value accordingly
            if (comboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                var selectedValue = selectedItem.Content.ToString();

                var valueToSet = selectedValue switch
                {
                    "Disabled" => 1, // Assuming 0 is the correct value for Disabled
                    "Normal" => 2,   // Assuming 1 is the correct value for Normal
                    "Quality" => 3,  // Assuming 2 is the correct value for Quality
                    _ => throw new InvalidOperationException($"Invalid selection for Link Sharpening: {selectedValue}")
                };

                // Update the registry with the new value
                using (var key = OVR_Dash_Manager.Functions.RegistryFunctions.GetRegistryKey(OVR_Dash_Manager.Functions.RegistryKeyType.CurrentUser, keyLocation))
                {
                    if (key != null)
                    {
                        var result = OVR_Dash_Manager.Functions.RegistryFunctions.SetKeyValue(key, keyName, valueToSet, RegistryValueKind.DWord); // Corrected line

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
                        var errorMessage = $"Unable to open the registry key at {keyLocation}";
                        ErrorLogger.LogError(new Exception(errorMessage));
                        Debug.WriteLine(errorMessage);
                    }
                }
            }
            else
            {
                var errorMessage = "The selected item is not a ComboBoxItem.";
                ErrorLogger.LogError(new Exception(errorMessage));
                Debug.WriteLine(errorMessage);
            }
        }

        private void cb_LocalDimming_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            var keyLocation = @"Software\Oculus\RemoteHeadset";
            var keyName = "LocalDimming";

            // Assuming the ComboBox items are ComboBoxItem objects
            if (comboBox.SelectedItem is ComboBoxItem selectedComboBoxItem)
            {
                var selectedItemContent = selectedComboBoxItem.Content.ToString();
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
                                var errorMessage = $"Failed to set {keyName} to {valueToSet.Value}: {ex.Message}";
                                ErrorLogger.LogError(ex, errorMessage);
                                Debug.WriteLine(errorMessage);
                            }
                        }
                        else
                        {
                            // Log the error using ErrorLogger
                            var errorMessage = "Unable to open the registry key at " + keyLocation;
                            ErrorLogger.LogError(new Exception(errorMessage));
                            Debug.WriteLine(errorMessage);
                        }
                    }
                }
                else
                {
                    // Log the error using ErrorLogger
                    var errorMessage = $"Invalid selection for Local Dimming: {selectedItemContent}";
                    ErrorLogger.LogError(new Exception(errorMessage));
                    Debug.WriteLine(errorMessage);
                }
            }
            else
            {
                // Log the error using ErrorLogger
                var errorMessage = "The selected item is not a ComboBoxItem.";
                ErrorLogger.LogError(new Exception(errorMessage));
                Debug.WriteLine(errorMessage);
            }
        }

        private void btn_SteamApps_Click(object sender, RoutedEventArgs e)
        {
            // Create an instance of frm_SteamApps window
            var steamAppsWindow = new frm_SteamApps();

            // Set the frm_OtherTools window as the owner of steamAppsWindow
            steamAppsWindow.Owner = this;

            // Show the window
            steamAppsWindow.Show();
        }

        private void btn_OculusApps_Click(object sender, RoutedEventArgs e)
        {
            // Create an instance of frm_SteamApps window
            var OculusAppsWindow = new frm_OculusApps();

            // Set the frm_OtherTools window as the owner of steamAppsWindow
            OculusAppsWindow.Owner = this;

            // Show the window
            OculusAppsWindow.Show();
        }
    }
}