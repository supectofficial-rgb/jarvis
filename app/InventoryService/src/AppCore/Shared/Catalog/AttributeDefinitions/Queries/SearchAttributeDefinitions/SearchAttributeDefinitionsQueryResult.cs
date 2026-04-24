namespace Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries.SearchAttributeDefinitions;

using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries.Common;

public class SearchAttributeDefinitionsQueryResult
{
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public List<AttributeDefinitionListItem> Items { get; set; } = new();
}
