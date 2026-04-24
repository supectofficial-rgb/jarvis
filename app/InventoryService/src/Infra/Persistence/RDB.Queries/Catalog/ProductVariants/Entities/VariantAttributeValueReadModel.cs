namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.ProductVariants.Entities;

public class VariantAttributeValueReadModel
{
    public long Id { get; set; }
    public Guid BusinessKey { get; set; }
    public Guid VariantRef { get; set; }
    public Guid AttributeRef { get; set; }
    public string? Value { get; set; }
    public Guid? OptionRef { get; set; }
}
