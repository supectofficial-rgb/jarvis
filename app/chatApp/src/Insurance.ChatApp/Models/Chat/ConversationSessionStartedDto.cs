namespace Insurance.ChatApp.Models.Chat;

public sealed class ConversationSessionStartedDto
{
    public string SessionId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public DateTimeOffset StartedAtUtc { get; set; }
}
