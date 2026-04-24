namespace Insurance.InventoryService.AppCore.AppServices.Warehouse.Warehouses.Queries.GetWarehouseLookup;

using Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Queries;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Queries.GetWarehouseLookup;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetWarehouseLookupQueryHandler : QueryHandler<GetWarehouseLookupQuery, GetWarehouseLookupQueryResult>
{
    private readonly IWarehouseQueryRepository _repository;

    public GetWarehouseLookupQueryHandler(IWarehouseQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetWarehouseLookupQueryResult>> ExecuteAsync(GetWarehouseLookupQuery request)
    {
        var items = await _repository.GetLookupAsync(request.IncludeInactive);
        return QueryResult<GetWarehouseLookupQueryResult>.Success(new GetWarehouseLookupQueryResult
        {
            Items = items
        });
    }
}
