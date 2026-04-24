namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.ProductVariants.Entities;

using Insurance.InventoryService.AppCore.Domain.Catalog.Entities;

public class VariantUomConversionReadModel
{
    public long Id { get; set; }
    public Guid BusinessKey { get; set; }
    public Guid VariantRef { get; set; }
    public Guid FromUomRef { get; set; }
    public Guid ToUomRef { get; set; }
    public decimal Factor { get; set; }
    public UomRoundingMode RoundingMode { get; set; }
    public bool IsBasePath { get; set; }
}
