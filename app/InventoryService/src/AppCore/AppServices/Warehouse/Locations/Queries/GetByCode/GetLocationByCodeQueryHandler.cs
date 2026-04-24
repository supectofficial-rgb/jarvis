namespace Insurance.InventoryService.AppCore.AppServices.Warehouse.Locations.Queries.GetByCode;

using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Queries;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Queries.GetByBusinessKey;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Queries.GetByCode;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetLocationByCodeQueryHandler : QueryHandler<GetLocationByCodeQuery, GetLocationByBusinessKeyQueryResult>
{
    private readonly ILocationQueryRepository _repository;

    public GetLocationByCodeQueryHandler(ILocationQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetLocationByBusinessKeyQueryResult>> ExecuteAsync(GetLocationByCodeQuery request)
    {
        if (string.IsNullOrWhiteSpace(request.LocationCode))
            return QueryResult<GetLocationByBusinessKeyQueryResult>.Fail("LocationCode is required.", "VALIDATION");

        var item = await _repository.GetByCodeAsync(request.LocationCode);
        if (item is null)
            return QueryResult<GetLocationByBusinessKeyQueryResult>.Fail("Location was not found.", "NOT_FOUND");

        return QueryResult<GetLocationByBusinessKeyQueryResult>.Success(item);
    }
}
