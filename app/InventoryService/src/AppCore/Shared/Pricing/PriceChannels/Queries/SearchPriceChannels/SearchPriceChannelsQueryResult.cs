namespace Insurance.InventoryService.AppCore.Shared.Pricing.PriceChannels.Queries.SearchPriceChannels;

using Insurance.InventoryService.AppCore.Shared.Pricing.PriceChannels.Queries.Common;

public class SearchPriceChannelsQueryResult
{
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public List<PriceChannelListItem> Items { get; set; } = new();
}
