namespace Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Queries.GetSellerSummary;

using OysterFx.AppCore.Shared.Queries;

public class GetSellerSummaryQuery : IQuery<GetSellerSummaryQueryResult>
{
    public GetSellerSummaryQuery(Guid sellerBusinessKey)
    {
        SellerBusinessKey = sellerBusinessKey;
    }

    public Guid SellerBusinessKey { get; }
}
