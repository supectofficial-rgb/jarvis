namespace Insurance.InventoryService.AppCore.Shared.Pricing.PriceChannels.Queries;

using Insurance.InventoryService.AppCore.Shared.Pricing.PriceChannels.Queries.GetByBusinessKey;
using OysterFx.AppCore.Shared.Queries;

public interface IPriceChannelQueryRepository : IQueryRepository
{
    Task<GetPriceChannelByBusinessKeyQueryResult?> GetByBusinessKeyAsync(Guid priceChannelBusinessKey);
}
