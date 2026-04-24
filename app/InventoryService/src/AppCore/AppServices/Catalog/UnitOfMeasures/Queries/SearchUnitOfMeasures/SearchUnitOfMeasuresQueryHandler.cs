namespace Insurance.InventoryService.AppCore.AppServices.Catalog.UnitOfMeasures.Queries.SearchUnitOfMeasures;

using Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Queries.SearchUnitOfMeasures;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class SearchUnitOfMeasuresQueryHandler : QueryHandler<SearchUnitOfMeasuresQuery, SearchUnitOfMeasuresQueryResult>
{
    private readonly IUnitOfMeasureQueryRepository _repository;

    public SearchUnitOfMeasuresQueryHandler(IUnitOfMeasureQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<SearchUnitOfMeasuresQueryResult>> ExecuteAsync(SearchUnitOfMeasuresQuery request)
    {
        var result = await _repository.SearchAsync(request);
        return QueryResult<SearchUnitOfMeasuresQueryResult>.Success(result);
    }
}
