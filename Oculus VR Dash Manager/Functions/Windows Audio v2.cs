using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace OVR_Dash_Manager.Functions
{
    public static class Windows_Audio_v2
    {
        // Fields
        private static IAudioController controller;

        private static bool _IsSetup;
        public static List<IDevice_Ext> Speakers;

        // Method to setup audio controller and initialize speakers list
        public static void Setup()
        {
            if (_IsSetup)
                return;

            controller = new CoreAudioController();
            var Devices = controller.GetDevices();
            Speakers = new List<IDevice_Ext>();

            foreach (IDevice item in Devices)
            {
                var New = new IDevice_Ext(item);

                if (New.ID == Properties.Settings.Default.Normal_Speaker_GUID)
                    New.Normal_Speaker = true;

                if (New.ID == Properties.Settings.Default.Quest_Speaker_GUID)
                    New.Quest_Speaker = true;

                Speakers.Add(New);
            }

            _IsSetup = true;
        }

        // Method to set the default playback device
        public static void Set_Default_PlaybackDevice(IDevice Speaker)
        {
            Speaker.SetAsDefault();

            if (Properties.Settings.Default.Auto_Audio_Change_DefaultCommunication)
                Speaker.SetAsDefaultCommunications();
        }

        // Overloaded method to set the default playback device using Speaker ID
        public static void Set_Default_PlaybackDevice(Guid Speaker_ID)
        {
            var Speaker = controller.GetDevice(Speaker_ID);

            if (Speaker != null)
                Set_Default_PlaybackDevice(Speaker);
        }

        // Method to set to normal speaker automatically
        public static void Set_To_Normal_Speaker_Auto(bool Force = false)
        {
            if (Properties.Settings.Default.Automatic_Audio_Switching || Force)
            {
                var Speaker = controller.GetDevice(Properties.Settings.Default.Normal_Speaker_GUID);

                if (Speaker != null)
                    Set_Default_PlaybackDevice(Speaker);
            }
        }

        // Method to set to Quest speaker automatically
        public static void Set_To_Quest_Speaker_Auto(bool Force = false)
        {
            if (Properties.Settings.Default.Automatic_Audio_Switching || Force)
            {
                var Speaker = controller.GetDevice(Properties.Settings.Default.Quest_Speaker_GUID);

                if (Speaker != null)
                    Set_Default_PlaybackDevice(Speaker);
            }
        }

        // Class to extend IDevice with additional properties and notify property changed capability
        public class IDevice_Ext : INotifyPropertyChanged
        {
            #region Notify Property Changed Members

            public event PropertyChangedEventHandler PropertyChanged;

            private void OnPropertyChanged(string propertyName)
            {
                var handler = PropertyChanged;

                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs(propertyName));
                }
            }

            #endregion Notify Property Changed Members

            // Constructor to initialize IDevice_Ext object
            public IDevice_Ext(IDevice Speaker)
            {
                Name = Speaker.FullName;
                ID = Speaker.Id;
            }

            // Properties with OnPropertyChanged notification
            private bool _Normal_Speaker;

            public bool Normal_Speaker
            {
                get { return _Normal_Speaker; }
                set { if (value != _Normal_Speaker) { _Normal_Speaker = value; OnPropertyChanged("Normal_Speaker"); } }
            }

            private bool _Quest_Speaker;

            public bool Quest_Speaker
            {
                get { return _Quest_Speaker; }
                set { if (value != _Quest_Speaker) { _Quest_Speaker = value; OnPropertyChanged("Quest_Speaker"); } }
            }

            private string _Name;

            public string Name
            {
                get { return _Name; }
                set { if (value != null && value != _Name) { _Name = value; OnPropertyChanged("Name"); } }
            }

            private Guid _ID;

            public Guid ID
            {
                get { return _ID; }
                set { if (value != _ID) { _ID = value; OnPropertyChanged("ID"); } }
            }
        }
    }
}