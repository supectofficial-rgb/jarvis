namespace Insurance.InventoryService.AppCore.Shared.Pricing.PriceTypes.Queries.GetPriceTypeLookup;

using Insurance.InventoryService.AppCore.Shared.Pricing.PriceTypes.Queries.Common;

public class GetPriceTypeLookupQueryResult
{
    public List<PriceTypeLookupItem> Items { get; set; } = new();
}
