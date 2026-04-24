namespace Insurance.InventoryService.AppCore.AppServices.Warehouse.Locations.Queries.GetById;

using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Queries;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Queries.GetByBusinessKey;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Queries.GetById;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetLocationByIdQueryHandler : QueryHandler<GetLocationByIdQuery, GetLocationByBusinessKeyQueryResult>
{
    private readonly ILocationQueryRepository _repository;

    public GetLocationByIdQueryHandler(ILocationQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetLocationByBusinessKeyQueryResult>> ExecuteAsync(GetLocationByIdQuery request)
    {
        var item = await _repository.GetByIdAsync(request.LocationId);
        if (item is null)
            return QueryResult<GetLocationByBusinessKeyQueryResult>.Fail("Location was not found.", "NOT_FOUND");

        return QueryResult<GetLocationByBusinessKeyQueryResult>.Success(item);
    }
}
