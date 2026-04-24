namespace Insurance.InventoryService.AppCore.AppServices.Catalog.UnitOfMeasures.Queries.GetByBusinessKey;

using Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Queries.GetByBusinessKey;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetUnitOfMeasureByBusinessKeyQueryHandler : QueryHandler<GetUnitOfMeasureByBusinessKeyQuery, GetUnitOfMeasureByBusinessKeyQueryResult>
{
    private readonly IUnitOfMeasureQueryRepository _repository;

    public GetUnitOfMeasureByBusinessKeyQueryHandler(IUnitOfMeasureQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetUnitOfMeasureByBusinessKeyQueryResult>> ExecuteAsync(GetUnitOfMeasureByBusinessKeyQuery request)
    {
        var item = await _repository.GetByBusinessKeyAsync(request.UnitOfMeasureBusinessKey);
        if (item is null)
            return QueryResult<GetUnitOfMeasureByBusinessKeyQueryResult>.Fail("Unit of measure was not found.", "NOT_FOUND");

        return QueryResult<GetUnitOfMeasureByBusinessKeyQueryResult>.Success(item);
    }
}
