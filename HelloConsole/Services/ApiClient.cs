namespace HelloConsole.Services;

public class ApiClient : IApiClient
{
    private readonly HttpClient _sharedClient =
        new()
        {
            BaseAddress = new Uri("https://wilds.mhdb.io/")
        };

    public async Task<HttpResponseMessage> GetAsync(string route, DateTimeOffset? ifModifiedSince = null)
    {
        var request =
            new HttpRequestMessage(
                HttpMethod.Get,
                route);

        // According to the API documentation, the server will respond with a
        // 304 Not Modified response if your locally cached data is up-to-date.
        if (ifModifiedSince is not null)
        {
            request.Headers
                    .IfModifiedSince =
                ifModifiedSince;
        }

        return await _sharedClient
            .SendAsync(request);
    }
}