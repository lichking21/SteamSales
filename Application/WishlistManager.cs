namespace Application;

public class WishlistManager
{
    private readonly IPriceManager _priceManager;

    public WishlistManager(IPriceManager priceManager)
    {
        _priceManager = priceManager;
    }

    // Task.WhenAll() is used to send all HTTP requests to the SteamAPI simultaneously to reduce waiting time
    // LINQ is used to filter and sort data
    public async Task<List<(string? name, string finalPrice, int discount)>> GetTopPricesAsync(List<long> gameIds, string region)
    {
        var fetchTasks = gameIds.Select(id => _priceManager.GetSteamPriceAsync(id, region));
        var res = await Task.WhenAll(fetchTasks);

        var bestPrices = res
            .OrderByDescending(game => game.discount)
            .OrderBy(game => game.finalPrice)
            .ToList();

        return bestPrices;
    }
}