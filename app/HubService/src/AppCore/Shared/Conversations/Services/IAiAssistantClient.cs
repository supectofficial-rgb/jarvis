namespace Insurance.HubService.AppCore.Shared.Conversations.Services;

using Insurance.HubService.AppCore.Domain.Conversations.Dtos;
using Insurance.HubService.AppCore.Domain.Conversations.Entities;
using Insurance.HubService.AppCore.Domain.Conversations.Enums;

public interface IAiAssistantClient
{
    Task<AiAssistantTurnResult> GetAssistantTurnAsync(AiAssistantRequest request, CancellationToken cancellationToken);
    Task<AudioTranscriptionResult> TranscribeAudioAsync(AudioTranscriptionRequest request, CancellationToken cancellationToken);
}


