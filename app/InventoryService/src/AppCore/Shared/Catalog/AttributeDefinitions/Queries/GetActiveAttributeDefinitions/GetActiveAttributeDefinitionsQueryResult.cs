namespace Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries.GetActiveAttributeDefinitions;

using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries.Common;

public class GetActiveAttributeDefinitionsQueryResult
{
    public List<AttributeDefinitionListItem> Items { get; set; } = new();
}
