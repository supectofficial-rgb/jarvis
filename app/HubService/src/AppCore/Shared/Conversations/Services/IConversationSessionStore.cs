namespace Insurance.HubService.AppCore.Shared.Conversations.Services;

using Insurance.HubService.AppCore.Domain.Conversations.Dtos;
using Insurance.HubService.AppCore.Domain.Conversations.Entities;
using Insurance.HubService.AppCore.Domain.Conversations.Enums;

public interface IConversationSessionStore
{
    Task<ConversationSession> GetOrCreateAsync(string? sessionId, string? userId, CancellationToken cancellationToken);
    Task<ConversationSession?> GetAsync(string sessionId, CancellationToken cancellationToken);
    Task SaveAsync(ConversationSession session, CancellationToken cancellationToken);
}


