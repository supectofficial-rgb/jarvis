using Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Orchestration;

namespace Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Session;

public sealed class AssistantSession
{
    public string SessionId { get; init; } = Guid.NewGuid().ToString("N");
    public string UserId { get; set; } = "anonymous";
    public DialogueState CurrentState { get; set; } = DialogueState.Idle;
    public string? CurrentAction { get; set; }
    public Dictionary<string, string?> CollectedParams { get; } = new(StringComparer.OrdinalIgnoreCase);
    public List<string> MissingParams { get; } = new();
    public string? LastTurnSummary { get; set; }
    public string? LastMessageId { get; set; }
    public string? LastResponseStatus { get; set; }
    public string? CorrelationId { get; set; }
    public bool RequiresAuthentication { get; set; }
    public bool WaitingForConfirmation { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public bool WasTimedOut { get; set; }

    public void ClearPendingAction()
    {
        CurrentAction = null;
        CollectedParams.Clear();
        MissingParams.Clear();
        RequiresAuthentication = false;
        WaitingForConfirmation = false;
    }
}


