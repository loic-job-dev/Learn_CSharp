namespace HelloConsole.Services;

public class ApiClient : IApiClient
{
    private readonly HttpClient _sharedClient = new()
    {
        BaseAddress = new Uri("https://wilds.mhdb.io/")
    };

    public async Task<string> GetAsync(string route)
    {
        using HttpResponseMessage response =
            await _sharedClient.GetAsync(route);

        response.EnsureSuccessStatusCode();

        return await response.Content
            .ReadAsStringAsync();
    }
}