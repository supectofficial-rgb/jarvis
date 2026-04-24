namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.Products.Entities;

public class ProductReadModel
{
    public long Id { get; set; }
    public Guid BusinessKey { get; set; }
    public Guid CategoryRef { get; set; }
    public Guid CategorySchemaVersionRef { get; set; }
    public string BaseSku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid DefaultUomRef { get; set; }
    public Guid? TaxCategoryRef { get; set; }
    public bool IsActive { get; set; }
}
