namespace Insurance.ChatApp.Models.Chat;

public sealed class ConversationReplyDto
{
    public string SessionId { get; set; } = string.Empty;
    public string AssistantMessage { get; set; } = string.Empty;
    public AiAssistantTurnResultDto Assistant { get; set; } = new();
    public IReadOnlyList<ConversationMessageDto> History { get; set; } = Array.Empty<ConversationMessageDto>();
    public DateTimeOffset RespondedAtUtc { get; set; }
}
