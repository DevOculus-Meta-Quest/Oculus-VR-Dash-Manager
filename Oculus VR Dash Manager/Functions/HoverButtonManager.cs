using System;
using System.Diagnostics;
using System.Timers;
using System.Windows;
using System.Windows.Controls;

namespace OVR_Dash_Manager
{
    public class HoverButtonManager
    {
        private MainWindow _mainWindow;
        private ProgressBar _pbNormal;
        private ProgressBar _pbExit;
        private Action _activateDash;
        public Hover_Button Oculus_Dash { get; private set; }
        public Hover_Button Exit_Link { get; private set; }

        public HoverButtonManager(MainWindow mainWindow, ProgressBar pbNormal, ProgressBar pbExit, Action activateDash)
        {
            _mainWindow = mainWindow;
            _pbNormal = pbNormal;
            _pbExit = pbExit;
            _activateDash = activateDash;

            GenerateHoverButtons();
        }

        // Add this method to set the _activateDash action after the instance has been created
        public void SetActivateDashAction(Action activateDashAction)
        {
            _activateDash = activateDashAction;
        }

        public void GenerateHoverButtons()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Oculus_Dash = new Hover_Button
                {
                    Hover_Complete_Action = OculusDashHoverActivate,
                    Bar = _pbNormal,
                    Check_SteamVR = true,
                    Hovered_Seconds_To_Activate = Properties.Settings.Default.Hover_Activation_Time
                };
                Exit_Link = new Hover_Button
                {
                    Hover_Complete_Action = ExitLinkHoverActivate,
                    Bar = _pbExit,
                    Check_SteamVR = true,
                    Hovered_Seconds_To_Activate = Properties.Settings.Default.Hover_Activation_Time
                };
                _pbNormal.Maximum = Properties.Settings.Default.Hover_Activation_Time * 1000;
                _pbExit.Maximum = Properties.Settings.Default.Hover_Activation_Time * 1000;
            });
        }

        public void CheckHover(object sender, ElapsedEventArgs args)
        {
            CheckHovering(Oculus_Dash);
            CheckHovering(Exit_Link);
        }

        public void EnableHoverButton(Dashes.Dash_Type Dash)
        {
            switch (Dash)
            {
                case Dashes.Dash_Type.Exit:
                    Exit_Link.Enabled = true;
                    break;
                case Dashes.Dash_Type.Normal:
                    Oculus_Dash.Enabled = true;
                    break;
            }
        }

        public void ResetHoverButtons()
        {
            Oculus_Dash.Reset();
            Exit_Link.Reset();
        }

        public void OculusDashHoverActivate()
        {
            Debug.WriteLine("OculusDashHoverActivate is about to be called");

            try
            {
                Debug.WriteLine("OculusDashHoverActivate called");

                // Check if the current thread is the UI thread
                if (Application.Current.Dispatcher.CheckAccess())
                {
                    Oculus_Dash.Bar.Value = 0;
                }
                else
                {
                    // If not, dispatch the UI update to the UI thread
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Oculus_Dash.Bar.Value = 0;
                    });
                }

                _activateDash.Invoke();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception occurred: {ex.Message}");
                Debug.WriteLine(ex.StackTrace);
                // Handle exception or rethrow if necessary
            }
        }


        public void ExitLinkHoverActivate()
        {
            Exit_Link.Bar.Value = 0;
            Software.Steam.Close_SteamVR_ResetLink();
        }

        public void UpdateDashButtons()
        {
            foreach (UIElement item in _mainWindow.gd_DashButtons.Children)
            {
                if (item is Button button)
                {
                    if (button.Tag is Dashes.Dash_Type Dash)
                    {
                        bool Enabled = Dashes.Dash_Manager.IsInstalled(Dash);
                        button.IsEnabled = Enabled;
                        if (Enabled)
                            EnableHoverButton(Dash);
                    }
                }
            }
            _mainWindow.btn_ExitOculusLink.IsEnabled = true;
        }

        private void CheckHovering(Hover_Button hoverButton)
        {
            if (hoverButton.IsHovering())
            {
                hoverButton.UpdateHoverState();
            }
            else
            {
                hoverButton.Reset();
            }
        }

        public void ActivateDash()
        {
            // TODO: Implement dash activation logic
            System.Diagnostics.Debug.WriteLine("ActivateDash called!");
        }

    }
}
