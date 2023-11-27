using System;
using System.Runtime.InteropServices;

namespace OVR_Dash_Manager.Functions
{
    internal class CursorUtilities
    {
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

        // You can add other cursor-related methods here as needed
    }
}