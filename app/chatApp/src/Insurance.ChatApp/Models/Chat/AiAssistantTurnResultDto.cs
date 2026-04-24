namespace Insurance.ChatApp.Models.Chat;

public sealed class AiAssistantTurnResultDto
{
    public string Status { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string SessionId { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = string.Empty;
    public string? ActionName { get; set; }
    public IReadOnlyList<string> MissingFields { get; set; } = Array.Empty<string>();
    public IReadOnlyList<string> Suggestions { get; set; } = Array.Empty<string>();
    public object? Data { get; set; }
}
