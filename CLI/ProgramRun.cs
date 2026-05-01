using Domain;
using Microsoft.Extensions.Logging;
using Application;
using Infrastructure;

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
        string[] games = {"doom", "the witcher", "dark souls III", "yakuza kiwami"};

        foreach(var name in games)
        {
            var id = appSearcher.GetId(name);
            Console.WriteLine($"NAME: {name}; ID: {id}");

            gameIds.Add(id);
        }

        var topPrices = await wishlistManager.GetTopPricesAsync(gameIds, "kg");
        
        Console.WriteLine("From lowest to highest:");
        foreach(var price in topPrices)
            Console.WriteLine($"{price.name}: price - {price.finalPrice}; discount - {price.discount}%");
    }

    private static void UI()
    {
        Console.WriteLine("============.__________________. ============");
        Console.WriteLine("============|                  |============");
        Console.WriteLine("============| Best Steam Deals |============");
        Console.WriteLine("============|                  |============");
        Console.WriteLine("============|__________________|============");

        Console.WriteLine("***for best optimization please type exact game name***");
    }
}