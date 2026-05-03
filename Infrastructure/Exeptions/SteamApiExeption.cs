namespace Infrastructure.Exeptions;

// Custom exeption to handle SteamAPI errors
public class SteamApiExeption : Exception
{
    public long _gameId {get;}

    public SteamApiExeption(string message) : base(message)
    {
    }
}