using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows;
using System.Windows.Threading;

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

            string result = "";
            if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
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
                    byte[] bytes = Encoding.ASCII.GetBytes(formParams);
                    webRequest.ContentLength = bytes.Length;
                    using (Stream os = webRequest.GetRequestStream())
                        os.Write(bytes, 0, bytes.Length);
                }

                WebResponse webResponse = null;
                StreamReader streamRead = null;

                try
                {
                    webResponse = webRequest.GetResponse();
                    streamRead = new StreamReader(webResponse.GetResponseStream(), Encoding.UTF8);
                    result = streamRead.ReadToEnd();
                }
                catch (Exception ex)
                {
                    result = HandleWebException(ex);
                }
                finally
                {
                    streamRead?.Dispose();
                    webResponse?.Dispose();
                }
            }

            return result;
        }

        private static string HandleWebException(Exception ex)
        {
            if (ex.Message.Contains("an error") && Regex.IsMatch(ex.Message, @"\(\d{3}\)"))
            {
                return Regex.Match(ex.Message, @"\(\d{3}\)").Value.Substring(1, 3);
            }

            if (ex.Message == "Unable to connect to the remote server")
            {
                return "Offline";
            }

            return "404";
        }

        public static bool Get_File(string fullUrl, string saveTo)
        {
            try
            {
                using (WebClient myWebClient = new WebClient())
                {
                    myWebClient.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
                    myWebClient.DownloadFile(fullUrl, saveTo);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static void OpenURL(string url)
        {
            ProcessStartInfo ps = new ProcessStartInfo(url)
            {
                UseShellExecute = true,
                Verb = "open"
            };
            Process.Start(ps);
        }

        public static void DoAction(Window form, Action doAction)
        {
            if (form == null || doAction == null)
                throw new ArgumentNullException(form == null ? nameof(form) : nameof(doAction));

            form.Dispatcher.Invoke(doAction, DispatcherPriority.Normal);
        }

        [DllImport("User32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        public static void MoveCursor(int X, int Y)
        {
            SetCursorPos(X, Y);
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
            StringBuilder Buff = new StringBuilder(nChars);
            IntPtr handle = GetForegroundWindow();

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
        private static Dictionary<string, Timer> Timers = new Dictionary<string, Timer>();

        public static bool SetNewInterval(string timerID, TimeSpan interval)
        {
            if (Timers.ContainsKey(timerID))
            {
                Timer timer = Timers[timerID];
                timer.Interval = interval.TotalMilliseconds;
                return true;
            }

            return false;
        }

        public static bool CreateTimer(string timerID, TimeSpan interval, ElapsedEventHandler tickHandler, bool repeat = true)
        {
            lock (Timers)  // Ensure thread safety during creation
            {
                if (!Timers.ContainsKey(timerID))
                {
                    Timer timer = new Timer
                    {
                        Interval = interval.TotalMilliseconds,
                        AutoReset = repeat,
                        Enabled = false  // Consider not starting the timer here
                    };

                    timer.Elapsed += tickHandler;
                    // timer.Start();  // Consider moving this to StartTimer

                    Timers.Add(timerID, timer);

                    return true;
                }
            }

            return false;
        }

        public static bool StartTimer(string timerID)
        {
            if (Timers.ContainsKey(timerID))
            {
                Timer timer = Timers[timerID];
                timer.Enabled = true;
                return true;
            }

            return false;
        }

        public static bool StopTimer(string timerID)
        {
            if (Timers.ContainsKey(timerID))
            {
                Timer timer = Timers[timerID];
                timer.Enabled = false;
                return true;
            }

            return false;
        }

        public static bool TimerExists(string timerID)
        {
            return Timers.ContainsKey(timerID);
        }

        public static void DisposeTimer(string timerID)
        {
            if (Timers.ContainsKey(timerID))
            {
                Timer timer = Timers[timerID];
                Timers.Remove(timerID);

                timer.Stop();
                timer.Dispose();
            }
        }

        public static void DisposeAllTimers()
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