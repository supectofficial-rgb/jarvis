namespace Insurance.InventoryService.AppCore.AppServices.Warehouse.Warehouses.Queries.GetWarehouseSummary;

using Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Queries;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Queries.GetWarehouseSummary;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetWarehouseSummaryQueryHandler : QueryHandler<GetWarehouseSummaryQuery, GetWarehouseSummaryQueryResult>
{
    private readonly IWarehouseQueryRepository _repository;

    public GetWarehouseSummaryQueryHandler(IWarehouseQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetWarehouseSummaryQueryResult>> ExecuteAsync(GetWarehouseSummaryQuery request)
    {
        var item = await _repository.GetSummaryAsync(request.WarehouseBusinessKey);
        if (item is null)
            return QueryResult<GetWarehouseSummaryQueryResult>.Fail("Warehouse was not found.", "NOT_FOUND");

        return QueryResult<GetWarehouseSummaryQueryResult>.Success(new GetWarehouseSummaryQueryResult
        {
            Item = item
        });
    }
}
