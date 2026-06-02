namespace HelloConsole.Services;

public interface IApiClient
{
    Task<ApiResponse> GetAsync(string route, DateTimeOffset? ifModifiedSince = null);
}