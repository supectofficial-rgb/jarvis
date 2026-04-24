namespace Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Response;

public sealed class AssistantTurnResponse
{
    public string Status { get; set; } = AssistantTurnStatus.UnsupportedRequest;
    public string Message { get; set; } = string.Empty;
    public string SessionId { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = string.Empty;
    public string? ActionName { get; set; }
    public List<string> MissingFields { get; set; } = new();
    public List<string> Suggestions { get; set; } = new();
    public object? Data { get; set; }
}


