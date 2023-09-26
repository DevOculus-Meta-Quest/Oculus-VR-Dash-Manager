using Microsoft.Win32;
using System;

namespace OVR_Dash_Manager.Functions
{
    public enum RegistryKeyType
    {
        ClassRoot,
        CurrentUser,
        LocalMachine,
        Users,
        CurrentConfig
    }

    public static class RegistryFunctions
    {
        public static RegistryKey GetRegistryKey(RegistryKeyType type, string keyLocation)
        {
            RegistryKey registryKey = type switch
            {
                RegistryKeyType.ClassRoot => Registry.ClassesRoot.OpenSubKey(keyLocation, writable: true),
                RegistryKeyType.CurrentUser => Registry.CurrentUser.OpenSubKey(keyLocation, writable: true),
                RegistryKeyType.LocalMachine => Registry.LocalMachine.OpenSubKey(keyLocation, writable: true),
                RegistryKeyType.Users => Registry.Users.OpenSubKey(keyLocation, writable: true),
                RegistryKeyType.CurrentConfig => Registry.CurrentConfig.OpenSubKey(keyLocation, writable: true),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };

            return registryKey;
        }

        public static bool SetKeyValue(RegistryKey key, string keyName, string value)
        {
            if (key == null) return false;

            string oldValue = Convert.ToString(key.GetValue(keyName));
            if (oldValue != value)
            {
                key.SetValue(keyName, value);
                return true;
            }

            return false;
        }

        public static string GetKeyValueString(RegistryKey key, string keyName)
        {
            return key?.GetValue(keyName)?.ToString();
        }

        public static string GetKeyValueString(RegistryKeyType type, string keyLocation, string keyName)
        {
            using RegistryKey key = GetRegistryKey(type, keyLocation);
            return GetKeyValueString(key, keyName);
        }

        public static void CloseKey(RegistryKey key)
        {
            key?.Close();
        }
    }
}
