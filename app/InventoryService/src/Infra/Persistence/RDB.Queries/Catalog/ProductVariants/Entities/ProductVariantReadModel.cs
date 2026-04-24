namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.ProductVariants.Entities;

using Insurance.InventoryService.AppCore.Domain.Catalog.Entities;

public class ProductVariantReadModel
{
    public long Id { get; set; }
    public Guid BusinessKey { get; set; }
    public Guid ProductRef { get; set; }
    public string VariantSku { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public TrackingPolicy TrackingPolicy { get; set; }
    public Guid BaseUomRef { get; set; }
    public bool IsActive { get; set; }
    public bool InventoryMovementLocked { get; set; }
}
