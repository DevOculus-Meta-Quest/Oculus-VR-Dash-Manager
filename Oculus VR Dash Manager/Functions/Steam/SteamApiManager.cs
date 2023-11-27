using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace OVR_Dash_Manager.Functions.Steam
{
    internal class SteamApiManager
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public SteamApiManager()
        {
            _httpClient = new HttpClient();
            _apiKey = Environment.GetEnvironmentVariable("STEAMGRIDDB_API_KEY");
            // Ensure that the environment variable "STEAMGRIDDB_API_KEY" is set with your SteamGridDB API key
        }

        // Method to fetch game cover image by SteamGridDB ID
        public async Task<string> GetGameCoverByGridDbIdAsync(string gameId)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);
            return await FetchImageUrlAsync($"https://www.steamgriddb.com/api/v2/grids/game/{gameId}");
        }

        // Method to fetch game cover image by Steam App ID
        public async Task<string> GetGameCoverBySteamIdAsync(string steamAppId)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);
            return await FetchImageUrlAsync($"https://www.steamgriddb.com/api/v2/grids/steam/{steamAppId}");
        }

        // Method to fetch game logo by SteamGridDB ID
        public async Task<string> GetGameLogoByGridDbIdAsync(string gameId)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);
            return await FetchImageUrlAsync($"https://www.steamgriddb.com/api/v2/logos/game/{gameId}");
        }

        // Method to fetch game icon by SteamGridDB ID
        public async Task<string> GetGameIconByGridDbIdAsync(string gameId)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);
            return await FetchImageUrlAsync($"https://www.steamgriddb.com/api/v2/icons/game/{gameId}");
        }

        // Helper method to fetch image URL from SteamGridDB
        private async Task<string> FetchImageUrlAsync(string requestUri)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{requestUri}?key={_apiKey}");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"API Response: {content}"); // Temporary logging

                var json = JObject.Parse(content);

                if (json["data"] != null && json["data"].Any())
                {
                    var imageUrl = json["data"][0]["url"]?.ToString();
                    return imageUrl;
                }
                else
                {
                    Console.WriteLine("No image data available for this ID.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching image: {ex.Message}");
                return null;
            }
        }

        // Method to perform a search
        public async Task<string> SearchGamesAsync(string searchTerm)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);
                var response = await _httpClient.GetAsync($"https://www.steamgriddb.com/api/v2/search/autocomplete/{searchTerm}");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                return content;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error performing search: {ex.Message}");
                return null;
            }
        }

        // Add more methods as needed
    }
}