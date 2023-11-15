using Microsoft.VisualBasic;
using Microsoft.Win32;
using OVR_Dash_Manager.Functions;
using OVR_Dash_Manager.Software;
using System;
using System.Windows;
using System.Windows.Input;
using static OVR_Dash_Manager.Software.ADB;

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

        void lstFiles_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lstFiles.SelectedItem != null)
            {
                var selectedPath = lstFiles.SelectedItem.ToString();
                var fullPath = System.IO.Path.Combine(currentDirectory, selectedPath); // Combine the current directory with the selected item

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

        string currentDirectory = "/"; // Start at the root directory

        void RefreshFileList()
        {
            var fileList = ADBFileManager.ListFiles(currentDirectory);
            lstFiles.Items.Clear();

            foreach (var file in fileList.Split('\n'))
                lstFiles.Items.Add(file);
            
        }

        // Call this method when a directory is double-clicked or a navigation button is pressed
        void NavigateToDirectory(string directory)
        {
            currentDirectory = directory; // Update the current directory
            RefreshFileList(); // Refresh the file list based on the new directory
        }

        void BtnRefresh_Click(object sender, RoutedEventArgs e) => RefreshFileList();

        void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var path = GetSelectedPath(); // Assume this method gets the path of the selected file or directory.
            var message = $"Are you sure you want to delete: {path}?";
            var result = MessageBox.Show(message, "Confirmation", MessageBoxButton.YesNo);

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

        void BtnUpload_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                var targetPath = ShowInputDialog("Enter the target directory on the device:");

                if (!string.IsNullOrEmpty(targetPath))
                {
                    ADBFileManager.UploadFile(openFileDialog.FileName, targetPath);
                    RefreshFileList();
                }
            }
        }

        void BtnDownload_Click(object sender, RoutedEventArgs e)
        {
            var fileNameToDownload = ShowInputDialog("Enter the name of the file to download:");

            if (!string.IsNullOrEmpty(fileNameToDownload))
            {
                var saveFileDialog = new SaveFileDialog();

                if (saveFileDialog.ShowDialog() == true)
                {
                    ADBFileManager.DownloadFile($"/path/to/directory/{fileNameToDownload}", saveFileDialog.FileName);
                }
            }
        }

        void BtnCreateDir_Click(object sender, RoutedEventArgs e)
        {
            var newDirName = Interaction.InputBox("Enter the name of the new directory:", "Create Directory");

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

        string ShowInputDialog(string text)
        {
            var input = Microsoft.VisualBasic.Interaction.InputBox(text, "Input", "Default", -1, -1);
            return input;
        }

        string GetSelectedPath()
        {
            if (lstFiles.SelectedItem != null)
            {
                return lstFiles.SelectedItem.ToString();
            }

            return null;
        }

        public static bool IsDirectory(string path)
        {
            var result = ADB.ADBFileManager.ExecuteADBCommand($"shell ls -ld {path}");
            return result.StartsWith("d");
        }
    }
}