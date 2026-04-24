namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.Categories.Entities;

public class CategoryAttributeRuleReadModel
{
    public long Id { get; set; }
    public Guid BusinessKey { get; set; }
    public Guid CategorySchemaVersionRef { get; set; }
    public Guid AttributeRef { get; set; }
    public bool IsRequired { get; set; }
    public bool IsVariant { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsOverridden { get; set; }
    public bool IsActive { get; set; }
}
