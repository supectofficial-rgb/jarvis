namespace Insurance.HubService.AppCore.AppServices.Conversations.Queries.GetHistory;

using Insurance.HubService.AppCore.Domain.Conversations.Dtos;
using Insurance.HubService.AppCore.Domain.Conversations.Entities;
using Insurance.HubService.AppCore.Domain.Conversations.Enums;
using Insurance.HubService.AppCore.Shared.Conversations.Queries;
using Insurance.HubService.AppCore.Shared.Conversations.Queries.GetHistory;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetConversationHistoryQueryHandler(IConversationHistoryQueryRepository conversationHistoryQueryRepository)
    : QueryHandler<GetConversationHistoryQuery, IReadOnlyList<ConversationMessage>>
{
    private readonly IConversationHistoryQueryRepository _conversationHistoryQueryRepository = conversationHistoryQueryRepository;

    public override async Task<QueryResult<IReadOnlyList<ConversationMessage>>> ExecuteAsync(GetConversationHistoryQuery request)
    {
        var result = await _conversationHistoryQueryRepository.QueryAsync(request);
        return await AsQueryResult(result);
    }
}


