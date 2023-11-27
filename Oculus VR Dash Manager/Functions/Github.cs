using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

// Disable the warning.
#pragma warning disable SYSLIB0014

namespace OVR_Dash_Manager.Functions
{
    public class Github
    {
        private readonly HttpClient httpClient = new HttpClient();
        private const string GithubApiBaseUrl = "https://api.github.com/repos/";

        public Github() => httpClient.DefaultRequestHeaders.Add("User-Agent", "OVR-Dash-Manager");

        public async Task<string> GetFileDownloadUrlAsync(string owner, string repo, string filePath)
        {
            // Constructing the URL to point to the raw content of the file
            var apiUrl = $"https://raw.githubusercontent.com/{owner}/{repo}/main/{filePath}";

            Debug.WriteLine($"Constructed URL: {apiUrl}"); // Log the constructed URL

            var response = await httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                return apiUrl; // Return the URL if the file exists
            }
            else
            {
                Debug.WriteLine($"Error: {response.ReasonPhrase}");
                return null; // Return null if the file does not exist
            }
        }

        public async Task<string> DownloadFileAsync(string downloadUrl)
        {
            var response = await httpClient.GetAsync(downloadUrl);
            response.EnsureSuccessStatusCode();

            var tempFilePath = Path.GetTempFileName();
            var fileBytes = await response.Content.ReadAsByteArrayAsync();
            await File.WriteAllBytesAsync(tempFilePath, fileBytes);

            return tempFilePath;
        }

        public async Task<string> GetFilesFromDirectoryAsync(string owner, string repo, string directoryPath, string branch = "main")
        {
            var url = $"{GithubApiBaseUrl}{owner}/{repo}/contents/{directoryPath}?ref={branch}";

            var response = await httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var jsonResult = await response.Content.ReadAsStringAsync();
                return jsonResult;
            }
            else
            {
                return $"Error: {response.ReasonPhrase}";
            }
        }

        public async Task<long> GetLatestSizeAsync(string repo, string project, string assetName)
        {
            var json = await GetJsonAsync($"https://api.github.com/repos/{repo}/{project}/releases/latest");
            var gitResponse = JsonConvert.DeserializeObject<GitResponse>(json);

            foreach (var asset in gitResponse.assets)
            {
                if (asset.name == assetName)
                {
                    return asset.size;
                }
            }

            return 0;
        }

        public async Task<string> GetLatestReleaseNameAsync(string repo, string project)
        {
            var json = await GetJsonAsync($"https://api.github.com/repos/{repo}/{project}/releases/latest");
            var gitResponse = JsonConvert.DeserializeObject<GitResponse>(json);
            return gitResponse.name;
        }

        public async Task DownloadAsync(string repo, string project, string assetName, string filePath)
        {
            var json = await GetJsonAsync($"https://api.github.com/repos/{repo}/{project}/releases/latest");
            var gitResponse = JsonConvert.DeserializeObject<GitResponse>(json);

            foreach (var asset in gitResponse.assets)
            {
                if (asset.name == assetName)
                {
                    using (var webClient = new System.Net.WebClient())
                    {
                        await webClient.DownloadFileTaskAsync(new Uri(asset.browser_download_url), filePath);
                    }
                }
            }
        }

        public async Task<GitHubReply> GetLatestReleaseInfoAsync(string repo, string project)
        {
            var json = await GetJsonAsync($"https://api.github.com/repos/{repo}/{project}/releases/latest");
            var gitResponse = JsonConvert.DeserializeObject<GitResponse>(json);
            var assetUrls = new Dictionary<string, string>();

            foreach (var asset in gitResponse.assets)
                assetUrls[asset.name] = asset.browser_download_url;

            return new GitHubReply(gitResponse.name, gitResponse.html_url, assetUrls);
        }

        private async Task<string> GetJsonAsync(string url)
        {
            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }

    public class GitHubReply
    {
        public GitHubReply(string releaseVersion, string releaseUrl, Dictionary<string, string> assetUrls)
        {
            ReleaseVersion = releaseVersion;
            ReleaseUrl = releaseUrl;
            AssetUrls = assetUrls;
        }

        public string ReleaseUrl { get; }
        public string ReleaseVersion { get; }
        public Dictionary<string, string> AssetUrls { get; }
    }

    internal class Asset
    {
        public string url { get; set; }
        public int id { get; set; }
        public string node_id { get; set; }
        public string name { get; set; }
        public object label { get; set; }
        public Uploader uploader { get; set; }
        public string content_type { get; set; }
        public string state { get; set; }
        public long size { get; set; }
        public int download_count { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string browser_download_url { get; set; }
    }

    internal class Author
    {
        public string login { get; set; }
        public int id { get; set; }
        public string node_id { get; set; }
        public string avatar_url { get; set; }
        public string gravatar_id { get; set; }
        public string url { get; set; }
        public string html_url { get; set; }
        public string followers_url { get; set; }
        public string following_url { get; set; }
        public string gists_url { get; set; }
        public string starred_url { get; set; }
        public string subscriptions_url { get; set; }
        public string organizations_url { get; set; }
        public string repos_url { get; set; }
        public string events_url { get; set; }
        public string received_events_url { get; set; }
        public string type { get; set; }
        public bool site_admin { get; set; }
    }

    internal class GitResponse
    {
        public string url { get; set; }
        public string assets_url { get; set; }
        public string upload_url { get; set; }
        public string html_url { get; set; }
        public int id { get; set; }
        public Author author { get; set; }
        public string node_id { get; set; }
        public string tag_name { get; set; }
        public string target_commitish { get; set; }
        public string name { get; set; }
        public bool draft { get; set; }
        public bool prerelease { get; set; }
        public DateTime created_at { get; set; }
        public DateTime published_at { get; set; }
        public List<Asset> assets { get; set; }
        public string tarball_url { get; set; }
        public string zipball_url { get; set; }
        public string body { get; set; }
        public int mentions_count { get; set; }
    }

    public class Uploader
    {
        public string login { get; set; }
        public int id { get; set; }
        public string node_id { get; set; }
        public string avatar_url { get; set; }
        public string gravatar_id { get; set; }
        public string url { get; set; }
        public string html_url { get; set; }
        public string followers_url { get; set; }
        public string following_url { get; set; }
        public string gists_url { get; set; }
        public string starred_url { get; set; }
        public string subscriptions_url { get; set; }
        public string organizations_url { get; set; }
        public string repos_url { get; set; }
        public string events_url { get; set; }
        public string received_events_url { get; set; }
        public string type { get; set; }
        public bool site_admin { get; set; }
    }
}