namespace OVR_Dash_Manager.Functions.Dashes
{
    public static class UtilityFunctions
    {
        public static bool EmulateReleaseMode(MainWindow mainForm)
        {
            return mainForm?.Debug_EmulateReleaseMode ?? false;
        }

        public static bool Oculus_Official_Dash_Installed(OVR_Dash oculusDash)
        {
            return oculusDash?.Installed ?? false;
        }
    }
}