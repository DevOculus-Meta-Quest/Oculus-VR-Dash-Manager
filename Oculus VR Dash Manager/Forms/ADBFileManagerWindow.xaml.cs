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
using System.IO;
using System.Windows.Shapes;
using Microsoft.VisualBasic;
using static OVR_Dash_Manager.Software.ADB;
using OVR_Dash_Manager.Software;
using YOVR_Dash_Manager.Functions;

namespace OVR_Dash_Manager.Forms
{
    /// <summary>
    /// Interaction logic for ADBFileManagerWindow.xaml
    /// </summary>
    public partial class ADBFileManagerWindow : Window
    {
        public ADBFileManagerWindow()
        {
            InitializeComponent();
            RefreshFileList();
        }

        private void lstFiles_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lstFiles.SelectedItem != null)
            {
                string selectedPath = lstFiles.SelectedItem.ToString();
                string fullPath = System.IO.Path.Combine(currentDirectory, selectedPath); // Combine the current directory with the selected item

                // Check if the selected item is a directory
                if (IsDirectory(fullPath))
                {
                    NavigateToDirectory(fullPath); // Navigate to the directory
                }
                else
                {
                    // Handle the file here, e.g., open or download the file
                    MessageBox.Show($"Selected file: {fullPath}");
                }
            }
        }

        private string currentDirectory = "/"; // Start at the root directory

        private void RefreshFileList()
        {
            string fileList = ADBFileManager.ListFiles(currentDirectory);
            lstFiles.Items.Clear();
            foreach (var file in fileList.Split('\n'))
            {
                lstFiles.Items.Add(file);
            }
        }

        // Call this method when a directory is double-clicked or a navigation button is pressed
        private void NavigateToDirectory(string directory)
        {
            currentDirectory = directory; // Update the current directory
            RefreshFileList(); // Refresh the file list based on the new directory
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshFileList();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            string path = GetSelectedPath(); // Assume this method gets the path of the selected file or directory.
            string message = $"Are you sure you want to delete: {path}?";
            MessageBoxResult result = MessageBox.Show(message, "Confirmation", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    ADB.ADBFileManager.DeleteFile(path);
                    MessageBox.Show("Delete operation successful.");
                    RefreshFileList();
                }
                catch (Exception ex)
                {
                    ErrorLogger.LogError(ex);
                    MessageBox.Show("An error occurred during the delete operation.");
                }
            }
        }

        private void BtnUpload_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                string targetPath = ShowInputDialog("Enter the target directory on the device:");
                if (!string.IsNullOrEmpty(targetPath))
                {
                    ADBFileManager.UploadFile(openFileDialog.FileName, targetPath);
                    RefreshFileList();
                }
            }
        }

        private void BtnDownload_Click(object sender, RoutedEventArgs e)
        {
            string fileNameToDownload = ShowInputDialog("Enter the name of the file to download:");
            if (!string.IsNullOrEmpty(fileNameToDownload))
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                if (saveFileDialog.ShowDialog() == true)
                {
                    ADBFileManager.DownloadFile($"/path/to/directory/{fileNameToDownload}", saveFileDialog.FileName);
                }
            }
        }

        private void BtnCreateDir_Click(object sender, RoutedEventArgs e)
        {
            string newDirName = Interaction.InputBox("Enter the name of the new directory:", "Create Directory");

            if (!string.IsNullOrEmpty(newDirName))
            {
                try
                {
                    ADB.ADBFileManager.CreateDirectory(newDirName);
                }
                catch (Exception ex)
                {
                    ErrorLogger.LogError(ex);
                }
            }
        }

        private string ShowInputDialog(string text)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox(text, "Input", "Default", -1, -1);
            return input;
        }

        private string GetSelectedPath()
        {
            if (lstFiles.SelectedItem != null)
            {
                return lstFiles.SelectedItem.ToString();
            }
            return null;
        }
        public static bool IsDirectory(string path)
        {
            string result = ADB.ADBFileManager.ExecuteADBCommand($"shell ls -ld {path}");
            return result.StartsWith("d");
        }
    }
}
