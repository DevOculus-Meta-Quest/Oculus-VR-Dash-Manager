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
    }
}