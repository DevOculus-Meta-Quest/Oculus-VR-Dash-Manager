using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Hardcodet.Wpf.TaskbarNotification;

namespace OculusVRDashManager.Functions
{
    public class WindowManager
    {
        private TaskbarIcon notifyIcon;
        private Window managedWindow; // The window that this WindowManager is managing

        public WindowManager(Window window)
        {
            managedWindow = window;
            notifyIcon = new TaskbarIcon
            {
                Icon = new System.Drawing.Icon(SystemIcons.Application, 40, 40),
                Visibility = Visibility.Visible,
                ToolTipText = "Application Name"
            };

            // Create the context menu and items
            ContextMenu contextMenu = new ContextMenu();
            MenuItem showItem = new MenuItem { Header = "Show" };
            showItem.Click += (sender, e) => ShowWindow();
            MenuItem exitItem = new MenuItem { Header = "Exit" };
            exitItem.Click += (sender, e) => ExitApplication();

            // Add the items to the context menu
            contextMenu.Items.Add(showItem);
            contextMenu.Items.Add(exitItem);

            // Assign the context menu
            notifyIcon.ContextMenu = contextMenu;

            notifyIcon.TrayMouseDoubleClick += (sender, args) => ShowWindow();
        }

        public void Minimize()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                managedWindow.WindowState = WindowState.Minimized;
                MinimizeToTray();
            });
        }

        public void MaximizeRestore()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                managedWindow.WindowState = managedWindow.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            });
        }

        public void Close()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                managedWindow.Close();
            });
        }

        public void ShowWindow()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                managedWindow.Show();
                managedWindow.WindowState = WindowState.Normal;
                notifyIcon.Visibility = Visibility.Hidden;
            });
        }

        public void HideWindow()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                managedWindow.Hide();
                notifyIcon.Visibility = Visibility.Visible;
            });
        }

        public void ExitApplication()
        {
            Application.Current.Shutdown();
        }

        public void MinimizeToTray()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                managedWindow.WindowState = WindowState.Minimized;
                managedWindow.Hide();
                notifyIcon.Visibility = Visibility.Visible;
            });
        }

        public void ShowNotification(string title, string text)
        {
            notifyIcon.ShowBalloonTip(title, text, BalloonIcon.Info);
        }

        public void ToggleAlwaysOnTop(bool enable)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                managedWindow.Topmost = enable;
            });
        }

        public void DragWindow()
        {
            Application.Current.Dispatcher.Invoke(() =>
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
