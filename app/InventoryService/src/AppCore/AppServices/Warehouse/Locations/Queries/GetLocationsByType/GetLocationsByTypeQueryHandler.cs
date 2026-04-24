namespace Insurance.InventoryService.AppCore.AppServices.Warehouse.Locations.Queries.GetLocationsByType;

using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Queries;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Queries.GetLocationsByType;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetLocationsByTypeQueryHandler : QueryHandler<GetLocationsByTypeQuery, GetLocationsByTypeQueryResult>
{
    private readonly ILocationQueryRepository _repository;

    public GetLocationsByTypeQueryHandler(ILocationQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetLocationsByTypeQueryResult>> ExecuteAsync(GetLocationsByTypeQuery request)
    {
        var items = await _repository.GetByTypeAsync(request.LocationType, request.WarehouseRef, request.OnlyActive);
        return QueryResult<GetLocationsByTypeQueryResult>.Success(new GetLocationsByTypeQueryResult
        {
            Items = items
        });
    }
}
