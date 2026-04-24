namespace Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Queries;

using Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Queries.Common;
using Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Queries.SearchSellers;
using Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Queries.GetByBusinessKey;
using OysterFx.AppCore.Shared.Queries;

public interface ISellerQueryRepository : IQueryRepository
{
    Task<GetSellerByBusinessKeyQueryResult?> GetByBusinessKeyAsync(Guid sellerBusinessKey);
    Task<GetSellerByBusinessKeyQueryResult?> GetByIdAsync(Guid sellerId);
    Task<GetSellerByBusinessKeyQueryResult?> GetByCodeAsync(string code);
    Task<SearchSellersQueryResult> SearchAsync(SearchSellersQuery query);
    Task<List<SellerListItem>> GetActiveSellersAsync();
    Task<List<SellerLookupItem>> GetLookupAsync(bool includeInactive = false);
    Task<SellerSummaryItem?> GetSummaryAsync(Guid sellerBusinessKey);
}
