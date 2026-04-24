namespace Insurance.InventoryService.AppCore.AppServices.SourceTracing.Queries.GetSummary;

using Insurance.InventoryService.AppCore.Shared.SourceTracing.Queries;
using Insurance.InventoryService.AppCore.Shared.SourceTracing.Queries.GetSummary;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetInventorySourceBalanceSummaryQueryHandler
    : QueryHandler<GetInventorySourceBalanceSummaryQuery, GetInventorySourceBalanceSummaryQueryResult>
{
    private readonly IInventorySourceBalanceQueryRepository _repository;

    public GetInventorySourceBalanceSummaryQueryHandler(IInventorySourceBalanceQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetInventorySourceBalanceSummaryQueryResult>> ExecuteAsync(GetInventorySourceBalanceSummaryQuery request)
    {
        var item = await _repository.GetSummaryAsync(request.SourceBalanceBusinessKey);
        if (item is null)
            return QueryResult<GetInventorySourceBalanceSummaryQueryResult>.Fail("Source balance was not found.", "NOT_FOUND");

        return QueryResult<GetInventorySourceBalanceSummaryQueryResult>.Success(
            new GetInventorySourceBalanceSummaryQueryResult { Item = item });
    }
}
