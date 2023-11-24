using System.Linq;
using System.Windows;
using OVR_Dash_Manager.Functions;

namespace OVR_Dash_Manager.Forms.Auto_Program_Launch
{
    /// <summary>
    /// Interaction logic for frm_Auto_Program_Launch_Settings.xaml
    /// </summary>
    public partial class frm_Auto_Program_Launch_Settings : Window
    {
        // Flag to track if any programs were removed during the session
        private bool Programs_Removed;

        public frm_Auto_Program_Launch_Settings() => InitializeComponent();

        // Event handler for adding a program
        private void btn_Add_Program_Click(object sender, RoutedEventArgs e)
        {
            // Open a file dialog and get the selected file path
            var FilePath = Functions.FileExplorerUtilities.OpenSingle();

            // If a file was selected, add it to the program list and refresh the UI
            if (!string.IsNullOrEmpty(FilePath))
            {
                Auto_Launch_Programs.Add_New_Program(FilePath);
                lv_Programs.Items.Refresh();
            }
        }

        // Event handler for removing a program
        private void btn_Remove_Program_Click(object sender, RoutedEventArgs e)
        {
            // Check if a program is selected in the UI
            if (lv_Programs.SelectedItem is Auto_Program Program)
            {
                Programs_Removed = true;
                Auto_Launch_Programs.Remove_Program(Program);
                lv_Programs.Items.Refresh();
            }
        }

        // Event handler for window load event
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Bind the program list to the UI and refresh
            lv_Programs.ItemsSource = Auto_Launch_Programs.Programs;
            lv_Programs.Items.Refresh();
        }

        // Event handler for window closing event
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Check if any programs were removed or changed during the session
            var Changed = Programs_Removed ||
                           (Auto_Launch_Programs.Programs?.Any(p => p.Changed) == true);

            // If changes were made, confirm with the user whether to save them
            if (Changed)
            {
                if (MessageBox.Show(this,
                                    "Programs Have Been Changed - Are you Sure you want to save changes ?",
                                    "Confirm Saving Changes",
                                    MessageBoxButton.YesNo,
                                    MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    // Save the changes
                    Auto_Launch_Programs.Save_Program_List();
                }
                else
                {
                    // Discard the changes
                    Auto_Launch_Programs.Generate_List();
                }
            }
        }

        // Event handler for opening the program folder
        private void btn_Open_Program_Folder_Click(object sender, RoutedEventArgs e)
        {
            // Check if a program is selected in the UI
            if (lv_Programs.SelectedItem is Auto_Program Program)
            {
                // Open the program's folder in File Explorer
                Functions.ProcessFunctions.StartProcess("explorer.exe", Program.Folder_Path);
            }
        }
    }
}