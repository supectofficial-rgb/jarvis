namespace Insurance.InventoryService.AppCore.Shared.Pricing.PriceTypes.Queries.SearchPriceTypes;

using Insurance.InventoryService.AppCore.Shared.Pricing.PriceTypes.Queries.Common;

public class SearchPriceTypesQueryResult
{
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public List<PriceTypeListItem> Items { get; set; } = new();
}
