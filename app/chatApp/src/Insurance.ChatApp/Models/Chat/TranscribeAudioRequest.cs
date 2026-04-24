namespace Insurance.ChatApp.Models.Chat;

public sealed class TranscribeAudioRequest
{
    public string SessionId { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public string AudioBase64 { get; set; } = string.Empty;
    public string? Extension { get; set; }
    public string? MessageId { get; set; }
}
