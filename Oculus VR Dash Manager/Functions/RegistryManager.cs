using System;
using Microsoft.Win32;
using YOVR_Dash_Manager.Functions;

public static class RegistryManager
{
    public static object ReadRegistryValue(RegistryKey baseKey, string keyPath, string valueName)
    {
        try
        {
            using (RegistryKey key = baseKey.OpenSubKey(keyPath))
            {
                if (key != null)
                {
                    object value = key.GetValue(valueName);
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

/*
// To read a value from the registry:
object value = RegistryManager.ReadRegistryValue(Registry.CurrentUser, @"Software\MyApplication", "MyValue");

// To write a value to the registry:
bool success = RegistryManager.WriteRegistryValue(Registry.CurrentUser, @"Software\MyApplication", "MyValue", "MyNewValue", RegistryValueKind.String);

if (!success)
{
    // Handle the error, e.g., inform the user or attempt a retry.
}
*/