using Domain;
using Microsoft.Extensions.Logging;
using Application;
using Infrastructure;

public class Program
{
    static async Task Main()
    {
        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        });
        ILogger<PriceManager> plog = loggerFactory.CreateLogger<PriceManager>();

        PriceManager priceManager = new PriceManager(plog);
        WishlistManager wishlistManager = new WishlistManager(priceManager);

        List<long> gameIds = new List<long> 
        { 
            208650, 
            578080, 
            3357650, 
            1364780, 
            1091500 
        };

        var topPrices = await wishlistManager.GetTopPricesAsync(gameIds, "kg");
        
        Console.WriteLine("From lowest to highest:");
        foreach(var price in topPrices)
            Console.WriteLine($"{price.name}: price - {price.finalPrice}; discount - {price.discount}%");
    }
}