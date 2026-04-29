namespace Application;

public class WishlistManager
{
    private readonly IPriceManager _priceManager;

    public WishlistManager(IPriceManager priceManager)
    {
        _priceManager = priceManager;
    }

    public async Task<List<(string? name, string finalPrice, int discount)>> GetTopPricesAsync(List<long> gameIds, string region)
    {
        var fetchTasks = gameIds.Select(id => _priceManager.GetSteamPriceAsync(id, region));
        var res = await Task.WhenAll(fetchTasks);

        var bestPrices = res
            .OrderByDescending(game => game.discount)
            .ToList();

        return bestPrices;
    }
}