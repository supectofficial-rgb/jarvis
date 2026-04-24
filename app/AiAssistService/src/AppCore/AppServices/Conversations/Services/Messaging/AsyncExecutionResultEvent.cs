namespace Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Messaging;

public sealed class AsyncExecutionResultEvent
{
    public string SessionId { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = string.Empty;
    public string ActionName { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string? Reason { get; set; }
    public object? Data { get; set; }
}


