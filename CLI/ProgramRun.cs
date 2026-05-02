using Domain;
using Microsoft.Extensions.Logging;
using Application;
using Infrastructure;
using System.Collections;

public static class ProgramRun
{
    private static string[] _listsPath =
    {
        "GamesLists/List1.json",
        "GamesLists/List2.json",
        "GamesLists/List3.json",
        "GamesLists/List4.json",
        "GamesLists/List5.json",
    };
    
    private static PriceManager? _priceManager;
    private static WishlistManager? _wishlistManager;
    private static AppSearcher? _appSearcher;

    public static async Task Execute()
    {
        using var loggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });
        ILogger<PriceManager> pmlog = loggerFactory.CreateLogger<PriceManager>();
        ILogger<AppSearcher> aslog = loggerFactory.CreateLogger<AppSearcher>();
        
        _priceManager = new PriceManager(pmlog);
        _wishlistManager = new WishlistManager(_priceManager);
        _appSearcher = new AppSearcher(aslog, _listsPath);
        
        Console.WriteLine("Loading Steam database...");
        await _appSearcher.FillSteamDictionary();

        List<long> gameIds = new List<long>();
        string region = SetRegion();

        bool isAdding = await UI(gameIds, region);
        if (!isAdding) return;

        await ShowWishlist(region, gameIds);
    }

    private static string SetRegion()
    {
        while(true)
        {
            Console.WriteLine("[>] Enter your region: ");
            string? region = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(region)) continue;
            else return region;
        }
    }

    private static async Task ShowWishlist(string region, List<long> gameIds)
    {
        if (_wishlistManager == null) return;

        var topPrices = await _wishlistManager.GetTopPricesAsync(gameIds, region);
        
        Console.WriteLine("\n=== Your wishlist ===");
        foreach(var price in topPrices)
            Console.WriteLine($"{price.name}: price - {price.finalPrice}; discount - {price.discount}%");
    }

    private static async Task<bool> UI(List<long> gameIds, string region)
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("============================================");
        Console.WriteLine("============|                  |============");
        Console.WriteLine("============| Best Steam Deals |============");
        Console.WriteLine("============|                  |============");
        Console.WriteLine("============================================\n");
        Console.ResetColor();

        Console.WriteLine("  [q]/[quit]  - to close the program");
        Console.WriteLine("  [e]/[enter] - to end adding games");
        Console.WriteLine("  [e]/[enter] - to show your wishlist");
        Console.WriteLine("***for better optimization please type exact game name***");
        Console.WriteLine("-------------------------------------------");

        while(true)
        {
            Console.WriteLine("[>] Type game title...");
            string? input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input)) continue;

            string command = input.ToLower();
            if (command == "q" || command == "quit") return false;
            if (command == "e" || command == "enter")
            {
                if (gameIds.Count == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("[-] You didn't add any game! Press [q]/[quit] or add game");
                    Console.ResetColor();
                    continue;
                }
                return true;
            }
            if (command == "l" || command == "list")
            {
                if (gameIds.Count == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("[-] You didn't add any game! Press [q]/[quit] or add game");
                    Console.ResetColor();
                    continue;
                }
                
                await ShowWishlist(region, gameIds);
                continue;   
            }
            long? id = _appSearcher?.GetId(input);
            if (id == null || id == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[-] Game '{input}' not found. Try exact title.");
                Console.ResetColor();
                continue;
            }

            if (!gameIds.Contains(id.Value))
            {
                gameIds.Add(id.Value);
                
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine($"[+] Game {input} was added");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[-] Game {input} is already in wishlist");
                Console.ResetColor();
            }
        }
    }
}