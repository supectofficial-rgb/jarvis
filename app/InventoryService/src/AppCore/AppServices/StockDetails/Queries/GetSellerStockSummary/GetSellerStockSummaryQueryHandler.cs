namespace Insurance.InventoryService.AppCore.AppServices.StockDetails.Queries.GetSellerStockSummary;

using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetSellerStockSummary;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetSellerStockSummaryQueryHandler : QueryHandler<GetSellerStockSummaryQuery, GetSellerStockSummaryQueryResult>
{
    private readonly IStockDetailQueryRepository _repository;

    public GetSellerStockSummaryQueryHandler(IStockDetailQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetSellerStockSummaryQueryResult>> ExecuteAsync(GetSellerStockSummaryQuery request)
    {
        var item = await _repository.GetSellerSummaryAsync(request.SellerRef);
        if (item is null)
            return QueryResult<GetSellerStockSummaryQueryResult>.Fail("Seller stock summary was not found.", "NOT_FOUND");

        return QueryResult<GetSellerStockSummaryQueryResult>.Success(new GetSellerStockSummaryQueryResult { Item = item });
    }
}
