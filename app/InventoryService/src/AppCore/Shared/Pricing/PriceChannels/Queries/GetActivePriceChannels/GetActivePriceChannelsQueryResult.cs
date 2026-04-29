namespace Insurance.InventoryService.AppCore.Shared.Pricing.PriceChannels.Queries.GetActivePriceChannels;

using Insurance.InventoryService.AppCore.Shared.Pricing.PriceChannels.Queries.Common;

public class GetActivePriceChannelsQueryResult
{
    public List<PriceChannelListItem> Items { get; set; } = new();
}
