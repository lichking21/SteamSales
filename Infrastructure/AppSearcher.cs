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

    public async Task FillSteamDictionary()
    {
        foreach(var path in _listsPath)
        {
            if (!File.Exists(path))
            {
                _logger.LogError($"(ERR) >> file wasn't found: {path}");
                continue;
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

        if (_dictionary.TryGetValue(name.Trim(), out long id)) return id;

        return 0;
    }
}