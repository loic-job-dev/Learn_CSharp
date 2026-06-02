namespace HelloConsole.Services;

public interface IApiClient
{
    Task<string> GetAsync(string route);
}