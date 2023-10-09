using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

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
