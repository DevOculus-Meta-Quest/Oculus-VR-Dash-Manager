using System;

namespace OVR_Dash_Manager.Functions
{
    public static class StringFunctions  // Renamed to follow PascalCase naming convention
    {
        /// <summary>
        /// Removes a specified substring from the start of the given string.
        /// </summary>
        /// <param name="text">The original string.</param>
        /// <param name="remove">The substring to remove.</param>
        /// <returns>The updated string.</returns>
        public static string RemoveStringFromStart(string text, string remove)
        {
            return text.StartsWith(remove) ? text.Substring(remove.Length) : text;
        }

        /// <summary>
        /// Checks if the given string is a valid URL.
        /// </summary>
        /// <param name="url">The string to check.</param>
        /// <returns>True if the string is a valid URL, false otherwise.</returns>
        public static bool IsValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        /// <summary>
        /// Ensures the given string is a valid URL by prepending "http://" if necessary.
        /// </summary>
        /// <param name="url">The original string.</param>
        /// <returns>The updated string.</returns>
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