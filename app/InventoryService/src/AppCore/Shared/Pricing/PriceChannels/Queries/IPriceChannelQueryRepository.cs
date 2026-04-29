namespace Insurance.InventoryService.AppCore.Shared.Pricing.PriceChannels.Queries;

using Insurance.InventoryService.AppCore.Shared.Pricing.PriceChannels.Queries.GetByBusinessKey;
using Insurance.InventoryService.AppCore.Shared.Pricing.PriceChannels.Queries.Common;
using Insurance.InventoryService.AppCore.Shared.Pricing.PriceChannels.Queries.SearchPriceChannels;
using OysterFx.AppCore.Shared.Queries;

public interface IPriceChannelQueryRepository : IQueryRepository
{
    Task<GetPriceChannelByBusinessKeyQueryResult?> GetByBusinessKeyAsync(Guid priceChannelBusinessKey);
    Task<SearchPriceChannelsQueryResult> SearchAsync(SearchPriceChannelsQuery query);
    Task<List<PriceChannelListItem>> GetActiveAsync();
    Task<List<PriceChannelLookupItem>> GetLookupAsync(bool includeInactive = false);
}
