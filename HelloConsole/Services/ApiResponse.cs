namespace HelloConsole.Services;

public record ApiResponse
{
    public string? Content { get; init; }

    public bool IsNotModified { get; init; }

    public bool IsSuccess { get; init; }

    public DateTimeOffset? LastModified { get; init; }
}