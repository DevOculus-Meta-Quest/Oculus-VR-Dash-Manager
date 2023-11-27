using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace OVR_Dash_Manager.Functions
{
    internal class FileExplorerUtilities
    {
        public static void ShowFileInDirectory(string fullPath)
        {
            Process.Start("explorer.exe", $@"/select,""{fullPath}""");
        }

        public static string OpenSingle(
            string defaultDirectory = "",
            string defaultExtension = "",
            string fileExtensionFilters = "*.*;",
            bool mustExist = true)
        {
            var files = DoFileBrowser(defaultDirectory, defaultExtension, fileExtensionFilters, false, mustExist);
            return files.Count == 1 ? files[0] : string.Empty;
        }

        public static List<string> OpenMultiple(
            string defaultDirectory = "",
            string defaultExtension = "",
            string fileExtensionFilters = "*.*;",
            bool mustExist = true)
        {
            return DoFileBrowser(defaultDirectory, defaultExtension, fileExtensionFilters, true, mustExist);
        }

        private static List<string> DoFileBrowser(
            string defaultDirectory,
            string defaultExtension,
            string fileExtensionFilters,
            bool multipleFiles,
            bool mustExist)
        {
            var files = new List<string>();

            if (string.IsNullOrEmpty(defaultDirectory))
            {
                defaultDirectory = GetCurrentExecutableDirectory();
            }

            var fileTypes = new Dictionary<string, string>();

            if (!fileExtensionFilters.Contains("*.*"))
            {
                var splitFilters = fileExtensionFilters.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var ext in splitFilters)
                {
                    if (!fileTypes.ContainsKey(ext))
                    {
                        var name = GetDescription(ext.Replace("*", ""));
                        name = string.IsNullOrEmpty(name) ? ext.Replace("*.", "").ToUpper() + " File" : name;
                        fileTypes.Add(ext, name);
                    }
                }
            }
            else
            {
                fileTypes.Add("*.*", "All Files");
            }

            var extensionFilterSeparateAll = "Filtered Files|";
            var extensionFilterSeparate = "";

            foreach (var filter in fileTypes)
            {
                extensionFilterSeparateAll += filter.Key + ";";
                extensionFilterSeparate += "|" + filter.Value + "|" + filter.Key;
            }

            extensionFilterSeparate = extensionFilterSeparate.TrimStart('|');

            var dlg = new OpenFileDialog
            {
                DefaultExt = defaultExtension,
                Filter = extensionFilterSeparateAll + "|" + extensionFilterSeparate,
                InitialDirectory = defaultDirectory,
                AddExtension = !string.IsNullOrEmpty(defaultExtension),
                CheckFileExists = mustExist,
                CheckPathExists = true,
                ValidateNames = true,
                Multiselect = multipleFiles
            };

            var result = dlg.ShowDialog();

            if (result == true)
                files.AddRange(dlg.FileNames);

            return files;
        }

        private static string ReadDefaultValue(string regKey)
        {
            using (var key = Registry.ClassesRoot.OpenSubKey(regKey, false))
            {
                if (key != null)
                {
                    return key.GetValue("") as string;
                }
            }

            return null;
        }

        private static string GetDescription(string ext)
        {
            if (ext.StartsWith(".") && ext.Length > 1) ext = ext.Substring(1);

            var retVal = ReadDefaultValue(ext + "file");
            if (!string.IsNullOrEmpty(retVal)) return ext;

            using (var key = Registry.ClassesRoot.OpenSubKey("." + ext, false))
            {
                if (key == null) return "";

                using (var subkey = key.OpenSubKey("OpenWithProgids"))
                {
                    if (subkey == null) return "";

                    var names = subkey.GetValueNames();
                    if (names == null || names.Length == 0) return "";

                    foreach (var name in names)
                    {
                        retVal = ReadDefaultValue(name);
                        if (!string.IsNullOrEmpty(retVal)) return retVal;
                    }
                }
            }

            return "";
        }

        private static string GetCurrentExecutableDirectory()
        {
            return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }
    }
}