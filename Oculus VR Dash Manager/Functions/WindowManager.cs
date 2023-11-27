using Hardcodet.Wpf.TaskbarNotification;
using OVR_Dash_Manager.Functions;
using System;
using System.Drawing;
using System.IO; // For Stream
using System.Reflection; // For Assembly
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OculusVRDashManager.Functions
{
    public class WindowManager
    {
        private TaskbarIcon notifyIcon;
        private Window managedWindow; // The window that this WindowManager is managing

        public WindowManager(Window window)
        {
            managedWindow = window ?? throw new ArgumentNullException(nameof(window));
            notifyIcon = new TaskbarIcon(); // Instantiate the notifyIcon before using it.

            try
            {
                using (Stream iconStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("OVR_Dash_Manager.Icon.ico"))
                {
                    if (iconStream != null)
                    {
                        notifyIcon.Icon = new Icon(iconStream);
                        notifyIcon.Visibility = Visibility.Hidden; // Set the icon to be hidden initially.
                    }
                    else
                    {
                        // Log the error using the ErrorLogger class
                        ErrorLogger.LogError(new FileNotFoundException("Icon stream is null. Resource not found."));
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception using the ErrorLogger class
                ErrorLogger.LogError(ex, "Failed to load the application icon.");
            }

            // Now that notifyIcon is instantiated, you can set its properties.
            notifyIcon.Visibility = Visibility.Hidden; // Set this to Hidden initially.
            notifyIcon.ToolTipText = "Oculus VR Dash Manager";

            // Create the context menu and items
            var contextMenu = new ContextMenu();
            var showItem = new MenuItem { Header = "Show" };
            showItem.Click += (sender, e) => ShowWindow();
            var exitItem = new MenuItem { Header = "Exit" };
            exitItem.Click += (sender, e) => ExitApplication();

            // Add the items to the context menu
            contextMenu.Items.Add(showItem);
            contextMenu.Items.Add(exitItem);

            // Assign the context menu
            notifyIcon.ContextMenu = contextMenu;

            notifyIcon.TrayMouseDoubleClick += (sender, args) => ShowWindow();

            // Subscribe to the StateChanged event
            managedWindow.StateChanged += ManagedWindow_StateChanged;
        }

        private void ManagedWindow_StateChanged(object sender, EventArgs e)
        {
            // Check if the window was minimized
            if (managedWindow.WindowState == WindowState.Minimized)
            {
                MinimizeToTray();
            }
        }

        public void Minimize()
        {
            Application.Current.Dispatcher
                .Invoke(() =>
            {
                managedWindow.WindowState = WindowState.Minimized;
                MinimizeToTray();
            });
        }

        public void MaximizeRestore()
        {
            Application.Current.Dispatcher
                .Invoke(() =>
            {
                managedWindow.WindowState = managedWindow.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            });
        }

        public void Close()
        {
            Application.Current.Dispatcher
                .Invoke(() =>
            {
                managedWindow.Close();
            });
        }

        public void ShowWindow()
        {
            Application.Current.Dispatcher
                .Invoke(() =>
            {
                managedWindow.Show();
                managedWindow.WindowState = WindowState.Normal;
                notifyIcon.Visibility = Visibility.Hidden;
            });
        }

        public void HideWindow()
        {
            Application.Current.Dispatcher
                .Invoke(() =>
            {
                managedWindow.Hide();
                notifyIcon.Visibility = Visibility.Visible;
            });
        }

        public void ExitApplication() => Application.Current.Shutdown();

        public void MinimizeToTray()
        {
            if (OVR_Dash_Manager.Properties.Settings.Default.MinToTray)
            {
                Application.Current.Dispatcher
                    .Invoke(() =>
                {
                    managedWindow.Hide();
                    notifyIcon.Visibility = Visibility.Visible;
                });
            }
            else
            {
                // Handle the case where MinToTray is false
                // Perhaps just minimize the window normally
                Application.Current.Dispatcher
                    .Invoke(() =>
                {
                    managedWindow.WindowState = WindowState.Minimized;
                });
            }
        }

        public void ShowNotification(string title, string text)
        {
            notifyIcon.ShowBalloonTip(title, text, BalloonIcon.Info);
        }

        public void ToggleAlwaysOnTop(bool enable)
        {
            Application.Current.Dispatcher
                .Invoke(() =>
            {
                managedWindow.Topmost = enable;
            });
        }

        public void DragWindow()
        {
            Application.Current.Dispatcher
                .Invoke(() =>
            {
                if (managedWindow.WindowState == WindowState.Maximized)
                {
                    // Calculate correct position to restore down to before moving
                    var mouseX = Mouse.GetPosition(managedWindow).X;
                    var width = managedWindow.RestoreBounds.Width;
                    var x = mouseX - width / 2;

                    // Make sure window gets moved onto the screen
                    if (x < 0) x = 0;

                    if (x + width > SystemParameters.WorkArea.Width)
                        x = SystemParameters.WorkArea.Width - width;

                    managedWindow.Top = 0;
                    managedWindow.Left = x;

                    // Restore window to normal state
                    managedWindow.WindowState = WindowState.Normal;
                }

                managedWindow.DragMove();
            });
        }
    }
}