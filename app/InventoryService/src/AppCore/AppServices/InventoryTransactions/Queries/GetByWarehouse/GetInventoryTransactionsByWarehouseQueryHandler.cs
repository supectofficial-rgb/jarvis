namespace Insurance.InventoryService.AppCore.AppServices.InventoryTransactions.Queries.GetByWarehouse;

using Insurance.InventoryService.AppCore.Shared.InventoryTransactions.Queries;
using Insurance.InventoryService.AppCore.Shared.InventoryTransactions.Queries.GetByWarehouse;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetInventoryTransactionsByWarehouseQueryHandler
    : QueryHandler<GetInventoryTransactionsByWarehouseQuery, GetInventoryTransactionsByWarehouseQueryResult>
{
    private readonly IInventoryTransactionQueryRepository _repository;

    public GetInventoryTransactionsByWarehouseQueryHandler(IInventoryTransactionQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetInventoryTransactionsByWarehouseQueryResult>> ExecuteAsync(GetInventoryTransactionsByWarehouseQuery request)
    {
        var items = await _repository.GetByWarehouseAsync(request.WarehouseRef);
        return QueryResult<GetInventoryTransactionsByWarehouseQueryResult>.Success(new GetInventoryTransactionsByWarehouseQueryResult { Items = items });
    }
}
