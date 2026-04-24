namespace Insurance.InventoryService.AppCore.AppServices.Catalog.UnitOfMeasures.Queries.GetByCode;

using Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Queries.GetByBusinessKey;
using Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Queries.GetByCode;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetUnitOfMeasureByCodeQueryHandler : QueryHandler<GetUnitOfMeasureByCodeQuery, GetUnitOfMeasureByBusinessKeyQueryResult>
{
    private readonly IUnitOfMeasureQueryRepository _repository;

    public GetUnitOfMeasureByCodeQueryHandler(IUnitOfMeasureQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetUnitOfMeasureByBusinessKeyQueryResult>> ExecuteAsync(GetUnitOfMeasureByCodeQuery request)
    {
        if (string.IsNullOrWhiteSpace(request.Code))
            return QueryResult<GetUnitOfMeasureByBusinessKeyQueryResult>.Fail("Code is required.", "VALIDATION");

        var item = await _repository.GetByCodeAsync(request.Code);
        if (item is null)
            return QueryResult<GetUnitOfMeasureByBusinessKeyQueryResult>.Fail("Unit of measure was not found.", "NOT_FOUND");

        return QueryResult<GetUnitOfMeasureByBusinessKeyQueryResult>.Success(item);
    }
}
