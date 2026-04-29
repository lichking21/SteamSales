namespace Application;

public interface IPriceManager
{
    public Task<(string? name, string finalPrice, int discount)> GetSteamPriceAsync(int gameId, string userRegion);
}
