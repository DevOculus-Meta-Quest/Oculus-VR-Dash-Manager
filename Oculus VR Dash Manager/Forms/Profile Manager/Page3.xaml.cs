using System.Collections.Generic;
using System.Windows.Controls;

namespace OVR_Dash_Manager.Forms.Profile_Manager
{
    public partial class Page3 : Page
    {
        private frm_ProfileManager profileManager;

        public Page3(frm_ProfileManager profileManager)
        {
            InitializeComponent();
            this.profileManager = profileManager;
        }

        public Dictionary<string, object> GetPageControlsState()
        {
            return new Dictionary<string, object>
            {
                { "chkAdaptive", chkAdaptive.IsChecked },
                { "chkDepth", chkDepth.IsChecked },
                { "chkMedian", chkMedian.IsChecked },
                { "chkPhase45", chkPhase45.IsChecked },
                { "chkReverseMapWithCPU", chkReverseMapWithCPU.IsChecked },
                { "chkSimpleRasterizer", chkSimpleRasterizer.IsChecked },
                { "chkThreadCPUReverseMap", chkThreadCPUReverseMap.IsChecked },

                // ... (add more controls as needed)
            };
        }

        public void PopulateUI(Dictionary<string, object> profileData)
        {
            if (profileData != null && profileData.TryGetValue("Page3", out object pageDataObj))
            {
                if (pageDataObj is Dictionary<string, object> pageData)
                {
                    // Applying saved state to each control
                    foreach (var controlState in pageData)
                    {
                        string controlName = controlState.Key;
                        object controlValue = controlState.Value;

                        switch (controlName)
                        {
                            case "chkAdaptive":
                                chkAdaptive.IsChecked = (bool?)controlValue;
                                break;

                            case "cmbOverrideFocusedApp":
                                cmbOverrideFocusedApp.SelectedItem = FindComboBoxItemByContent(cmbOverrideFocusedApp, (string)controlValue);
                                break;

                            case "chkDepth":
                                chkDepth.IsChecked = (bool?)controlValue;
                                break;

                            case "chkMedian":
                                chkMedian.IsChecked = (bool?)controlValue;
                                break;

                            case "chkPhase45":
                                chkPhase45.IsChecked = (bool?)controlValue;
                                break;
                                // Continue adding cases for other controls as needed
                        }
                    }
                }
            }
        }

        // Helper method to find a ComboBoxItem by its content
        private ComboBoxItem FindComboBoxItemByContent(ComboBox comboBox, string content)
        {
            foreach (ComboBoxItem item in comboBox.Items)
            {
                if (item.Content.ToString() == content)
                {
                    return item;
                }
            }
            return null;
        }

        private void cmbASWOptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cmb = sender as ComboBox;
            string selectedOption = (cmb.SelectedItem as ComboBoxItem).Content.ToString();

            switch (selectedOption)
            {
                case "Auto ASW Operation":
                    // Execute code for Auto ASW Operation here
                    // asw.Auto();
                    break;

                case "Simulate 18 Hz Display":
                    // Execute code for Simulate 18 Hz Display here
                    // asw.Clock18();
                    break;

                case "Simulate 30 Hz Display":
                    // Execute code for Simulate 30 Hz Display here
                    // asw.Clock30();
                    break;

                case "Simulate 45 Hz Display":
                    // Execute code for Simulate 45 Hz Display here
                    // asw.Clock45();
                    break;
            }
        }
    }
}