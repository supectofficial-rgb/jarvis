namespace Insurance.GraphService.AppCore.AppServices.GraphNodes.Queries.GetNodesByType;

using Insurance.GraphService.AppCore.Shared.GraphNodes.Queries.GetNodesByType;
using Insurance.GraphService.AppCore.Shared.Graphs.Services;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public sealed class GetNodesByTypeQueryHandler(IGraphStoreService graphStoreService)
    : QueryHandler<GetNodesByTypeQuery, GetNodesByTypeQueryResult>
{
    private readonly IGraphStoreService _graphStoreService = graphStoreService;

    public override async Task<QueryResult<GetNodesByTypeQueryResult>> ExecuteAsync(GetNodesByTypeQuery request)
    {
        if (string.IsNullOrWhiteSpace(request.NodeType))
            return QueryResult<GetNodesByTypeQueryResult>.Fail("NodeType is required.");

        var limit = request.Limit <= 0 ? 100 : Math.Min(request.Limit, 1000);

        var items = await _graphStoreService.GetNodesByTypeAsync(
            request.NodeType.Trim(),
            limit,
            request.EqualsFilters,
            CancellationToken.None);

        return QueryResult<GetNodesByTypeQueryResult>.Success(new GetNodesByTypeQueryResult
        {
            NodeType = request.NodeType.Trim(),
            Items = items.Select(x => new GraphNodeQueryItem
            {
                NodeKey = x.NodeKey,
                Properties = x.Properties
            }).ToList()
        });
    }
}


