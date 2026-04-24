namespace Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetSellerStockSummary;

using OysterFx.AppCore.Shared.Queries;

public class GetSellerStockSummaryQuery : IQuery<GetSellerStockSummaryQueryResult>
{
    public GetSellerStockSummaryQuery(Guid sellerRef)
    {
        SellerRef = sellerRef;
    }

    public Guid SellerRef { get; }
}
