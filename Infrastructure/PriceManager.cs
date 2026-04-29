using System.Text.Json;
using Application;
using Domain;
using Microsoft.Extensions.Logging;
using Infrastructure.Exeptions;
namespace Infrastructure;

public class PriceManager : IPriceManager
{
    private static readonly HttpClient _client = new HttpClient();
    private readonly ILogger<PriceManager> _logger;
    public PriceManager(ILogger<PriceManager> logger)
    {
        _logger = logger;
    }

    public async Task<(string? name, string finalPrice, int discount)> GetSteamPriceAsync(int gameId, string userRegion)
    {
        string url = $"https://store.steampowered.com/api/appdetails?appids={gameId}&cc={userRegion}&l=english";

        try
        {
            var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string jsonContent = await response.Content.ReadAsStringAsync();
            var dictionary = JsonSerializer.Deserialize<Dictionary<string, GameDetails>>(jsonContent);

            if (dictionary != null && dictionary.TryGetValue(gameId.ToString(), out var gameDetails))
            {
                if (gameDetails.Success && gameDetails.Data != null)
                {
                    if (gameDetails.Data.IsFree)
                    {
                        return (gameDetails.Data.Name, "free", 0);
                    }
                    else if (gameDetails.Data.Price != null)
                    {
                        var price = gameDetails.Data.Price;
                        
                        if (price.DiscountPercent != 0)
                        {
                            _logger.LogInformation($"(LOG) >> Discount is: {price.DiscountPercent}%");
                            _logger.LogInformation($"(LOG) >> Initial price is: {price.InitialPrice}");
                            return (
                                gameDetails.Data.Name, 
                                price.FinalPrice, 
                                price.DiscountPercent);
                        }
                        else
                        {
                            return (
                                gameDetails.Data.Name, 
                                price.FinalPrice, 
                                price.DiscountPercent);
                        }
                    }
                }
            }
        }
        catch(HttpRequestException ex)
        {
            _logger.LogError($"{ex} (ERR) >> Network fail while fetching [{gameId}]");
            throw new SteamApiExeption($"Failed to fetch [{gameId}]");
        }
        catch(JsonException ex)
        {
            _logger.LogError($"{ex} (ERR) >> Deserialization failed for [{gameId}]");
            throw new SteamApiExeption($"Invalid JSON from SteamAPI for [{gameId}]");
        }

        return ("N/A", "N/A", 0);
    }
}
