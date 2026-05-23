namespace Insurance.InventoryService.AppCore.AppServices.Warehouse.LocationStructures.Queries.GetWarehouseLocationStructureTree;

using Insurance.InventoryService.AppCore.Shared.Warehouse.LocationStructures.Queries;
using Insurance.InventoryService.AppCore.Shared.Warehouse.LocationStructures.Queries.GetWarehouseLocationStructureTree;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public sealed class GetWarehouseLocationStructureTreeQueryHandler : QueryHandler<GetWarehouseLocationStructureTreeQuery, GetWarehouseLocationStructureTreeQueryResult>
{
    private readonly ILocationStructureQueryRepository _repository;

    public GetWarehouseLocationStructureTreeQueryHandler(ILocationStructureQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetWarehouseLocationStructureTreeQueryResult>> ExecuteAsync(GetWarehouseLocationStructureTreeQuery request)
    {
        var result = await _repository.GetTreeAsync(request.WarehouseRef, request.IncludeInactive);
        if (result is null)
            return QueryResult<GetWarehouseLocationStructureTreeQueryResult>.Fail("Location structure tree was not found.", "NOT_FOUND");

        return QueryResult<GetWarehouseLocationStructureTreeQueryResult>.Success(result);
    }
}
