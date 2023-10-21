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

        private void LoadFogColor()
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
            }
        }

        private void SaveFogColor(Color color)
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

        private void colorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (colorPicker.SelectedColor.HasValue)
            {
                Color selectedColor = colorPicker.SelectedColor.Value;

                // Update the TextBlock text with the RGB values of the selected color
                tb_PickColor.Text = $"{selectedColor.R / 255.0f}, {selectedColor.G / 255.0f}, {selectedColor.B / 255.0f}";

                // Update the TextBlock foreground color to the selected color
                tb_PickColor.Foreground = new SolidColorBrush(selectedColor);
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

                // Now you can use the 'color' variable wherever you need it
                // For example, you might want to display it in a TextBox or use it in your GLSL file
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
    }
}