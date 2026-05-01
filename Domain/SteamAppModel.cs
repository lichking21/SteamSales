using System.Text.Json.Serialization;

namespace Domain;

public class SteamAppModel
{
    [JsonPropertyName("appid")]
    public long ID {get;set;}

    [JsonPropertyName("name")]
    public string? Name {get;set;}
}

public class SteamAppList
{
    [JsonPropertyName("apps")]
    public List<SteamAppModel> Apps {get;set;} = new();
}

public class SteamAppsResponse
{
    [JsonPropertyName("response")]
    public SteamAppsList AppsLists {get;set;} = new();
}