namespace Insurance.AiAssistService.AppCore.Shared.Conversations.Commands.TranscribeAudio;

public class TranscribeAudioCommandResult
{
    public string SessionId { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = string.Empty;
    public string MessageId { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}


