namespace Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Queries.GetSellerLookup;

using OysterFx.AppCore.Shared.Queries;

public class GetSellerLookupQuery : IQuery<GetSellerLookupQueryResult>
{
    public bool IncludeInactive { get; set; }
}
