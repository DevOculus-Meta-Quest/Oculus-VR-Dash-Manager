using System.IO;

namespace OVR_Dash_Manager.Functions
{
    public static class ProcessFunctions
    {

        public static string GetCurrentProcessDirectory()
        {
            string assemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
            return Path.GetDirectoryName(assemblyLocation);
        }
    }
}
