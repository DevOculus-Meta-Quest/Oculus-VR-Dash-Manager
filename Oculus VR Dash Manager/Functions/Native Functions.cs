using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;

namespace OVR_Dash_Manager.Functions
{
    public static class NativeFunctions
    {
        // Constants for window show state
        private const int SW_HIDE = 0;
        private const int SW_SHOWNORMAL = 1;
        private const int SW_SHOWMINIMIZED = 2;
        private const int SW_SHOWMAXIMIZED = 3;
        private const int SW_SHOW = 5;

        // Importing ShowWindow function from User32.dll
        [DllImport("User32")]
        private static extern int ShowWindow(IntPtr hwnd, int nCmdShow);

        // Method to show an external window
        public static void ShowExternalWindow(IntPtr hwnd)
        {
            if (hwnd != IntPtr.Zero)
                ShowWindow(hwnd, SW_SHOW);
        }

        // Method to hide an external window
        public static void HideExternalWindow(IntPtr hwnd)
        {
            if (hwnd != IntPtr.Zero)
                ShowWindow(hwnd, SW_HIDE);
        }

        // Method to minimize an external window
        public static void MinimizeExternalWindow(IntPtr hwnd)
        {
            if (hwnd != IntPtr.Zero)
                ShowWindow(hwnd, SW_SHOWMINIMIZED);
        }

        // Importing GetWindowRect function from user32.dll
        [DllImport("user32.dll")]
        internal static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);

        // Method to get the text of a window
        public static string GetWindowText(IntPtr pHandle)
        {
            string pReturn = string.Empty;
            int pDataLength = GetWindowTextLength(pHandle);
            pDataLength++;  // Increase 1 for safety
            StringBuilder buff = new StringBuilder(pDataLength);

            if (GetWindowText(pHandle, buff, pDataLength) > 0)
                pReturn = buff.ToString();

            buff.Clear();
            return pReturn;
        }

        // Importing GetWindowTextLength function from user32.dll
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern int GetWindowTextLength(IntPtr hWnd);

        // Importing GetWindowText function from user32.dll
        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
    }
}
