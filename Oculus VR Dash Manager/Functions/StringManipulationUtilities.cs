using System;

namespace OVR_Dash_Manager.Functions
{
    internal class StringManipulationUtilities
    {
        public static string RemoveStringFromEnd(string text, string remove)
        {
            if (text.EndsWith(remove))
                text = text.Substring(0, text.Length - remove.Length);

            return text;
        }

        public static string RemoveStringFromStart(string text, string remove)
        {
            if (text.StartsWith(remove))
                text = text.Substring(remove.Length, text.Length - remove.Length);

            return text;
        }

        public static bool IsValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        public static string GetFullUrl(string url)
        {
            if (IsValidUrl(url))
            {
                return url.StartsWith("http", StringComparison.OrdinalIgnoreCase) ? url : "http://" + url;
            }

            return url;
        }
    }
}