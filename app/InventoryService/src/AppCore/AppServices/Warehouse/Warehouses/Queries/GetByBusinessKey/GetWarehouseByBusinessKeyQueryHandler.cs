namespace Insurance.InventoryService.AppCore.AppServices.Warehouse.Warehouses.Queries.GetByBusinessKey;

using Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Queries;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Queries.GetByBusinessKey;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetWarehouseByBusinessKeyQueryHandler : QueryHandler<GetWarehouseByBusinessKeyQuery, GetWarehouseByBusinessKeyQueryResult>
{
    private readonly IWarehouseQueryRepository _repository;

    public GetWarehouseByBusinessKeyQueryHandler(IWarehouseQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetWarehouseByBusinessKeyQueryResult>> ExecuteAsync(GetWarehouseByBusinessKeyQuery request)
    {
        var item = await _repository.GetByBusinessKeyAsync(request.WarehouseBusinessKey);
        if (item is null)
            return QueryResult<GetWarehouseByBusinessKeyQueryResult>.Fail("Warehouse was not found.", "NOT_FOUND");

        return QueryResult<GetWarehouseByBusinessKeyQueryResult>.Success(item);
    }
}
