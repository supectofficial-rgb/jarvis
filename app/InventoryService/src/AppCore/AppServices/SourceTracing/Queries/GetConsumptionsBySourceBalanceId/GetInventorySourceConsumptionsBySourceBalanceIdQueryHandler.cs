namespace Insurance.InventoryService.AppCore.AppServices.SourceTracing.Queries.GetConsumptionsBySourceBalanceId;

using Insurance.InventoryService.AppCore.Shared.SourceTracing.Queries;
using Insurance.InventoryService.AppCore.Shared.SourceTracing.Queries.GetConsumptionsBySourceBalanceId;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetInventorySourceConsumptionsBySourceBalanceIdQueryHandler
    : QueryHandler<GetInventorySourceConsumptionsBySourceBalanceIdQuery, GetInventorySourceConsumptionsBySourceBalanceIdQueryResult>
{
    private readonly IInventorySourceBalanceQueryRepository _repository;

    public GetInventorySourceConsumptionsBySourceBalanceIdQueryHandler(IInventorySourceBalanceQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetInventorySourceConsumptionsBySourceBalanceIdQueryResult>> ExecuteAsync(GetInventorySourceConsumptionsBySourceBalanceIdQuery request)
    {
        var items = await _repository.GetConsumptionsBySourceBalanceIdAsync(request.SourceBalanceBusinessKey);
        return QueryResult<GetInventorySourceConsumptionsBySourceBalanceIdQueryResult>.Success(
            new GetInventorySourceConsumptionsBySourceBalanceIdQueryResult { Items = items });
    }
}
