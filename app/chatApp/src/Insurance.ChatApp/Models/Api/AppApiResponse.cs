namespace Insurance.ChatApp.Models.Api;

public sealed class AppApiResponse<T>
{
    public bool IsSuccess { get; init; }
    public T? Data { get; init; }
    public string? ErrorMessage { get; init; }
    public List<string> Errors { get; init; } = new();

    public static AppApiResponse<T> Success(T? data)
        => new() { IsSuccess = true, Data = data };

    public static AppApiResponse<T> Failure(string? errorMessage, IEnumerable<string>? errors = null)
        => new()
        {
            IsSuccess = false,
            ErrorMessage = string.IsNullOrWhiteSpace(errorMessage) ? "Unexpected error." : errorMessage,
            Errors = errors?.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).ToList() ?? new List<string>()
        };
}
