namespace Insurance.InventoryService.AppCore.AppServices.Warehouse.LocationStructures.Queries.GetLocationStructureValues;

using Insurance.InventoryService.AppCore.Shared.Warehouse.LocationStructures.Queries;
using Insurance.InventoryService.AppCore.Shared.Warehouse.LocationStructures.Queries.GetLocationStructureValues;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public sealed class GetLocationStructureValuesQueryHandler : QueryHandler<GetLocationStructureValuesQuery, GetLocationStructureValuesQueryResult>
{
    private readonly ILocationStructureQueryRepository _repository;

    public GetLocationStructureValuesQueryHandler(ILocationStructureQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetLocationStructureValuesQueryResult>> ExecuteAsync(GetLocationStructureValuesQuery request)
    {
        var result = await _repository.GetValuesAsync(request.StructureRef, request.IncludeInactive);
        if (result is null)
            return QueryResult<GetLocationStructureValuesQueryResult>.Fail("Location structure values were not found.", "NOT_FOUND");

        return QueryResult<GetLocationStructureValuesQueryResult>.Success(result);
    }
}
