using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace OVR_Dash_Manager.Forms.Profile_Manager
{
    /// <summary>
    /// Interaction logic for frm_ProfileManager.xaml
    /// </summary>
    public partial class frm_ProfileManager : Window
    {
        public frm_ProfileManager()
        {
            InitializeComponent();
            sldXRay.ValueChanged += SldXRay_ValueChanged;
        }

        private void SldXRay_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            lblXRayValue.Content = $"XRay Value: {sldXRay.Value}";
        }

        private void btnASWAuto_Click(object sender, RoutedEventArgs e)
        {
            ExecuteCommand("server:asw.Auto");
        }

        private void ExecuteCommand(string command)
        {
            // Execute the command using OculusDebugToolCLI.exe
            Process.Start("OculusDebugToolCLI.exe", command);
        }

        // Add more event handlers here for each control
    }
}
