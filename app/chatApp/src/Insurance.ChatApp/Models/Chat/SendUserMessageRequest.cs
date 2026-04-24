namespace Insurance.ChatApp.Models.Chat;

public sealed class SendUserMessageRequest
{
    public string SessionId { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public string Text { get; set; } = string.Empty;
}
