namespace Insurance.InventoryService.AppCore.AppServices.Warehouse.Locations.Queries.GetActiveByWarehouseId;

using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Queries;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Queries.GetActiveByWarehouseId;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetActiveLocationsByWarehouseIdQueryHandler : QueryHandler<GetActiveLocationsByWarehouseIdQuery, GetActiveLocationsByWarehouseIdQueryResult>
{
    private readonly ILocationQueryRepository _repository;

    public GetActiveLocationsByWarehouseIdQueryHandler(ILocationQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetActiveLocationsByWarehouseIdQueryResult>> ExecuteAsync(GetActiveLocationsByWarehouseIdQuery request)
    {
        var items = await _repository.GetByWarehouseIdAsync(request.WarehouseId, onlyActive: true);
        return QueryResult<GetActiveLocationsByWarehouseIdQueryResult>.Success(new GetActiveLocationsByWarehouseIdQueryResult
        {
            Items = items
        });
    }
}
