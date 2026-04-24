namespace Insurance.InventoryService.AppCore.AppServices.StockDetails.Queries.GetBySeller;

using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetBySeller;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetStockDetailsBySellerQueryHandler : QueryHandler<GetStockDetailsBySellerQuery, GetStockDetailsBySellerQueryResult>
{
    private readonly IStockDetailQueryRepository _repository;

    public GetStockDetailsBySellerQueryHandler(IStockDetailQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetStockDetailsBySellerQueryResult>> ExecuteAsync(GetStockDetailsBySellerQuery request)
    {
        var items = await _repository.GetBySellerAsync(request.SellerRef);
        return QueryResult<GetStockDetailsBySellerQueryResult>.Success(new GetStockDetailsBySellerQueryResult { Items = items });
    }
}
