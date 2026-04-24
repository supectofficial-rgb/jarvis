namespace Insurance.InventoryService.AppCore.AppServices.Warehouse.Warehouses.Queries.GetByCode;

using Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Queries;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Queries.GetByBusinessKey;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Queries.GetByCode;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetWarehouseByCodeQueryHandler : QueryHandler<GetWarehouseByCodeQuery, GetWarehouseByBusinessKeyQueryResult>
{
    private readonly IWarehouseQueryRepository _repository;

    public GetWarehouseByCodeQueryHandler(IWarehouseQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetWarehouseByBusinessKeyQueryResult>> ExecuteAsync(GetWarehouseByCodeQuery request)
    {
        if (string.IsNullOrWhiteSpace(request.Code))
            return QueryResult<GetWarehouseByBusinessKeyQueryResult>.Fail("Code is required.", "VALIDATION");

        var item = await _repository.GetByCodeAsync(request.Code);
        if (item is null)
            return QueryResult<GetWarehouseByBusinessKeyQueryResult>.Fail("Warehouse was not found.", "NOT_FOUND");

        return QueryResult<GetWarehouseByBusinessKeyQueryResult>.Success(item);
    }
}
