namespace Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries.SearchAttributeOptions;

using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries.Common;

public class SearchAttributeOptionsQueryResult
{
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public List<AttributeOptionListItem> Items { get; set; } = new();
}
