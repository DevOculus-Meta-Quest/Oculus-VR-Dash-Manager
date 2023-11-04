using Microsoft.Win32;
using System;
using YOVR_Dash_Manager.Functions;

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

        public static bool SetKeyValue(RegistryKey key, string keyName, object value, RegistryValueKind valueKind)
        {
            try
            {
                if (key == null) throw new ArgumentNullException(nameof(key));

                // Depending on the expected value kind, cast the value accordingly
                switch (valueKind)
                {
                    case RegistryValueKind.DWord:
                        int intValue = Convert.ToInt32(value);
                        key.SetValue(keyName, intValue, RegistryValueKind.DWord);
                        break;

                    case RegistryValueKind.String:
                        string stringValue = Convert.ToString(value);
                        key.SetValue(keyName, stringValue, RegistryValueKind.String);
                        break;

                    case RegistryValueKind.ExpandString:
                        string expandStringValue = Convert.ToString(value);
                        key.SetValue(keyName, expandStringValue, RegistryValueKind.ExpandString);
                        break;
                    // Handle other types as necessary
                    default:
                        throw new ArgumentException($"Unsupported registry value kind: {valueKind}");
                }
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception with more details
                ErrorLogger.LogError(ex, $"Failed to set {keyName} to {value}");
                return false;
            }
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

        public static RegistryKey CreateRegistryKey(RegistryKeyType type, string keyLocation)
        {
            try
            {
                RegistryKey baseKey = type switch
                {
                    RegistryKeyType.ClassRoot => Registry.ClassesRoot,
                    RegistryKeyType.CurrentUser => Registry.CurrentUser,
                    RegistryKeyType.LocalMachine => Registry.LocalMachine,
                    RegistryKeyType.Users => Registry.Users,
                    RegistryKeyType.CurrentConfig => Registry.CurrentConfig,
                    _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                };

                RegistryKey key = baseKey.CreateSubKey(keyLocation);
                if (key != null)
                {
                    // The key was created successfully
                    return key;
                }
                else
                {
                    ErrorLogger.LogError(new Exception("Failed to create registry key at " + keyLocation));
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions if any
                ErrorLogger.LogError(ex, "Exception creating registry key.");
            }

            return null;
        }
    }
}