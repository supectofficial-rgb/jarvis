namespace Insurance.ChatApp.Models.Chat;

public sealed class AudioTranscriptionResultDto
{
    public string SessionId { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = string.Empty;
    public string MessageId { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public DateTimeOffset TimestampUtc { get; set; }
}
