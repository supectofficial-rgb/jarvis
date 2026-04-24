namespace Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetBySeller;

using OysterFx.AppCore.Shared.Queries;

public class GetStockDetailsBySellerQuery : IQuery<GetStockDetailsBySellerQueryResult>
{
    public GetStockDetailsBySellerQuery(Guid sellerRef)
    {
        SellerRef = sellerRef;
    }

    public Guid SellerRef { get; }
}
