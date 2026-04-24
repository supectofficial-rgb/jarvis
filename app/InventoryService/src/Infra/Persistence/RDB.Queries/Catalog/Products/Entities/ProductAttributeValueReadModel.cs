namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.Products.Entities;

public class ProductAttributeValueReadModel
{
    public long Id { get; set; }
    public Guid BusinessKey { get; set; }
    public Guid ProductRef { get; set; }
    public Guid AttributeRef { get; set; }
    public string? Value { get; set; }
    public Guid? OptionRef { get; set; }
}
