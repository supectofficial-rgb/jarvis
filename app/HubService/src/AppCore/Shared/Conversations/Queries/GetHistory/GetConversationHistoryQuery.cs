namespace Insurance.HubService.AppCore.Shared.Conversations.Queries.GetHistory;

using Insurance.HubService.AppCore.Domain.Conversations.Dtos;
using Insurance.HubService.AppCore.Domain.Conversations.Entities;
using Insurance.HubService.AppCore.Domain.Conversations.Enums;
using OysterFx.AppCore.Shared.Queries;

public class GetConversationHistoryQuery : IQuery<IReadOnlyList<ConversationMessage>>
{
    public string SessionId { get; set; } = string.Empty;
    public int Take { get; set; } = 20;
}


