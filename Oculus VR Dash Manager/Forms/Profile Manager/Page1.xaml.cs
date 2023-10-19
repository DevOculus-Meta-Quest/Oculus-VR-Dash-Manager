using OVR_Dash_Manager.Functions;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace OVR_Dash_Manager.Forms.Profile_Manager
{
    public partial class Page1 : Page
    {
        private OculusDebugToolFunctions oculusDebugTool;

        public Page1()
        {
            InitializeComponent();
            oculusDebugTool = new OculusDebugToolFunctions();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            oculusDebugTool.Dispose();
        }

        private async void CheckBox_Changed(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            if (checkBox == null) return;

            string checkBoxName = checkBox.Name.Replace("_", "."); // Replacing underscore with period
            bool isChecked = checkBox.IsChecked == true;

            Debug.WriteLine($"{checkBoxName} CheckBox Changed: {isChecked}");

            await oculusDebugTool.ExecuteCommandAsync(isChecked ? $"server:{checkBoxName}" : $"server:no{checkBoxName}");
        }

        private async void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox == null) return;

            string comboBoxName = comboBox.Name;
            int selectedIndex = comboBox.SelectedIndex;

            Debug.WriteLine($"{comboBoxName} ComboBox Selection Changed: {selectedIndex}");

            string command = comboBoxName switch
            {
                "cmb_pass_guard" => $"server:pass.guard {selectedIndex}",
                "cmb_pass_mixreality" => $"server:pass.mixreality {selectedIndex}",
                _ => null
            };

            if (command != null)
            {
                await oculusDebugTool.ExecuteCommandAsync(command);
            }
            else
            {
                Debug.WriteLine($"No command mapping found for combobox: {comboBoxName}");
            }
        }
    }
}
