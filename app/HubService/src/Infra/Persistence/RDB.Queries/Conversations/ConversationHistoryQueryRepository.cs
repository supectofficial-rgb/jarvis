namespace Insurance.HubService.Infra.Persistence.RDB.Queries.Conversations;

using Insurance.HubService.AppCore.Domain.Conversations.Dtos;
using Insurance.HubService.AppCore.Domain.Conversations.Entities;
using Insurance.HubService.AppCore.Domain.Conversations.Enums;
using Insurance.HubService.AppCore.Shared.Conversations.Queries;
using Insurance.HubService.AppCore.Shared.Conversations.Queries.GetHistory;
using OysterFx.Infra.Persistence.RDB.Queries;
using Microsoft.EntityFrameworkCore;

public class ConversationHistoryQueryRepository(HubServiceQueryDbContext dbContext)
    : QueryRepository<HubServiceQueryDbContext>(dbContext), IConversationHistoryQueryRepository
{
    public async Task<IReadOnlyList<ConversationMessage>> QueryAsync(GetConversationHistoryQuery query)
    {
        if (string.IsNullOrWhiteSpace(query.SessionId))
        {
            return Array.Empty<ConversationMessage>();
        }

        var take = Math.Max(1, query.Take);
        var normalizedSessionId = query.SessionId.Trim();

        var rows = await _dbContext.ConversationMessages
            .AsNoTracking()
            .Where(x => x.SessionId == normalizedSessionId)
            .OrderByDescending(x => x.TimestampUtc)
            .ThenByDescending(x => x.MessageId)
            .Take(take)
            .ToListAsync();

        rows.Reverse();

        return rows
            .Select(x => new ConversationMessage(
                x.MessageId,
                MapRole(x.Role),
                x.Content,
                x.TimestampUtc))
            .ToArray();
    }

    private static ConversationMessageRole MapRole(int role)
        => Enum.IsDefined(typeof(ConversationMessageRole), role)
            ? (ConversationMessageRole)role
            : ConversationMessageRole.Assistant;
}


