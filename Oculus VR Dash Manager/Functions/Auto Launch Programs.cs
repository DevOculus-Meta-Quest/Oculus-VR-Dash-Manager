using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace OVR_Dash_Manager.Functions
{
    public static class Auto_Launch_Programs
    {
        public static List<Auto_Program> Programs { get; private set; } = new List<Auto_Program>();

        public static List<Auto_Program> Generate_List()
        {
            Programs.Clear();

            try
            {
                var programData = Properties.Settings.Default.Auto_Programs_JSON;

                var slimPrograms = JsonFunctions.DeserializeClass<List<Slim_Auto_Program>>(programData);

                if (slimPrograms.Count > 0)
                {
                    foreach (var item in slimPrograms)
                    {
                        try
                        {
                            Programs.Add(new Auto_Program(item.FullPath, item.Startup_Launch, item.Closing_Launch));
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                        }
                    }
                }

                slimPrograms = null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return Programs;
        }

        public static void Run_Startup_Programs()
        {
            foreach (var item in Programs)
            {
                if (item.Startup_Launch)
                    ProcessFunctions.StartProcess(item.Full_Path);
            }
        }

        public static void Run_Closing_Programs()
        {
            foreach (var item in Programs)
            {
                if (item.Closing_Launch)
                    ProcessFunctions.StartProcess(item.Full_Path);
            }
        }

        public static void Save_Program_List()
        {
            var slimPrograms = new List<Slim_Auto_Program>();

            foreach (var item in Programs)
            {
                slimPrograms.Add(new Slim_Auto_Program(item.Full_Path, item.Startup_Launch, item.Closing_Launch));
                item.Changed = false;
            }

            Properties.Settings.Default.Auto_Programs_JSON = JsonFunctions.SerializeClass(slimPrograms);
            Properties.Settings.Default.Save();
        }

        public static void Add_New_Program(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    Programs.Add(new Auto_Program(path, false, false, true));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }

        public static void Remove_Program(Auto_Program program) => Programs.Remove(program);
    }

    public class Slim_Auto_Program
    {
        public Slim_Auto_Program()
        { }

        public Slim_Auto_Program(string fullPath, bool startupLaunch, bool closingLaunch)
        {
            FullPath = fullPath;
            Startup_Launch = startupLaunch;
            Closing_Launch = closingLaunch;
        }

        public string FullPath { get; set; }
        public bool Startup_Launch { get; set; }
        public bool Closing_Launch { get; set; }
    }

    public class Auto_Program : INotifyPropertyChanged
    {
        #region Notify Property Changed Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion Notify Property Changed Members

        public Auto_Program(string filePath, bool startupLaunch, bool closingLaunch, bool changed = false)
        {
            Full_Path = filePath;
            _File_Name = Path.GetFileName(filePath);
            _Folder_Path = Path.GetDirectoryName(filePath);
            _Program_Found = File.Exists(filePath);
            _Startup_Launch = startupLaunch;
            _Closing_Launch = closingLaunch;
            Changed = changed;
            _Program_Icon = null;

            if (_Program_Found)
            {
                try
                {
                    using (var ico = Icon.ExtractAssociatedIcon(filePath))
                    {
                        _Program_Icon = Imaging.CreateBitmapSourceFromHIcon(ico.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }

        public string Full_Path { get; set; }

        private ImageSource _Program_Icon;

        public ImageSource Program_Icon
        {
            get { return _Program_Icon; }
            set { if (value != null || value != _Program_Icon) _Program_Icon = value; OnPropertyChanged("Program_Icon"); }
        }

        private string _File_Name;

        public string File_Name
        {
            get { return _File_Name; }
            set { if (value != null || value != _File_Name) _File_Name = value; OnPropertyChanged("File_Name"); }
        }

        private string _Folder_Path;

        public string Folder_Path
        {
            get { return _Folder_Path; }
            set { if (value != null || value != _Folder_Path) _Folder_Path = value; OnPropertyChanged("Folder_Path"); }
        }

        private bool _Startup_Launch;

        public bool Startup_Launch
        {
            get { return _Startup_Launch; }
            set { if (value != _Startup_Launch) _Startup_Launch = value; OnPropertyChanged("Startup_Launch"); Changed = true; }
        }

        private bool _Closing_Launch;

        public bool Closing_Launch
        {
            get { return _Closing_Launch; }
            set { if (value != _Closing_Launch) _Closing_Launch = value; OnPropertyChanged("Closing_Launch"); Changed = true; }
        }

        private bool _Program_Found;

        public bool Program_Found
        {
            get { return _Program_Found; }
            set { if (value != _Program_Found) _Program_Found = value; OnPropertyChanged("Program_Found"); }
        }

        public bool Changed { get; set; }
    }
}