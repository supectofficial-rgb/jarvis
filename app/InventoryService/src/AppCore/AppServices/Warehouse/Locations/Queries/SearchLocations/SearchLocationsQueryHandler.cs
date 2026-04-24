namespace Insurance.InventoryService.AppCore.AppServices.Warehouse.Locations.Queries.SearchLocations;

using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Queries;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Queries.SearchLocations;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class SearchLocationsQueryHandler : QueryHandler<SearchLocationsQuery, SearchLocationsQueryResult>
{
    private readonly ILocationQueryRepository _repository;

    public SearchLocationsQueryHandler(ILocationQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<SearchLocationsQueryResult>> ExecuteAsync(SearchLocationsQuery request)
    {
        var result = await _repository.SearchAsync(request);
        return QueryResult<SearchLocationsQueryResult>.Success(result);
    }
}
