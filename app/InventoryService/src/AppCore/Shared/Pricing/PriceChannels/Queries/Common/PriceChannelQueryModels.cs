namespace Insurance.InventoryService.AppCore.Shared.Pricing.PriceChannels.Queries.Common;

public class PriceChannelListItem
{
    public Guid PriceChannelBusinessKey { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public sealed class PriceChannelLookupItem : PriceChannelListItem
{
}
