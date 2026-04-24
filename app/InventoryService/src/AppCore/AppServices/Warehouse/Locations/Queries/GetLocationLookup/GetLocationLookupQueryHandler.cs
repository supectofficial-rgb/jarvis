namespace Insurance.InventoryService.AppCore.AppServices.Warehouse.Locations.Queries.GetLocationLookup;

using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Queries;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Queries.GetLocationLookup;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetLocationLookupQueryHandler : QueryHandler<GetLocationLookupQuery, GetLocationLookupQueryResult>
{
    private readonly ILocationQueryRepository _repository;

    public GetLocationLookupQueryHandler(ILocationQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetLocationLookupQueryResult>> ExecuteAsync(GetLocationLookupQuery request)
    {
        var items = await _repository.GetLookupAsync(request.WarehouseRef, request.IncludeInactive);
        return QueryResult<GetLocationLookupQueryResult>.Success(new GetLocationLookupQueryResult
        {
            Items = items
        });
    }
}
