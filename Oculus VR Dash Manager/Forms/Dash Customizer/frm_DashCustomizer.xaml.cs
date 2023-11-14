using OVR_Dash_Manager.Functions;
using OVR_Dash_Manager.Software;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using Color = System.Windows.Media.Color;
using ImageConverter = OVR_Dash_Manager.Functions.ImageConverter;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
using SaveFileDialog = System.Windows.Forms.SaveFileDialog;

namespace OVR_Dash_Manager.Forms.Dash_Customizer
{
    /// <summary>
    /// Interaction logic for frm_DashCustomizer.xaml
    /// </summary>
    public partial class frm_DashCustomizer : Window
    {
        public frm_DashCustomizer()
        {
            InitializeComponent();
            LoadColorValues();
        }

        private void ConvertToDDS_Click(object sender, RoutedEventArgs e)
        {
            string inputFilePath = txtInputFilePath.Text;
            string outputFilePath = txtOutputFilePath.Text;

            if (string.IsNullOrWhiteSpace(inputFilePath) || string.IsNullOrWhiteSpace(outputFilePath))
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Please select valid input and output files.");
                return;
            }

            try
            {
                ImageConverter.ConvertPngToDds(inputFilePath, outputFilePath);
                Xceed.Wpf.Toolkit.MessageBox.Show("Conversion to DDS completed successfully.");
            }
            catch (Exception ex)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show($"An error occurred during conversion: {ex.Message}");
            }
        }

        private void ConvertToPNG_Click(object sender, RoutedEventArgs e)
        {
            string inputFilePath = txtInputFilePath.Text;
            string outputFilePath = txtOutputFilePath.Text;

            if (string.IsNullOrWhiteSpace(inputFilePath) || string.IsNullOrWhiteSpace(outputFilePath))
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Please select valid input and output files.");
                return;
            }

            try
            {
                ImageConverter.ConvertDdsToPng(inputFilePath, outputFilePath);
                Xceed.Wpf.Toolkit.MessageBox.Show("Conversion to PNG completed successfully.");
            }
            catch (Exception ex)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show($"An error occurred during conversion: {ex.Message}");
            }
        }

        private void BrowseInputFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.dds)|*.png;*.dds|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) // Updated this line
            {
                txtInputFilePath.Text = openFileDialog.FileName;
            }
        }

        private void BrowseOutputFile_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "DDS files (*.dds)|*.dds|PNG files (*.png)|*.png|All files (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) // Updated this line
            {
                txtOutputFilePath.Text = saveFileDialog.FileName;
            }
        }

        private void LoadColorValues()
        {
            try
            {
                string glslPath = Oculus.GetTheVoidUniformsPath();
                string glslContent = File.ReadAllText(glslPath);
                Match match = Regex.Match(glslContent, @"vec3 u_voidFogColor = \{(.*?)\};");
                if (match.Success)
                {
                    string[] colors = match.Groups[1].Value.Split(',');
                    Color fogColor = Color.FromRgb(
                    (byte)(float.Parse(colors[0]) * 255),
                    (byte)(float.Parse(colors[1]) * 255),
                    (byte)(float.Parse(colors[2]) * 255));

                    colorPicker.SelectedColor = fogColor;

                    // Update the TextBlocks with the loaded values
                    tb_PickColor.Text = $"{fogColor.R / 255.0f}, {fogColor.G / 255.0f}, {fogColor.B / 255.0f}";
                    tb_RawColor.Text = $"{fogColor.R}, {fogColor.G}, {fogColor.B}";

                    // Set the foreground color of the TextBlocks to the loaded color
                    tb_PickColor.Foreground = new SolidColorBrush(fogColor);
                    tb_RawColor.Foreground = new SolidColorBrush(fogColor);
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex, "An error occurred while loading color values.");
                Xceed.Wpf.Toolkit.MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private void SaveFogColor(Color color)
        {
            try
            {
                string glslPath = Oculus.GetTheVoidUniformsPath();
                string glslContent = File.ReadAllText(glslPath);
                float r = color.R / 255.0f;
                float g = color.G / 255.0f;
                float b = color.B / 255.0f;
                string newFogColor = $"vec3 u_voidFogColor = {{{r}, {g}, {b}}};";
                string modifiedGlslContent = Regex.Replace(glslContent, @"vec3 u_voidFogColor = \{.*?\};", newFogColor);
                File.WriteAllText(glslPath, modifiedGlslContent);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex, "An error occurred while saving fog color.");
                Xceed.Wpf.Toolkit.MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private void colorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (colorPicker.SelectedColor.HasValue)
            {
                Color selectedColor = colorPicker.SelectedColor.Value;

                // Calculate the GLSL values
                float r = selectedColor.R / 255.0f;
                float g = selectedColor.G / 255.0f;
                float b = selectedColor.B / 255.0f;

                // Update the TextBlock text with the GLSL values of the selected color with three decimal places
                tb_PickColor.Text = $"{r:F3}, {g:F3}, {b:F3}";

                // Update the TextBlock text with the raw RGB values of the selected color, ensuring three digits
                tb_RawColor.Text = $"{selectedColor.R:D3}, {selectedColor.G:D3}, {selectedColor.B:D3}";

                // Update the TextBlock foreground color to the selected color
                tb_PickColor.Foreground = new SolidColorBrush(selectedColor);
                tb_RawColor.Foreground = new SolidColorBrush(selectedColor);
            }
        }

        private void btnOpenColorPicker_Click(object sender, RoutedEventArgs e)
        {
            // Create and configure the ColorPicker dialog
            System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // Get the selected color
                System.Drawing.Color selectedColor = colorDialog.Color;

                // Convert System.Drawing.Color to System.Windows.Media.Color
                Color color = Color.FromArgb(selectedColor.A, selectedColor.R, selectedColor.G, selectedColor.B);

                // Set the selected color to the colorPicker, this will trigger the colorPicker_SelectedColorChanged event
                colorPicker.SelectedColor = color;
            }
        }

        private void btnSaveColor_Click(object sender, RoutedEventArgs e)
        {
            if (colorPicker.SelectedColor.HasValue)
            {
                Color selectedColor = colorPicker.SelectedColor.Value;

                // Calculate the GLSL values
                float r = selectedColor.R / 255.0f;
                float g = selectedColor.G / 255.0f;
                float b = selectedColor.B / 255.0f;

                // Save the color to the GLSL file
                SaveFogColor(selectedColor);

                // Optionally, show a message to the user
                Xceed.Wpf.Toolkit.MessageBox.Show("Color saved to the GLSL file successfully.");
            }
            else
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Please select a color first.");
            }
        }

        private void btn_FloorGrid_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Image files (*.dds, *.png)|*.dds;*.png|All files (*.*)|*.*";
                openFileDialog.Title = "Select a DDS or PNG file";
                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    // Check if the file exists and is accessible
                    if (File.Exists(openFileDialog.FileName))
                    {
                        txt_FloorGrid.Text = openFileDialog.FileName;
                    }
                    else
                    {
                        Xceed.Wpf.Toolkit.MessageBox.Show("The selected file does not exist or is not accessible.");
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex, "An error occurred while selecting the file.");
                Xceed.Wpf.Toolkit.MessageBox.Show($"An error occurred while selecting the file: {ex.Message}");
            }
        }

        private void btn_ReplaceFloorGrid_Click(object sender, RoutedEventArgs e)
        {
            string selectedFilePath = txt_FloorGrid.Text;
            if (string.IsNullOrWhiteSpace(selectedFilePath) || !File.Exists(selectedFilePath))
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Please select a valid file.");
                return;
            }

            try
            {
                string oculusFilePath = Oculus.GetGridPlanePath();

                // Creating a backup of the current file
                string backupFilePath = oculusFilePath + ".backup";
                if (File.Exists(backupFilePath))
                {
                    File.Delete(backupFilePath); // Delete the existing backup file if it exists
                }
                File.Copy(oculusFilePath, backupFilePath); // Create a new backup file

                // Check if the selected file is a PNG
                if (Path.GetExtension(selectedFilePath).ToLower() == ".png")
                {
                    // Convert PNG to DDS
                    string tempDdsPath = Path.Combine(Path.GetTempPath(), "converted.dds");
                    ImageConverter.ConvertPngToDds(selectedFilePath, tempDdsPath);
                    selectedFilePath = tempDdsPath; // Update the selected file path to the converted DDS file
                }

                // Replacing the file
                File.Copy(selectedFilePath, oculusFilePath, overwrite: true);
                Xceed.Wpf.Toolkit.MessageBox.Show("File replaced successfully. A backup of the original file has been created.");
            }
            catch (IOException ioEx)
            {
                ErrorLogger.LogError(ioEx, "An IO error occurred while replacing the file.");
                Xceed.Wpf.Toolkit.MessageBox.Show($"An IO error occurred: {ioEx.Message}");
            }
            catch (UnauthorizedAccessException uaEx)
            {
                ErrorLogger.LogError(uaEx, "Access denied to one of the files.");
                Xceed.Wpf.Toolkit.MessageBox.Show($"Access denied: {uaEx.Message}");
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex, "An unexpected error occurred while replacing the file.");
                Xceed.Wpf.Toolkit.MessageBox.Show($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}