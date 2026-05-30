namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.ProductVariants.Entities;

public class VariantAddOnReadModel
{
    public long Id { get; set; }
    public Guid BusinessKey { get; set; }
    public Guid VariantRef { get; set; }
    public Guid? AddOnVariantRef { get; set; }
    public Guid? TagId { get; set; }
    public bool IsRequired { get; set; }
}
