using System.Text.Json.Serialization;

namespace Domain;

public class AppModel
{
    [JsonPropertyName("appid")]
    public long ID {get;set;}

    [JsonPropertyName("name")]
    public string? Name {get;set;}
}

public class AppList
{
    [JsonPropertyName("apps")]
    public List<AppModel> Apps {get;set;} = new();
}

public class Response
{
    [JsonPropertyName("response")]
    public AppList AppList {get;set;} = new();
}