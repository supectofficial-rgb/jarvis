namespace Insurance.HubService.AppCore.Shared.Conversations.Services;

using Insurance.HubService.AppCore.Domain.Conversations.Dtos;
using Insurance.HubService.AppCore.Domain.Conversations.Entities;
using Insurance.HubService.AppCore.Domain.Conversations.Enums;

public interface IConversationOrchestrator
{
    Task<ConversationSessionStarted> StartSessionAsync(string? sessionId, string? userId, CancellationToken cancellationToken);
    Task<ConversationReply> HandleUserMessageAsync(UserMessageRequest request, CancellationToken cancellationToken);
    Task<AudioTranscriptionResult> TranscribeUserAudioAsync(AudioTranscriptionRequest request, CancellationToken cancellationToken);
    Task<IReadOnlyList<ConversationMessage>> GetHistoryAsync(string sessionId, int maxItems, CancellationToken cancellationToken);
}


