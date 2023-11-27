using System;
using System.Net;
using System.Net.Cache;

namespace OVR_Dash_Manager.Functions
{
    internal class GetFile
    {
        public static bool DownloadFile(string fullUrl, string saveTo)
        {
            try
            {
                ExecuteFileDownload(fullUrl, saveTo);
                return true;
            }
            catch (Exception ex)
            {
                HandleFileDownloadError(ex, fullUrl, saveTo);
                return false;
            }
        }

        private static void ExecuteFileDownload(string url, string savePath)
        {
            using (WebClient webClient = new WebClient())
            {
                webClient.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
                webClient.DownloadFile(url, savePath);
            }
        }

        private static void HandleFileDownloadError(Exception ex, string url, string savePath)
        {
            // Log the exception with your ErrorLogger
            // Assuming ErrorLogger is a static class available in your project
            ErrorLogger.LogError(ex, $"Failed to download file from {url} to {savePath}");
        }
    }
}

// bool downloadSuccess = GetFile.DownloadFile("http://example.com/file.zip", "localPath/file.zip");