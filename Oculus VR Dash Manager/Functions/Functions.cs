using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows;
using System.Windows.Threading;

// Disable the warning.
#pragma warning disable SYSLIB0014

namespace OVR_Dash_Manager.Functions
{
    public static class FunctionsOld
    {
        public static void ShowFileInDirectory(string fullPath)
        {
            Process.Start("explorer.exe", $@"/select,""{fullPath}""");
        }

        public static void OpenURL(string url)
        {
            try
            {
                var ps = new ProcessStartInfo(url)
                {
                    UseShellExecute = true,
                    Verb = "open"
                };

                Process.Start(ps);
            }
            catch (Exception ex)
            {
                // Log the exception with your ErrorLogger
                ErrorLogger.LogError(ex, $"Failed to open URL: {url}");

                // Optionally: Inform the user about the error
                // MessageBox.Show("Failed to open the URL. Please check your internet connection and try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static void DoAction(Window form, Action doAction)
        {
            if (form == null || doAction == null)
                throw new ArgumentNullException(form == null ? nameof(form) : nameof(doAction));

            try
            {
                form.Dispatcher.Invoke(doAction, DispatcherPriority.Normal);
            }
            catch (Exception ex)
            {
                // Log error and handle as per your application's policy
                ErrorLogger.LogError(ex, "Error performing UI action.");
            }
        }

        [DllImport("User32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        public static void MoveCursor(int X, int Y)
        {
            try
            {
                SetCursorPos(X, Y);
            }
            catch (Exception ex)
            {
                // Log error and handle as per your application's policy
                ErrorLogger.LogError(ex, $"Error moving cursor to position ({X}, {Y}).");
            }
        }

        public static string RemoveStringFromEnd(string text, string remove)
        {
            if (text.EndsWith(remove))
                text = text.Substring(0, text.Length - remove.Length);

            return text;
        }

        public static string RemoveStringFromStart(string text, string remove)
        {
            if (text.StartsWith(remove))
                text = text.Substring(remove.Length, text.Length - remove.Length);

            return text;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        public static string GetActiveWindowTitle()
        {
            const int nChars = 256;
            var Buff = new StringBuilder(nChars);
            var handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }

            return null;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool BringWindowToTop(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr SetFocus(IntPtr hWnd);
    }
}