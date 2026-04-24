namespace Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries.GetByScope;

using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries.Common;

public class GetAttributeDefinitionsByScopeQueryResult
{
    public List<AttributeDefinitionListItem> Items { get; set; } = new();
}
