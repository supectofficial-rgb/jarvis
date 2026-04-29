namespace Insurance.InventoryService.AppCore.Shared.Pricing.PriceTypes.Queries.GetActivePriceTypes;

using Insurance.InventoryService.AppCore.Shared.Pricing.PriceTypes.Queries.Common;

public class GetActivePriceTypesQueryResult
{
    public List<PriceTypeListItem> Items { get; set; } = new();
}
