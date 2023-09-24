using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace OVR_Dash_Manager
{
    public class Github
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public async Task<long> GetLatestFileSize(string repo, string project, string assetName)
        {
            var json = await _httpClient.GetStringAsync($"https://api.github.com/repos/{repo}/{project}/releases/latest");
            var gitResponse = JsonConvert.DeserializeObject<GitResponse>(json);
            foreach (var asset in gitResponse.Assets)
            {
                if (asset.Name == assetName)
                {
                    return asset.Size;
                }
            }
            return 0;
        }

        public async Task<string> GetLatestReleaseVersion(string repo, string project)
        {
            var json = await _httpClient.GetStringAsync($"https://api.github.com/repos/{repo}/{project}/releases/latest");
            var gitResponse = JsonConvert.DeserializeObject<GitResponse>(json);
            return gitResponse.Name;
        }

        public async Task DownloadFile(string repo, string project, string assetName, string filePath)
        {
            var json = await _httpClient.GetStringAsync($"https://api.github.com/repos/{repo}/{project}/releases/latest");
            var gitResponse = JsonConvert.DeserializeObject<GitResponse>(json);
            foreach (var asset in gitResponse.Assets)
            {
                if (asset.Name == assetName)
                {
                    using (var webClient = new System.Net.WebClient())
                    {
                        await webClient.DownloadFileTaskAsync(new Uri(asset.Browser_download_url), filePath);
                    }
                }
            }
        }

        public async Task<GitHubReply> GetLatestReleaseDetails(string repo, string project)
        {
            var json = await _httpClient.GetStringAsync($"https://api.github.com/repos/{repo}/{project}/releases/latest");
            var gitResponse = JsonConvert.DeserializeObject<GitResponse>(json);
            var assetUrls = new Dictionary<string, string>();
            foreach (var asset in gitResponse.Assets)
            {
                assetUrls[asset.Name] = asset.Browser_download_url;
            }
            return new GitHubReply(gitResponse.Name, gitResponse.Html_url, assetUrls);
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
        public string Url { get; set; }
        public int Id { get; set; }
        public string Node_id { get; set; }
        public string Name { get; set; }
        public object Label { get; set; }
        public Uploader Uploader { get; set; }
        public string Content_type { get; set; }
        public string State { get; set; }
        public long Size { get; set; }
        public int Download_count { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime Updated_at { get; set; }
        public string Browser_download_url { get; set; }
    }

    internal class Author
    {
        public string Login { get; set; }
        public int Id { get; set; }
        public string Node_id { get; set; }
        public string Avatar_url { get; set; }
        public string Gravatar_id { get; set; }
        public string Url { get; set; }
        public string Html_url { get; set; }
        public string Followers_url { get; set; }
        public string Following_url { get; set; }
        public string Gists_url { get; set; }
        public string Starred_url { get; set; }
        public string Subscriptions_url { get; set; }
        public string Organizations_url { get; set; }
        public string Repos_url { get; set; }
        public string Events_url { get; set; }
        public string Received_events_url { get; set; }
        public string Type { get; set; }
        public bool Site_admin { get; set; }
    }

    internal class GitResponse
    {
        public string Url { get; set; }
        public string Assets_url { get; set; }
        public string Upload_url { get; set; }
        public string Html_url { get; set; }
        public int Id { get; set; }
        public Author Author { get; set; }
        public string Node_id { get; set; }
        public string Tag_name { get; set; }
        public string Target_commitish { get; set; }
        public string Name { get; set; }
        public string Draft { get; set; }
        public Author Uploader { get; set; }
        public string Prerelease { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime Published_at { get; set; }
        public List<Asset> Assets { get; set; }
        public string Tarball_url { get; set; }
        public string Zipball_url { get; set; }
        public string Body { get; set; }
    }

    internal class Uploader
    {
        public string Login { get; set; }
        public int Id { get; set; }
        public string Node_id { get; set; }
        public string Avatar_url { get; set; }
        public string Gravatar_id { get; set; }
        public string Url { get; set; }
        public string Html_url { get; set; }
        public string Followers_url { get; set; }
        public string Following_url { get; set; }
        public string Gists_url { get; set; }
        public string Starred_url { get; set; }
        public string Subscriptions_url { get; set; }
        public string Organizations_url { get; set; }
        public string Repos_url { get; set; }
        public string Events_url { get; set; }
        public string Received_events_url { get; set; }
        public string Type { get; set; }
        public bool Site_admin { get; set; }
    }
}
