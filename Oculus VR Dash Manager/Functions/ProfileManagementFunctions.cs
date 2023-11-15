using System;
using System.IO;

namespace OVR_Dash_Manager.Functions
{
    public static class ProfileManagementFunctions
    {
        private static readonly string ProfilesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Profiles");

        static ProfileManagementFunctions()
        {
            if (!Directory.Exists(ProfilesDirectory))
            {
                Directory.CreateDirectory(ProfilesDirectory);
            }
        }

        public static bool SaveProfile(string profileName, string profileData)
        {
            try
            {
                var filePath = Path.Combine(ProfilesDirectory, $"{profileName}.json");
                File.WriteAllText(filePath, profileData);
                return true;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex, $"Failed to save profile: {profileName}");
                return false;
            }
        }

        public static string LoadProfile(string profileName)
        {
            try
            {
                var filePath = Path.Combine(ProfilesDirectory, $"{profileName}.json");

                if (File.Exists(filePath))
                {
                    return File.ReadAllText(filePath);
                }
                else
                {
                    ErrorLogger.LogError(new FileNotFoundException($"Profile not found: {profileName}"));
                    return null;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex, $"Failed to load profile: {profileName}");
                return null;
            }
        }

        public static bool ApplyProfile(string profileData)
        {
            // You will need to implement the logic to apply the profile using OculusDebugToolCLI.exe
            // Return true if successful, false otherwise
            return false;
        }

        public static string[] GetAllProfiles()
        {
            try
            {
                var profileFiles = Directory.GetFiles(ProfilesDirectory, "*.json");

                for (int i = 0; i < profileFiles.Length; i++)
                    profileFiles[i] = Path.GetFileNameWithoutExtension(profileFiles[i]);

                return profileFiles;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex, "Failed to retrieve profiles.");
                return new string[0];
            }
        }
    }
}