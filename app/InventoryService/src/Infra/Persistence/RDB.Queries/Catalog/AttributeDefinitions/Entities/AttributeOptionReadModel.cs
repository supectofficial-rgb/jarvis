namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.AttributeDefinitions.Entities;

public class AttributeOptionReadModel
{
    public long Id { get; set; }
    public Guid BusinessKey { get; set; }
    public Guid AttributeRef { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}
