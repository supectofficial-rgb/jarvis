namespace Insurance.HubService.AppCore.Shared.Conversations.Commands.TranscribeAudio;

using Insurance.HubService.AppCore.Domain.Conversations.Dtos;
using Insurance.HubService.AppCore.Domain.Conversations.Entities;
using Insurance.HubService.AppCore.Domain.Conversations.Enums;
using OysterFx.AppCore.Shared.Commands;

public class TranscribeAudioCommand : ICommand<AudioTranscriptionResult>
{
    public string? SessionId { get; set; }
    public string? UserId { get; set; }
    public string AudioBase64 { get; set; } = string.Empty;
    public string? Extension { get; set; }
    public string? MessageId { get; set; }
}


