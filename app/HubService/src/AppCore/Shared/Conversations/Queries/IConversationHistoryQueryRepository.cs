namespace Insurance.HubService.AppCore.Shared.Conversations.Queries;

using Insurance.HubService.AppCore.Domain.Conversations.Dtos;
using Insurance.HubService.AppCore.Domain.Conversations.Entities;
using Insurance.HubService.AppCore.Domain.Conversations.Enums;
using Insurance.HubService.AppCore.Shared.Conversations.Queries.GetHistory;
using OysterFx.AppCore.Shared.Queries;

public interface IConversationHistoryQueryRepository : IQueryRepository
{
    Task<IReadOnlyList<ConversationMessage>> QueryAsync(GetConversationHistoryQuery query);
}


