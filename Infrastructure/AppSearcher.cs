using System.Text.Json;
using Microsoft.Extensions.Logging;
using Domain;

namespace Infrastructure;

public class AppSearcher
{
    private readonly ILogger<AppSearcher> _logger;
    private readonly string[] _listsPath;
    private readonly Dictionary<string, long> _dictionary = new(StringComparer.OrdinalIgnoreCase);

    public AppSearcher(ILogger<AppSearcher> logger, string[] listsPath)
    {
        _logger = logger;
        _listsPath = listsPath;
    }

    // Uses Streams to transfer data from JSONs into Dictionary with ignoring duplicates
    // Data in Dictionary is stored with case ignoring to provide fast search
    public async Task FillSteamDictionary()
    {
        foreach(var path in _listsPath)
        {
            if (!File.Exists(path))
            {
                string msg = $"Database file missing: {path}";
                _logger.LogCritical(msg);
                throw new FileNotFoundException(msg);
            }

            using var stream = File.OpenRead(path);
            var res = await JsonSerializer.DeserializeAsync<Response>(stream);

            if (res?.AppList.Apps != null)
            {
                foreach (var app in res.AppList.Apps)
                {
                    if (!string.IsNullOrWhiteSpace(app.Name))
                    {
                        _dictionary.TryAdd(app.Name.Trim(), app.ID);
                    }
                }
            }
        }
    }

    public long GetId(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            _logger.LogError("(ERR) >> app name can'be empty");
            return 0;
        }

        string searchItem = name.Trim();
        
        if (_dictionary.TryGetValue(searchItem, out long id)) return id;

        var partialMatch = _dictionary.FirstOrDefault(
            k => RemoveTradeMarks(k.Key).Contains(searchItem, StringComparison.OrdinalIgnoreCase)
        );

        if (partialMatch.Value != 0) return partialMatch.Value;

        return 0;
    }

    // Help function to remove trademarks in game's title
    private string RemoveTradeMarks(string input) => input.Replace("™", "").Replace("®", "").Replace("©", "");
}