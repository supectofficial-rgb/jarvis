namespace Insurance.HubService.Infra.Persistence.RDB.Commands.Conversations;

using Insurance.HubService.AppCore.Domain.Conversations.Dtos;
using Insurance.HubService.AppCore.Domain.Conversations.Entities;
using Insurance.HubService.AppCore.Domain.Conversations.Enums;
using Insurance.HubService.AppCore.Shared.Conversations.Services;
using Insurance.HubService.Infra.Persistence.RDB.Commands.Conversations.Entities;
using Insurance.HubService.Infra.Persistence.RDB.Commands.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

public sealed class PostgresConversationSessionStore : IConversationSessionStore
{
    private readonly HubServiceCommandDbContext _dbContext;
    private readonly HubConversationPersistenceOptions _options;

    public PostgresConversationSessionStore(
        HubServiceCommandDbContext dbContext,
        IOptions<HubConversationPersistenceOptions> options)
    {
        _dbContext = dbContext;
        _options = options.Value;
    }

    public async Task<ConversationSession> GetOrCreateAsync(string? sessionId, string? userId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await CleanupExpiredSessionsAsync(cancellationToken);

        var normalizedSessionId = string.IsNullOrWhiteSpace(sessionId)
            ? Guid.NewGuid().ToString("N")
            : sessionId.Trim();

        var normalizedUserId = string.IsNullOrWhiteSpace(userId)
            ? "anonymous"
            : userId.Trim();

        var sessionEntity = await _dbContext.ConversationSessions
            .Include(x => x.Messages)
            .FirstOrDefaultAsync(x => x.SessionId == normalizedSessionId, cancellationToken);

        if (sessionEntity is null)
        {
            sessionEntity = new ConversationSessionEntity
            {
                SessionId = normalizedSessionId,
                UserId = normalizedUserId,
                CreatedAtUtc = DateTimeOffset.UtcNow,
                LastActivityUtc = DateTimeOffset.UtcNow
            };

            await _dbContext.ConversationSessions.AddAsync(sessionEntity, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        return ToDomainSession(sessionEntity);
    }

    public async Task<ConversationSession?> GetAsync(string sessionId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await CleanupExpiredSessionsAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(sessionId))
        {
            return null;
        }

        var normalizedSessionId = sessionId.Trim();

        var sessionEntity = await _dbContext.ConversationSessions
            .Include(x => x.Messages)
            .FirstOrDefaultAsync(x => x.SessionId == normalizedSessionId, cancellationToken);

        return sessionEntity is null ? null : ToDomainSession(sessionEntity);
    }

    public async Task SaveAsync(ConversationSession session, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var sessionEntity = await _dbContext.ConversationSessions
            .Include(x => x.Messages)
            .FirstOrDefaultAsync(x => x.SessionId == session.SessionId, cancellationToken);

        if (sessionEntity is null)
        {
            sessionEntity = new ConversationSessionEntity
            {
                SessionId = session.SessionId,
                UserId = session.UserId,
                CreatedAtUtc = session.CreatedAtUtc,
                LastActivityUtc = session.LastActivityUtc
            };

            await _dbContext.ConversationSessions.AddAsync(sessionEntity, cancellationToken);
        }
        else
        {
            sessionEntity.UserId = session.UserId;
            sessionEntity.LastActivityUtc = session.LastActivityUtc;
        }

        var domainMessages = session.GetMessages(int.MaxValue);
        var messageIds = domainMessages.Select(x => x.MessageId).ToHashSet(StringComparer.OrdinalIgnoreCase);

        var removableMessages = sessionEntity.Messages
            .Where(x => !messageIds.Contains(x.MessageId))
            .ToList();

        if (removableMessages.Count > 0)
        {
            _dbContext.ConversationMessages.RemoveRange(removableMessages);
        }

        var existingMessages = sessionEntity.Messages
            .ToDictionary(x => x.MessageId, StringComparer.OrdinalIgnoreCase);

        foreach (var message in domainMessages)
        {
            if (existingMessages.TryGetValue(message.MessageId, out var existingEntity))
            {
                existingEntity.Content = message.Content;
                existingEntity.Role = message.Role;
                existingEntity.TimestampUtc = message.TimestampUtc;
                continue;
            }

            sessionEntity.Messages.Add(new ConversationMessageEntity
            {
                MessageId = message.MessageId,
                SessionId = session.SessionId,
                Role = message.Role,
                Content = message.Content,
                TimestampUtc = message.TimestampUtc
            });
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task CleanupExpiredSessionsAsync(CancellationToken cancellationToken)
    {
        var idleLimit = TimeSpan.FromMinutes(Math.Max(1, _options.SessionIdleTimeoutMinutes));
        var expirationThreshold = DateTimeOffset.UtcNow.Subtract(idleLimit);

        var expiredSessions = await _dbContext.ConversationSessions
            .Where(x => x.LastActivityUtc < expirationThreshold)
            .ToListAsync(cancellationToken);

        if (expiredSessions.Count == 0)
        {
            return;
        }

        _dbContext.ConversationSessions.RemoveRange(expiredSessions);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private static ConversationSession ToDomainSession(ConversationSessionEntity entity)
    {
        var session = new ConversationSession(entity.SessionId, entity.UserId, entity.CreatedAtUtc);

        foreach (var message in entity.Messages.OrderBy(x => x.TimestampUtc).ThenBy(x => x.MessageId, StringComparer.Ordinal))
        {
            session.AppendMessage(message.Role, message.Content, message.TimestampUtc, message.MessageId);
        }

        return session;
    }
}



