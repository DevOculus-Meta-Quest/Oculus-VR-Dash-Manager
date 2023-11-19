using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq; // Ensure Newtonsoft.Json is installed

namespace OVR_Dash_Manager.Software
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
                var response = await _httpClient.GetAsync(requestUri);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(content);

                // Assuming the API returns a JSON object with a field that contains the image URL
                var imageUrl = json["data"]?[0]?["url"]?.ToString();

                return imageUrl;
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
