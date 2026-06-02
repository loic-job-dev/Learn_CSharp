namespace HelloConsole.Exceptions;

public class InvalidApiRequestException : Exception
{
    public InvalidApiRequestException()
        : base(
            "Requête API invalide.")
    {
    }
}