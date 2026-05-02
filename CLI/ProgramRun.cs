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
    
    public static async Task Execute()
    {
        using var loggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });
        ILogger<PriceManager> pmlog = loggerFactory.CreateLogger<PriceManager>();
        ILogger<AppSearcher> aslog = loggerFactory.CreateLogger<AppSearcher>();

        PriceManager priceManager = new PriceManager(pmlog);
        WishlistManager wishlistManager = new WishlistManager(priceManager);
        AppSearcher appSearcher = new AppSearcher(aslog, _listsPath);
        await appSearcher.FillSteamDictionary();

        List<long> gameIds = new List<long>();
        string region = SetRegion();

        bool isAdding = UI(gameIds, appSearcher);
        if (!isAdding) return;

        var topPrices = await wishlistManager.GetTopPricesAsync(gameIds, region);
        
        Console.WriteLine("\nYour wishlist:");
        foreach(var price in topPrices)
            Console.WriteLine($"{price.name}: price - {price.finalPrice}; discount - {price.discount}%");
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

    private static bool UI(List<long> gameIds, AppSearcher appSearcher)
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

            long id = appSearcher.GetId(input);
            if (!gameIds.Contains(id))
            {
                gameIds.Add(id);
                
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine($"[+] Game {id} was added");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[-] Game {id} is already in wishlist");
                Console.ResetColor();
            }
        }
    }
}