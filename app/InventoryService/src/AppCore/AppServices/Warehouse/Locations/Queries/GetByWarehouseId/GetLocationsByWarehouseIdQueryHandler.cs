namespace Insurance.InventoryService.AppCore.AppServices.Warehouse.Locations.Queries.GetByWarehouseId;

using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Queries;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Queries.GetByWarehouseId;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetLocationsByWarehouseIdQueryHandler : QueryHandler<GetLocationsByWarehouseIdQuery, GetLocationsByWarehouseIdQueryResult>
{
    private readonly ILocationQueryRepository _repository;

    public GetLocationsByWarehouseIdQueryHandler(ILocationQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetLocationsByWarehouseIdQueryResult>> ExecuteAsync(GetLocationsByWarehouseIdQuery request)
    {
        var items = await _repository.GetByWarehouseIdAsync(request.WarehouseId, onlyActive: false);
        return QueryResult<GetLocationsByWarehouseIdQueryResult>.Success(new GetLocationsByWarehouseIdQueryResult
        {
            Items = items
        });
    }
}
