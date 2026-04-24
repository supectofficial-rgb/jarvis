namespace Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries.GetActiveAttributeOptionsByAttributeId;

using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries.Common;

public class GetActiveAttributeOptionsByAttributeIdQueryResult
{
    public List<AttributeOptionListItem> Items { get; set; } = new();
}
