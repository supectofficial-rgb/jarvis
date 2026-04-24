namespace Insurance.InventoryService.AppCore.AppServices.Warehouse.Warehouses.Queries.GetById;

using Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Queries;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Queries.GetByBusinessKey;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Queries.GetById;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetWarehouseByIdQueryHandler : QueryHandler<GetWarehouseByIdQuery, GetWarehouseByBusinessKeyQueryResult>
{
    private readonly IWarehouseQueryRepository _repository;

    public GetWarehouseByIdQueryHandler(IWarehouseQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetWarehouseByBusinessKeyQueryResult>> ExecuteAsync(GetWarehouseByIdQuery request)
    {
        var item = await _repository.GetByIdAsync(request.WarehouseId);
        if (item is null)
            return QueryResult<GetWarehouseByBusinessKeyQueryResult>.Fail("Warehouse was not found.", "NOT_FOUND");

        return QueryResult<GetWarehouseByBusinessKeyQueryResult>.Success(item);
    }
}
