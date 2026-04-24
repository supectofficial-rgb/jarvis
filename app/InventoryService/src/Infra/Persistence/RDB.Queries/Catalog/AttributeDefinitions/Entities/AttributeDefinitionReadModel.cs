namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.AttributeDefinitions.Entities;

using Insurance.InventoryService.AppCore.Domain.Catalog.Entities;

public class AttributeDefinitionReadModel
{
    public long Id { get; set; }
    public Guid BusinessKey { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public AttributeDataType DataType { get; set; }
    public AttributeScope Scope { get; set; }
    public bool IsActive { get; set; }
}
