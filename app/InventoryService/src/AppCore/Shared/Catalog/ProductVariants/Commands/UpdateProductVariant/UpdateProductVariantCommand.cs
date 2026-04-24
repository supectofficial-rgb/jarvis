namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.UpdateProductVariant;

using OysterFx.AppCore.Shared.Commands;

public class UpdateProductVariantCommand : ICommand<UpdateProductVariantCommandResult>
{
    public Guid ProductVariantBusinessKey { get; set; }
    public string VariantSku { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public string TrackingPolicy { get; set; } = string.Empty;
    public Guid BaseUomRef { get; set; }
    public bool IsActive { get; set; } = true;
    public List<UpdateVariantAttributeValueItem> AttributeValues { get; set; } = new();
    public List<UpdateVariantUomConversionItem> UomConversions { get; set; } = new();
}

public class UpdateVariantAttributeValueItem
{
    public Guid AttributeRef { get; set; }
    public string? Value { get; set; }
    public Guid? OptionRef { get; set; }
}

public class UpdateVariantUomConversionItem
{
    public Guid FromUomRef { get; set; }
    public Guid ToUomRef { get; set; }
    public decimal Factor { get; set; }
    public string RoundingMode { get; set; } = string.Empty;
    public bool IsBasePath { get; set; }
}
