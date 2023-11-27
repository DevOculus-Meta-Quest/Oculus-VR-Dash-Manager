using OVR_Dash_Manager.Functions;
using OVR_Dash_Manager.Functions.Dashes;
using OVR_Dash_Manager.Functions.Steam;
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
            Application.Current.Dispatcher
                .Invoke(() =>
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

        public void EnableHoverButton(Dash_Type Dash)
        {
            switch (Dash)
            {
                case Dash_Type.Exit:
                    Exit_Link.Enabled = true;
                    break;

                case Dash_Type.Normal:
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
                    Application.Current.Dispatcher
                        .Invoke(() =>
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
            SteamRunning.Close_SteamVR_ResetLink();
        }

        public void UpdateDashButtons()
        {
            foreach (UIElement item in _mainWindow.gd_DashButtons.Children)
            {
                if (item is Button button)
                {
                    if (button.Tag is Dash_Type Dash)
                    {
                        var Enabled = Dash_Manager.IsInstalled(Dash);
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
            try
            {
                // If not hovering, exit the method early to avoid unnecessary processing.
                if (!hoverButton.Hovering)
                    return;

                // If Check_SteamVR is true and Ignore_SteamVR_Status_HoverButtonAction is false,
                // check if Steam_VR_Server_Running is false. If it is, exit the method early.
                if (hoverButton.Check_SteamVR &&
                    !Properties.Settings.Default.Ignore_SteamVR_Status_HoverButtonAction &&
                    !SteamRunning.Steam_VR_Server_Running)
                    return;

                // Calculate the time passed since hovering started.
                var Passed = DateTime.Now.Subtract(hoverButton.Hover_Started);

                // Update the Bar value on the UI thread to avoid potential threading issues.
                Application.Current.Dispatcher
                    .Invoke(() =>
                {
                    hoverButton.Bar.Value = Passed.TotalMilliseconds;
                });

                // Check if the passed time is greater or equal to the defined threshold.
                // If it is, reset the bar, invoke the hover complete action, and exit the method.
                if (Passed.TotalSeconds >= hoverButton.Hovered_Seconds_To_Activate)
                {
                    hoverButton.Reset();
                    hoverButton.Bar.Value = hoverButton.Bar.Maximum;
                    hoverButton.Hover_Complete_Action.Invoke();
                }
            }
            catch (Exception ex)
            {
                // Log any exceptions that occur for debugging and diagnostic purposes.
                Debug.WriteLine($"Exception in CheckHovering: {ex.Message}");
            }
        }

        public void ActivateDash()
        {
            // TODO: Implement dash activation logic
            System.Diagnostics.Debug.WriteLine("ActivateDash called!");
        }
    }
}