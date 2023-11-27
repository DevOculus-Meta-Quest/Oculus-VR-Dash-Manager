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

    public static class RegistryManager
    {
        public static RegistryKey GetRegistryKey(RegistryKeyType type, string keyLocation)
        {
            var registryKey = type switch
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
                        var intValue = Convert.ToInt32(value);
                        key.SetValue(keyName, intValue, RegistryValueKind.DWord);
                        break;

                    case RegistryValueKind.String:
                        var stringValue = Convert.ToString(value);
                        key.SetValue(keyName, stringValue, RegistryValueKind.String);
                        break;

                    case RegistryValueKind.ExpandString:
                        var expandStringValue = Convert.ToString(value);
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
            using var key = GetRegistryKey(type, keyLocation);
            return GetKeyValueString(key, keyName);
        }

        public static void CloseKey(RegistryKey key) => key?.Close();

        public static RegistryKey CreateRegistryKey(RegistryKeyType type, string keyLocation)
        {
            try
            {
                var baseKey = type switch
                {
                    RegistryKeyType.ClassRoot => Registry.ClassesRoot,
                    RegistryKeyType.CurrentUser => Registry.CurrentUser,
                    RegistryKeyType.LocalMachine => Registry.LocalMachine,
                    RegistryKeyType.Users => Registry.Users,
                    RegistryKeyType.CurrentConfig => Registry.CurrentConfig,
                    _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                };

                var key = baseKey.CreateSubKey(keyLocation);

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

        public static object ReadRegistryValue(RegistryKey baseKey, string keyPath, string valueName)
        {
            try
            {
                using (RegistryKey key = baseKey.OpenSubKey(keyPath))
                {
                    if (key != null)
                    {
                        var value = key.GetValue(valueName);
                        return value; // Returns null if the value does not exist
                    }
                    else
                    {
                        ErrorLogger.LogError(new Exception("Key not found."), $"ReadRegistryValue: Key path {keyPath} not found.");
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex, "ReadRegistryValue Exception");
            }

            return null;
        }

        public static bool WriteRegistryValue(RegistryKey baseKey, string keyPath, string valueName, object value, RegistryValueKind valueKind)
        {
            try
            {
                using (RegistryKey key = baseKey.CreateSubKey(keyPath))
                {
                    if (key != null)
                    {
                        key.SetValue(valueName, value, valueKind);
                        return true;
                    }
                    else
                    {
                        ErrorLogger.LogError(new Exception("Unable to create key."), $"WriteRegistryValue: Unable to create or open key path {keyPath}.");
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex, "WriteRegistryValue Exception");
            }

            return false;
        }
    }
}

/*
// Usage examples
// To read a value from the registry:
object value = RegistryManager.ReadRegistryValue(RegistryManager.RegistryKeyType.CurrentUser, @"Software\MyApplication", "MyValue");

// To write a value to the registry:
bool success = RegistryManager.WriteRegistryValue(RegistryManager.RegistryKeyType.CurrentUser, @"Software\MyApplication", "MyValue", "MyNewValue", RegistryValueKind.String);

if (!success)
{
    // Handle the error, e.g., inform the user or attempt a retry.
}
*/