namespace Insurance.InventoryService.AppCore.AppServices.Catalog.UnitOfMeasures.Queries.GetById;

using Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Queries.GetByBusinessKey;
using Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Queries.GetById;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetUnitOfMeasureByIdQueryHandler : QueryHandler<GetUnitOfMeasureByIdQuery, GetUnitOfMeasureByBusinessKeyQueryResult>
{
    private readonly IUnitOfMeasureQueryRepository _repository;

    public GetUnitOfMeasureByIdQueryHandler(IUnitOfMeasureQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetUnitOfMeasureByBusinessKeyQueryResult>> ExecuteAsync(GetUnitOfMeasureByIdQuery request)
    {
        var item = await _repository.GetByIdAsync(request.UnitOfMeasureId);
        if (item is null)
            return QueryResult<GetUnitOfMeasureByBusinessKeyQueryResult>.Fail("Unit of measure was not found.", "NOT_FOUND");

        return QueryResult<GetUnitOfMeasureByBusinessKeyQueryResult>.Success(item);
    }
}
