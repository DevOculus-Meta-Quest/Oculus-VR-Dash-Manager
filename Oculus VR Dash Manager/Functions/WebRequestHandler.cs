using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace OVR_Dash_Manager.Functions
{
    public class WebRequestHandler
    {
        public string GetPageHTML(string url, string method = "GET", CookieContainer cookies = null, string formParams = "", string contentType = "")
        {
            url = ValidateAndFormatUrl(url);

            var webRequest = SetupWebRequest(url, method, contentType, cookies, formParams);

            return ExecuteWebRequest(webRequest, formParams);
        }

        private string ValidateAndFormatUrl(string url)
        {
            if (url.Contains("&amp;"))
                url = url.Replace("&amp;", "&");

            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
                throw new ArgumentException("Invalid URL format.", nameof(url));

            return url;
        }

        private HttpWebRequest SetupWebRequest(string url, string method, string contentType, CookieContainer cookies, string formParams)
        {
            var webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Method = method;
            // ... other setup code ...

            return webRequest;
        }

        private string ExecuteWebRequest(HttpWebRequest webRequest, string formParams)
        {
            try
            {
                using (WebResponse webResponse = webRequest.GetResponse())
                using (StreamReader streamRead = new StreamReader(webResponse.GetResponseStream(), Encoding.UTF8))
                    return streamRead.ReadToEnd();
            }
            catch (Exception ex)
            {
                return HandleWebException(ex);
            }
        }

        private static string HandleWebException(Exception ex)
        {
            // Log the exception details for future diagnosis.
            ErrorLogger.LogError(ex, "Web request failed");

            // Check for specific error messages and return user-friendly messages.
            if (ex.Message.Contains("an error") && Regex.IsMatch(ex.Message, @"\(\d{3}\)"))
            {
                // Extract and return the HTTP status code from the error message.
                return Regex.Match(ex.Message, @"\(\d{3}\)").Value.Substring(1, 3);
            }

            if (ex.Message == "Unable to connect to the remote server")
            {
                // Return a user-friendly message indicating offline status.
                return "Offline";
            }

            // Return a generic error message.
            return "An error occurred while fetching the webpage.";
        }

        // ... other related methods ...
    }
}