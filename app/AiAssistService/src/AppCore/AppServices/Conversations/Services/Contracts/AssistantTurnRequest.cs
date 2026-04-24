namespace Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Contracts;

public sealed class AssistantTurnRequest
{
    public string? SessionId { get; set; }
    public string? UserId { get; set; }
    public string? MessageId { get; set; }
    public string Text { get; set; } = string.Empty;
    public string? AccessToken { get; set; }
    public bool IsConfirmation { get; set; }
    public bool IsRejection { get; set; }
    public bool IsCancellation { get; set; }
    public bool IsOverride { get; set; }
    public bool PreviewOnly { get; set; }
}


