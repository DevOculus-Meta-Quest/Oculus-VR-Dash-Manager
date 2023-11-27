using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows; // Ensure this using directive is correct for your project

namespace OVR_Dash_Manager.Functions
{
    public static class WindowUtilities
    {
        // Constants for window show state
        private const int SW_HIDE = 0;

        private const int SW_SHOWNORMAL = 1;
        private const int SW_SHOWMINIMIZED = 2;
        private const int SW_SHOWMAXIMIZED = 3;
        private const int SW_SHOW = 5;

        // Importing User32.dll functions
        [DllImport("User32")]
        private static extern int ShowWindow(IntPtr hwnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool BringWindowToTop(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr SetFocus(IntPtr hWnd);

        // Importing GetWindowRect function from user32.dll
        [DllImport("user32.dll")]
        internal static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);

        // Methods for window manipulation
        public static void ShowExternalWindow(IntPtr hwnd)
        {
            if (hwnd != IntPtr.Zero)
                ShowWindow(hwnd, SW_SHOW);
        }

        public static void HideExternalWindow(IntPtr hwnd)
        {
            if (hwnd != IntPtr.Zero)
                ShowWindow(hwnd, SW_HIDE);
        }

        public static void MinimizeExternalWindow(IntPtr hwnd)
        {
            if (hwnd != IntPtr.Zero)
                ShowWindow(hwnd, SW_SHOWMINIMIZED);
        }

        public static string GetActiveWindowTitle()
        {
            const int nChars = 256;
            StringBuilder Buff = new StringBuilder(nChars);
            IntPtr handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }

            return null;
        }

        public static string GetWindowText(IntPtr pHandle)
        {
            var pReturn = string.Empty;
            var pDataLength = GetWindowTextLength(pHandle);
            pDataLength++;  // Increase 1 for safety
            var buff = new StringBuilder(pDataLength);

            if (GetWindowText(pHandle, buff, pDataLength) > 0)
                pReturn = buff.ToString();

            buff.Clear();
            return pReturn;
        }

        // Additional window-related methods can be added here
    }
}