namespace Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Queries.SearchSellers;

using OysterFx.AppCore.Shared.Queries;

public class SearchSellersQuery : IQuery<SearchSellersQueryResult>
{
    public string? Code { get; set; }
    public string? Name { get; set; }
    public bool? IsSystemOwner { get; set; }
    public bool? IsActive { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
