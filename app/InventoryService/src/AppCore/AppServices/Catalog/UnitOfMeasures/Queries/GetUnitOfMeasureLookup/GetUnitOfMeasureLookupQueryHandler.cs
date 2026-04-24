namespace Insurance.InventoryService.AppCore.AppServices.Catalog.UnitOfMeasures.Queries.GetUnitOfMeasureLookup;

using Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Queries.GetUnitOfMeasureLookup;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetUnitOfMeasureLookupQueryHandler : QueryHandler<GetUnitOfMeasureLookupQuery, GetUnitOfMeasureLookupQueryResult>
{
    private readonly IUnitOfMeasureQueryRepository _repository;

    public GetUnitOfMeasureLookupQueryHandler(IUnitOfMeasureQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetUnitOfMeasureLookupQueryResult>> ExecuteAsync(GetUnitOfMeasureLookupQuery request)
    {
        var items = await _repository.GetLookupAsync(request.IncludeInactive);
        return QueryResult<GetUnitOfMeasureLookupQueryResult>.Success(new GetUnitOfMeasureLookupQueryResult
        {
            Items = items
        });
    }
}
