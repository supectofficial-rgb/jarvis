namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.UpsertVariantUomConversion;

using Insurance.InventoryService.AppCore.Domain.Catalog.Entities;

public class UpsertVariantUomConversionCommandResult
{
    public Guid ProductVariantBusinessKey { get; set; }
    public Guid FromUomRef { get; set; }
    public Guid ToUomRef { get; set; }
    public decimal Factor { get; set; }
    public UomRoundingMode RoundingMode { get; set; }
    public bool IsBasePath { get; set; }
}
