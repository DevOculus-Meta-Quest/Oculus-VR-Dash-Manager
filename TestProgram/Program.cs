using System;
using System.Threading.Tasks;
using OVR_Dash_Manager.Software; // Replace with the correct namespace if different
using Newtonsoft.Json.Linq;


class Program
{
    static async Task Main(string[] args)
    {
        var steamApiManager = new SteamApiManager();

        Console.Write("Enter a search term: ");
        string searchTerm = Console.ReadLine();

        string searchResults = await steamApiManager.SearchGamesAsync(searchTerm);
        Console.WriteLine("Search Results:");
        Console.WriteLine(searchResults);

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

}
