namespace Insurance.InventoryService.AppCore.AppServices.StockDetails.Queries.GetByWarehouse;

using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetByWarehouse;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetStockDetailsByWarehouseQueryHandler : QueryHandler<GetStockDetailsByWarehouseQuery, GetStockDetailsByWarehouseQueryResult>
{
    private readonly IStockDetailQueryRepository _repository;

    public GetStockDetailsByWarehouseQueryHandler(IStockDetailQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetStockDetailsByWarehouseQueryResult>> ExecuteAsync(GetStockDetailsByWarehouseQuery request)
    {
        var items = await _repository.GetByWarehouseAsync(request.WarehouseRef);
        return QueryResult<GetStockDetailsByWarehouseQueryResult>.Success(new GetStockDetailsByWarehouseQueryResult { Items = items });
    }
}
