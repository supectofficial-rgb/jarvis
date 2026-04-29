namespace Insurance.InventoryService.AppCore.Shared.Pricing.PriceTypes.Queries;

using Insurance.InventoryService.AppCore.Shared.Pricing.PriceTypes.Queries.GetByBusinessKey;
using Insurance.InventoryService.AppCore.Shared.Pricing.PriceTypes.Queries.Common;
using Insurance.InventoryService.AppCore.Shared.Pricing.PriceTypes.Queries.SearchPriceTypes;
using OysterFx.AppCore.Shared.Queries;

public interface IPriceTypeQueryRepository : IQueryRepository
{
    Task<GetPriceTypeByBusinessKeyQueryResult?> GetByBusinessKeyAsync(Guid priceTypeBusinessKey);
    Task<SearchPriceTypesQueryResult> SearchAsync(SearchPriceTypesQuery query);
    Task<List<PriceTypeListItem>> GetActiveAsync();
    Task<List<PriceTypeLookupItem>> GetLookupAsync(bool includeInactive = false);
}
