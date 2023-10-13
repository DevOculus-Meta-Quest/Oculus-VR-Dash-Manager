using System;
using System.Windows;

namespace OVR_Dash_Manager.Functions
{
    public class UIManager
    {
        private MainWindow _window;

        public UIManager(MainWindow window)
        {
            _window = window;
        }

        public void UpdateStatusLabel(string text)
        {
            _window.Dispatcher.Invoke(() =>
            {
                _window.lbl_CurrentSetting.Content = text;
            });
        }

        public void UpdateSteamVRStatusLabel(string text)
        {
            _window.Dispatcher.Invoke(() =>
            {
                _window.lbl_SteamVR_Status.Content = text;
            });
        }

        public void UpdateDesktopPlusStatusLabel(bool isInstalled)
        {
            _window.Dispatcher.Invoke(() =>
            {
                string statusText = isInstalled ? "Installed: True" : "Installed: False";
                _window.lbl_DesktopPlusStatus.Content = statusText;
            });
        }

        public void EnableButton(string buttonName, bool isEnabled)
        {
            _window.Dispatcher.Invoke(() =>
            {
                switch (buttonName)
                {
                    case "Diagnostics":
                        _window.btn_Diagnostics.IsEnabled = isEnabled;
                        break;
                    case "OpenSettings":
                        _window.btn_OpenSettings.IsEnabled = isEnabled;
                        break;
                    //... Add cases for other buttons as needed ...
                    default:
                        throw new ArgumentException("Invalid button name", nameof(buttonName));
                }
            });
        }

        public void ShowMessageBox(string message, string title, MessageBoxButton buttons, MessageBoxImage icon)
        {
            _window.Dispatcher.Invoke(() =>
            {
                MessageBox.Show(_window, message, title, buttons, icon);
            });
        }

        private void ShowDesktopPlusNotInstalledWarning()
        {
            // Assuming mainWindow is your main window that is set to always on top
            Window mainWindow = Application.Current.MainWindow;

            // Temporarily set the main window to not be topmost
            mainWindow.Topmost = false;

            // Notify the user or disable certain functionality.
            MessageBox.Show(mainWindow, "Desktop+ is not installed. Some functionality may be limited.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);

            // Set the main window back to topmost
            mainWindow.Topmost = true;
        }

        //... Add other UI management methods as needed ...
    }
}
