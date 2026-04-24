namespace Insurance.InventoryService.AppCore.AppServices.Warehouse.Locations.Queries.GetByBusinessKey;

using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Queries;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Queries.GetByBusinessKey;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetLocationByBusinessKeyQueryHandler : QueryHandler<GetLocationByBusinessKeyQuery, GetLocationByBusinessKeyQueryResult>
{
    private readonly ILocationQueryRepository _repository;

    public GetLocationByBusinessKeyQueryHandler(ILocationQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetLocationByBusinessKeyQueryResult>> ExecuteAsync(GetLocationByBusinessKeyQuery request)
    {
        var item = await _repository.GetByBusinessKeyAsync(request.LocationBusinessKey);
        if (item is null)
            return QueryResult<GetLocationByBusinessKeyQueryResult>.Fail("Location was not found.", "NOT_FOUND");

        return QueryResult<GetLocationByBusinessKeyQueryResult>.Success(item);
    }
}
