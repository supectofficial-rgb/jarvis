namespace Insurance.InventoryService.AppCore.AppServices.Warehouse.Warehouses.Queries.SearchWarehouses;

using Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Queries;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Queries.SearchWarehouses;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class SearchWarehousesQueryHandler : QueryHandler<SearchWarehousesQuery, SearchWarehousesQueryResult>
{
    private readonly IWarehouseQueryRepository _repository;

    public SearchWarehousesQueryHandler(IWarehouseQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<SearchWarehousesQueryResult>> ExecuteAsync(SearchWarehousesQuery request)
    {
        var result = await _repository.SearchAsync(request);
        return QueryResult<SearchWarehousesQueryResult>.Success(result);
    }
}
