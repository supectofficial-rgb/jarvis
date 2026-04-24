namespace Insurance.InventoryService.AppCore.AppServices.Warehouse.Warehouses.Queries.GetWarehouseWithLocations;

using Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Queries;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Queries.GetWarehouseWithLocations;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetWarehouseWithLocationsQueryHandler : QueryHandler<GetWarehouseWithLocationsQuery, GetWarehouseWithLocationsQueryResult>
{
    private readonly IWarehouseQueryRepository _repository;

    public GetWarehouseWithLocationsQueryHandler(IWarehouseQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetWarehouseWithLocationsQueryResult>> ExecuteAsync(GetWarehouseWithLocationsQuery request)
    {
        var item = await _repository.GetWithLocationsAsync(request.WarehouseBusinessKey, request.IncludeInactiveLocations);
        if (item is null)
            return QueryResult<GetWarehouseWithLocationsQueryResult>.Fail("Warehouse was not found.", "NOT_FOUND");

        return QueryResult<GetWarehouseWithLocationsQueryResult>.Success(new GetWarehouseWithLocationsQueryResult
        {
            Item = item
        });
    }
}
