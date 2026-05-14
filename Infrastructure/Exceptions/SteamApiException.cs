namespace Infrastructure.Exceptions;

// Custom exeption to handle SteamAPI errors
public class SteamApiException : Exception
{
    public long _gameId {get;}

    public SteamApiException(string message) : base(message)
    {
    }
}