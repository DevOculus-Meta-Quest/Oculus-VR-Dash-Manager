using Pfim;
using Microsoft.Win32;
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
using OVR_Dash_Manager.Functions;

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
                MessageBox.Show("Please select valid input and output files.");
                return;
            }

            try
            {
                ImageConverter.ConvertPngToDds(inputFilePath, outputFilePath);
                MessageBox.Show("Conversion to DDS completed successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred during conversion: {ex.Message}");
            }
        }

        private void ConvertToPNG_Click(object sender, RoutedEventArgs e)
        {
            string inputFilePath = txtInputFilePath.Text;
            string outputFilePath = txtOutputFilePath.Text;

            if (string.IsNullOrWhiteSpace(inputFilePath) || string.IsNullOrWhiteSpace(outputFilePath))
            {
                MessageBox.Show("Please select valid input and output files.");
                return;
            }

            try
            {
                ImageConverter.ConvertDdsToPng(inputFilePath, outputFilePath);
                MessageBox.Show("Conversion to PNG completed successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred during conversion: {ex.Message}");
            }
        }

        private void BrowseInputFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.dds)|*.png;*.dds|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                txtInputFilePath.Text = openFileDialog.FileName;
            }
        }

        private void BrowseOutputFile_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "DDS files (*.dds)|*.dds|PNG files (*.png)|*.png|All files (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                txtOutputFilePath.Text = saveFileDialog.FileName;
            }
        }
    }
}
