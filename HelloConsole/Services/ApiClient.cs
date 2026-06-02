namespace HelloConsole.Services;

public class ApiClient : IApiClient
{
    private readonly HttpClient _sharedClient =
        new()
        {
            BaseAddress = new Uri("https://wilds.mhdb.io/")
        };

    /// <summary>
    /// Sends an HTTP GET request to the Monster Hunter Wilds API.
    /// </summary>
    /// <param name="route">
    /// Relative route to call from the API base address.
    /// </param>
    /// <param name="ifModifiedSince">
    /// Optional date used to populate the
    /// <c>If-Modified-Since</c> HTTP header.
    /// When provided, the server may return
    /// <c>304 Not Modified</c> if the resource
    /// has not changed since that date.
    /// </param>
    /// <returns>
    /// The HTTP response returned by the API.
    /// </returns>
    /// <remarks>
    /// This method does not validate the HTTP status code.
    /// The caller is responsible for checking the response
    /// and calling <see cref="HttpResponseMessage.EnsureSuccessStatusCode"/>
    /// when appropriate.
    /// </remarks>
    /// <exception cref="HttpRequestException">
    /// Thrown when the request cannot be sent or completed.
    /// </exception>
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