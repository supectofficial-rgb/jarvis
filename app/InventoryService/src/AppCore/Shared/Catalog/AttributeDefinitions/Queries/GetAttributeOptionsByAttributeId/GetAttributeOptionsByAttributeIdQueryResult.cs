namespace Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries.GetAttributeOptionsByAttributeId;

using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries.Common;

public class GetAttributeOptionsByAttributeIdQueryResult
{
    public List<AttributeOptionListItem> Items { get; set; } = new();
}
