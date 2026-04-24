namespace Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Queries.GetByBusinessKey;

using OysterFx.AppCore.Shared.Queries;

public class GetSellerByBusinessKeyQuery : IQuery<GetSellerByBusinessKeyQueryResult>
{
    public GetSellerByBusinessKeyQuery(Guid sellerBusinessKey)
    {
        SellerBusinessKey = sellerBusinessKey;
    }

    public Guid SellerBusinessKey { get; }
}
