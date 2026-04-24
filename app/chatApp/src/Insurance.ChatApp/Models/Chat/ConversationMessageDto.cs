namespace Insurance.ChatApp.Models.Chat;

public sealed class ConversationMessageDto
{
    public string MessageId { get; set; } = string.Empty;
    public ConversationMessageRole Role { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTimeOffset TimestampUtc { get; set; }
}
