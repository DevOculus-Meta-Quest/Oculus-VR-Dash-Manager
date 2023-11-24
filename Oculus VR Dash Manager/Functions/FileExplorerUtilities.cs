using System;
using System.Diagnostics;

namespace OVR_Dash_Manager.Functions
{
    internal class FileExplorerUtilities
    {
        public static void ShowFileInDirectory(string fullPath)
        {
            Process.Start("explorer.exe", $@"/select,""{fullPath}""");
        }
    }
}
