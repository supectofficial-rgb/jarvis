namespace Insurance.InventoryService.AppCore.AppServices.SourceTracing.Queries.GetConsumptionsByTransactionLine;

using Insurance.InventoryService.AppCore.Shared.SourceTracing.Queries;
using Insurance.InventoryService.AppCore.Shared.SourceTracing.Queries.GetConsumptionsByTransactionLine;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetInventorySourceConsumptionsByTransactionLineQueryHandler
    : QueryHandler<GetInventorySourceConsumptionsByTransactionLineQuery, GetInventorySourceConsumptionsByTransactionLineQueryResult>
{
    private readonly IInventorySourceBalanceQueryRepository _repository;

    public GetInventorySourceConsumptionsByTransactionLineQueryHandler(IInventorySourceBalanceQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetInventorySourceConsumptionsByTransactionLineQueryResult>> ExecuteAsync(GetInventorySourceConsumptionsByTransactionLineQuery request)
    {
        var items = await _repository.GetConsumptionsByTransactionLineAsync(request.OutboundTransactionLineRef);
        return QueryResult<GetInventorySourceConsumptionsByTransactionLineQueryResult>.Success(
            new GetInventorySourceConsumptionsByTransactionLineQueryResult { Items = items });
    }
}
