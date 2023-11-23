using System;
using System.Timers;
using System.Windows;
using OVR_Dash_Manager.Functions;
using OVR_Dash_Manager.Functions.Oculus;
using OVR_Dash_Manager.Functions.Steam;

namespace OVR_Dash_Manager.Forms
{
    /// <summary>
    /// Interaction logic for frm_TestWindow.xaml
    /// </summary>
    public partial class frm_TestWindow : Window
    {
        public frm_TestWindow() => InitializeComponent();

        private void AddToReadOut(string Text)
        {
            UIManager.DoAction(this, new Action(delegate () { txtbx_ReadOut.AppendText(Text + "\r\n"); txtbx_ReadOut.ScrollToEnd(); }));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TimerManager.CreateTimer("Test_Function", TimeSpan.FromSeconds(1), Test_Function);
            TimerManager.StartTimer("Test_Function");
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            TimerManager.StopTimer("Test_Function");
            TimerManager.DisposeTimer("Test_Function");
        }

        private void Test_Function(object sender, ElapsedEventArgs args)
        {
        }

        private void btn_ChangeSteamRunTime_Click(object sender, RoutedEventArgs e)
        {
            Steam_VR_Settings.Set_SteamVR_Runtime();
        }

        private void btn_ChangeOculusRunTime_Click(object sender, RoutedEventArgs e)
        {
            Oculus_Link.SetToOculusRunTime();
        }
    }
}