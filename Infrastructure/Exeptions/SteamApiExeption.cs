namespace Infrastructure.Exeptions;
public class SteamApiExeption : Exception
{
    public long _gameId {get;}

    public SteamApiExeption(string message) : base(message)
    {
        //_gameId = gameId;
    }
}