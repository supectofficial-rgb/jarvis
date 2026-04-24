namespace Insurance.InventoryService.AppCore.AppServices.StockDetails.Queries.GetVariantStockSummary;

using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetVariantStockSummary;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetVariantStockSummaryQueryHandler : QueryHandler<GetVariantStockSummaryQuery, GetVariantStockSummaryQueryResult>
{
    private readonly IStockDetailQueryRepository _repository;

    public GetVariantStockSummaryQueryHandler(IStockDetailQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetVariantStockSummaryQueryResult>> ExecuteAsync(GetVariantStockSummaryQuery request)
    {
        var item = await _repository.GetVariantSummaryAsync(request.VariantRef);
        if (item is null)
            return QueryResult<GetVariantStockSummaryQueryResult>.Fail("Variant stock summary was not found.", "NOT_FOUND");

        return QueryResult<GetVariantStockSummaryQueryResult>.Success(new GetVariantStockSummaryQueryResult { Item = item });
    }
}
