namespace Insurance.AiAssistService.AppCore.Shared.Conversations.Commands.TranscribeAudio;

using OysterFx.AppCore.Shared.Commands;

public class TranscribeAudioCommand : ICommand<TranscribeAudioCommandResult>
{
    public string? SessionId { get; set; }
    public string? MessageId { get; set; }
    public string AudioBase64 { get; set; } = string.Empty;
    public string? Extension { get; set; }
}


