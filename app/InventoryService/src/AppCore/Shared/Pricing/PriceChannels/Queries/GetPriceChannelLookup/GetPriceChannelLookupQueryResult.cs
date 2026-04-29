namespace Insurance.InventoryService.AppCore.Shared.Pricing.PriceChannels.Queries.GetPriceChannelLookup;

using Insurance.InventoryService.AppCore.Shared.Pricing.PriceChannels.Queries.Common;

public class GetPriceChannelLookupQueryResult
{
    public List<PriceChannelLookupItem> Items { get; set; } = new();
}
