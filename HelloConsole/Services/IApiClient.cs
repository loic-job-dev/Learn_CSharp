namespace HelloConsole.Services;

public interface IApiClient
{
    Task<HttpResponseMessage> GetAsync(string route, DateTimeOffset? ifModifiedSince = null);
}