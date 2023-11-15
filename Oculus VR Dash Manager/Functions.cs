using OVR_Dash_Manager.Functions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows;
using System.Windows.Threading;

// Disable the warning.
#pragma warning disable SYSLIB0014

namespace OVR_Dash_Manager
{
    public static class Functions_Old
    {
        public static void ShowFileInDirectory(string fullPath)
        {
            Process.Start("explorer.exe", $@"/select,""{fullPath}""");
        }

        public static string GetPageHTML(string url, string method = "GET", CookieContainer cookies = null, string formParams = "", string contentType = "")
        {
            if (url.Contains("&amp;"))
                url = url.Replace("&amp;", "&");

            var result = "";

            if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                var webRequest = (HttpWebRequest)WebRequest.Create(url);
                webRequest.Method = method;
                webRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:88.0) Gecko/20100101 Firefox/88.0";
                webRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";

                if (contentType != "")
                    webRequest.ContentType = contentType;

                webRequest.AllowAutoRedirect = true;
                webRequest.MaximumAutomaticRedirections = 3;
                webRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                if (cookies != null)
                    webRequest.CookieContainer = cookies;

                if (formParams != "")
                {
                    var bytes = Encoding.ASCII.GetBytes(formParams);
                    webRequest.ContentLength = bytes.Length;

                    using (Stream os = webRequest.GetRequestStream())
                        os.Write(bytes, 0, bytes.Length);
                }

                try
                {
                    using (WebResponse webResponse = webRequest.GetResponse())
                    using (StreamReader streamRead = new StreamReader(webResponse.GetResponseStream(), Encoding.UTF8))
                        result = streamRead.ReadToEnd();
                }
                catch (Exception ex)
                {
                    result = HandleWebException(ex);
                }
            }

            return result;
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

        public static bool Get_File(string fullUrl, string saveTo)
        {
            try
            {
                using (WebClient myWebClient = new WebClient())
                {
                    myWebClient.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
                    myWebClient.DownloadFile(fullUrl, saveTo);
                }

                return true;
            }
            catch (Exception ex)
            {
                // Log the exception with your ErrorLogger
                ErrorLogger.LogError(ex, $"Failed to download file from {fullUrl} to {saveTo}");
                return false;
            }
        }

        public static void OpenURL(string url)
        {
            try
            {
                var ps = new ProcessStartInfo(url)
                {
                    UseShellExecute = true,
                    Verb = "open"
                };

                Process.Start(ps);
            }
            catch (Exception ex)
            {
                // Log the exception with your ErrorLogger
                ErrorLogger.LogError(ex, $"Failed to open URL: {url}");

                // Optionally: Inform the user about the error
                // MessageBox.Show("Failed to open the URL. Please check your internet connection and try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static void DoAction(Window form, Action doAction)
        {
            if (form == null || doAction == null)
                throw new ArgumentNullException(form == null ? nameof(form) : nameof(doAction));

            try
            {
                form.Dispatcher.Invoke(doAction, DispatcherPriority.Normal);
            }
            catch (Exception ex)
            {
                // Log error and handle as per your application's policy
                ErrorLogger.LogError(ex, "Error performing UI action.");
            }
        }

        [DllImport("User32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        public static void MoveCursor(int X, int Y)
        {
            try
            {
                SetCursorPos(X, Y);
            }
            catch (Exception ex)
            {
                // Log error and handle as per your application's policy
                ErrorLogger.LogError(ex, $"Error moving cursor to position ({X}, {Y}).");
            }
        }

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

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        public static string GetActiveWindowTitle()
        {
            const int nChars = 256;
            var Buff = new StringBuilder(nChars);
            var handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }

            return null;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool BringWindowToTop(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr SetFocus(IntPtr hWnd);
    }

    public enum RegistryKeyType
    {
        ClassRoot = 0,
        CurrentUser = 1,
        LocalMachine = 2,
        Users = 3,
        CurrentConfig = 4
    }

    public static class Timer_Functions
    {
        private static readonly object TimerLock = new object();
        private static Dictionary<string, Timer> Timers = new Dictionary<string, Timer>();

        public static bool SetNewInterval(string timerID, TimeSpan interval)
        {
            if (string.IsNullOrEmpty(timerID)) throw new ArgumentNullException(nameof(timerID));

            lock (TimerLock)
            {
                if (Timers.ContainsKey(timerID))
                {
                    var timer = Timers[timerID];
                    timer.Interval = interval.TotalMilliseconds;
                    return true;
                }
            }

            return false;
        }

        public static bool CreateTimer(string timerID, TimeSpan interval, ElapsedEventHandler tickHandler, bool repeat = true)
        {
            if (string.IsNullOrEmpty(timerID)) throw new ArgumentNullException(nameof(timerID));
            if (tickHandler == null) throw new ArgumentNullException(nameof(tickHandler));

            lock (TimerLock)
            {
                if (!Timers.ContainsKey(timerID))
                {
                    var timer = new Timer
                    {
                        Interval = interval.TotalMilliseconds,
                        AutoReset = repeat,
                        Enabled = false
                    };

                    timer.Elapsed += tickHandler;
                    Timers.Add(timerID, timer);

                    return true;
                }
            }

            return false;
        }

        public static bool StartTimer(string timerID)
        {
            if (string.IsNullOrEmpty(timerID)) throw new ArgumentNullException(nameof(timerID));

            lock (TimerLock)
            {
                if (Timers.ContainsKey(timerID))
                {
                    var timer = Timers[timerID];
                    timer.Start();
                    return true;
                }
            }

            return false;
        }

        public static bool StopTimer(string timerID)
        {
            if (string.IsNullOrEmpty(timerID)) throw new ArgumentNullException(nameof(timerID));

            lock (TimerLock)
            {
                if (Timers.ContainsKey(timerID))
                {
                    var timer = Timers[timerID];
                    timer.Stop();
                    return true;
                }
            }

            return false;
        }

        public static bool TimerExists(string timerID)
        {
            if (string.IsNullOrEmpty(timerID)) throw new ArgumentNullException(nameof(timerID));

            lock (TimerLock)
                return Timers.ContainsKey(timerID);
        }

        public static void DisposeTimer(string timerID)
        {
            if (string.IsNullOrEmpty(timerID)) throw new ArgumentNullException(nameof(timerID));

            lock (TimerLock)
            {
                if (Timers.ContainsKey(timerID))
                {
                    var timer = Timers[timerID];
                    Timers.Remove(timerID);

                    timer.Stop();
                    timer.Dispose();
                }
            }
        }

        public static void DisposeAllTimers()
        {
            lock (TimerLock)
            {
                foreach (var timer in Timers.Values)
                {
                    timer.Stop();
                    timer.Dispose();
                }

                Timers.Clear();
            }
        }
    }
}