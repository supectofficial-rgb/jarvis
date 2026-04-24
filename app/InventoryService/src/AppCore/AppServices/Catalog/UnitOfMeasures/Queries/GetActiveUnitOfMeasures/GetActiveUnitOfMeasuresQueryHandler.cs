namespace Insurance.InventoryService.AppCore.AppServices.Catalog.UnitOfMeasures.Queries.GetActiveUnitOfMeasures;

using Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Queries.GetActiveUnitOfMeasures;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetActiveUnitOfMeasuresQueryHandler : QueryHandler<GetActiveUnitOfMeasuresQuery, GetActiveUnitOfMeasuresQueryResult>
{
    private readonly IUnitOfMeasureQueryRepository _repository;

    public GetActiveUnitOfMeasuresQueryHandler(IUnitOfMeasureQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetActiveUnitOfMeasuresQueryResult>> ExecuteAsync(GetActiveUnitOfMeasuresQuery request)
    {
        var items = await _repository.GetActiveUnitOfMeasuresAsync();
        return QueryResult<GetActiveUnitOfMeasuresQueryResult>.Success(new GetActiveUnitOfMeasuresQueryResult
        {
            Items = items
        });
    }
}
