namespace Insurance.InventoryService.AppCore.AppServices.Warehouse.Warehouses.Queries.GetActiveWarehouses;

using Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Queries;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Queries.GetActiveWarehouses;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetActiveWarehousesQueryHandler : QueryHandler<GetActiveWarehousesQuery, GetActiveWarehousesQueryResult>
{
    private readonly IWarehouseQueryRepository _repository;

    public GetActiveWarehousesQueryHandler(IWarehouseQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetActiveWarehousesQueryResult>> ExecuteAsync(GetActiveWarehousesQuery request)
    {
        var items = await _repository.GetActiveWarehousesAsync();
        return QueryResult<GetActiveWarehousesQueryResult>.Success(new GetActiveWarehousesQueryResult
        {
            Items = items
        });
    }
}
