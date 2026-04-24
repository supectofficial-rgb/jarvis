using Insurance.HubService.AppCore.Shared.Conversations.Options;
using Insurance.HubService.AppCore.Domain.Conversations.Dtos;
using Insurance.HubService.AppCore.Domain.Conversations.Entities;
using Insurance.HubService.AppCore.Domain.Conversations.Enums;
using Insurance.HubService.AppCore.Shared.Conversations.Services;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace Insurance.HubService.AppCore.AppServices.Conversations.Services;

public sealed class InMemoryConversationSessionStore : IConversationSessionStore
{
    private readonly ConcurrentDictionary<string, ConversationSession> _sessions = new(StringComparer.OrdinalIgnoreCase);
    private readonly HubConversationOptions _options;

    public InMemoryConversationSessionStore(IOptions<HubConversationOptions> options)
    {
        _options = options.Value;
    }

    public Task<ConversationSession> GetOrCreateAsync(string? sessionId, string? userId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        CleanupExpiredSessions();

        var normalizedSessionId = string.IsNullOrWhiteSpace(sessionId)
            ? Guid.NewGuid().ToString("N")
            : sessionId.Trim();

        var normalizedUserId = string.IsNullOrWhiteSpace(userId)
            ? "anonymous"
            : userId.Trim();

        var session = _sessions.GetOrAdd(
            normalizedSessionId,
            static (key, state) => new ConversationSession(key, state, DateTimeOffset.UtcNow),
            normalizedUserId);

        return Task.FromResult(session);
    }

    public Task<ConversationSession?> GetAsync(string sessionId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        CleanupExpiredSessions();

        if (string.IsNullOrWhiteSpace(sessionId))
        {
            return Task.FromResult<ConversationSession?>(null);
        }

        _sessions.TryGetValue(sessionId.Trim(), out var session);
        return Task.FromResult(session);
    }

    public Task SaveAsync(ConversationSession session, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _sessions[session.SessionId] = session;
        return Task.CompletedTask;
    }

    private void CleanupExpiredSessions()
    {
        var now = DateTimeOffset.UtcNow;
        var idleLimit = TimeSpan.FromMinutes(Math.Max(1, _options.SessionIdleTimeoutMinutes));

        foreach (var item in _sessions)
        {
            if (now - item.Value.LastActivityUtc > idleLimit)
            {
                _sessions.TryRemove(item.Key, out _);
            }
        }
    }
}




