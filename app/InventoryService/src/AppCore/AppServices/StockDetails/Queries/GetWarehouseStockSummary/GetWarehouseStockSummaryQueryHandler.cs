namespace Insurance.InventoryService.AppCore.AppServices.StockDetails.Queries.GetWarehouseStockSummary;

using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetWarehouseStockSummary;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetWarehouseStockSummaryQueryHandler : QueryHandler<GetWarehouseStockSummaryQuery, GetWarehouseStockSummaryQueryResult>
{
    private readonly IStockDetailQueryRepository _repository;

    public GetWarehouseStockSummaryQueryHandler(IStockDetailQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetWarehouseStockSummaryQueryResult>> ExecuteAsync(GetWarehouseStockSummaryQuery request)
    {
        var item = await _repository.GetWarehouseSummaryAsync(request.WarehouseRef);
        if (item is null)
            return QueryResult<GetWarehouseStockSummaryQueryResult>.Fail("Warehouse stock summary was not found.", "NOT_FOUND");

        return QueryResult<GetWarehouseStockSummaryQueryResult>.Success(new GetWarehouseStockSummaryQueryResult { Item = item });
    }
}
