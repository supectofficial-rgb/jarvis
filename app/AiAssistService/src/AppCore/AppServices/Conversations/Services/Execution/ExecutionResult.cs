namespace Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Execution;

public sealed class ExecutionResult
{
    public bool Success { get; set; }
    public bool IsAsync { get; set; }
    public string Message { get; set; } = string.Empty;
    public object? Payload { get; set; }
}


