using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using Hardcodet.Wpf.TaskbarNotification;

namespace OculusVRDashManager.Functions
{
    public class WindowManager
    {
        private TaskbarIcon notifyIcon;
        private Window mainWindow;

        public WindowManager(Window main)
        {
            mainWindow = main;
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

        public void ShowWindow()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                mainWindow.Show();
                mainWindow.WindowState = WindowState.Normal;
                notifyIcon.Visibility = Visibility.Hidden;
            });
        }

        public void HideWindow()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                mainWindow.Hide();
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
                mainWindow.WindowState = WindowState.Minimized;
                mainWindow.Hide();
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
                mainWindow.Topmost = enable;
            });
        }
    }
}