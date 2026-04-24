namespace Insurance.ChatApp.Models.Chat;

public sealed class StartSessionRequest
{
    public string? SessionId { get; set; }
    public string? UserId { get; set; }
}
