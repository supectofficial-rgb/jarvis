namespace Insurance.InventoryService.AppCore.AppServices.SourceTracing.Queries.GetAllocationsBySourceBalanceId;

using Insurance.InventoryService.AppCore.Shared.SourceTracing.Queries;
using Insurance.InventoryService.AppCore.Shared.SourceTracing.Queries.GetAllocationsBySourceBalanceId;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetInventorySourceAllocationsBySourceBalanceIdQueryHandler
    : QueryHandler<GetInventorySourceAllocationsBySourceBalanceIdQuery, GetInventorySourceAllocationsBySourceBalanceIdQueryResult>
{
    private readonly IInventorySourceBalanceQueryRepository _repository;

    public GetInventorySourceAllocationsBySourceBalanceIdQueryHandler(IInventorySourceBalanceQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetInventorySourceAllocationsBySourceBalanceIdQueryResult>> ExecuteAsync(GetInventorySourceAllocationsBySourceBalanceIdQuery request)
    {
        var items = await _repository.GetAllocationsBySourceBalanceIdAsync(request.SourceBalanceBusinessKey);
        return QueryResult<GetInventorySourceAllocationsBySourceBalanceIdQueryResult>.Success(
            new GetInventorySourceAllocationsBySourceBalanceIdQueryResult { Items = items });
    }
}
