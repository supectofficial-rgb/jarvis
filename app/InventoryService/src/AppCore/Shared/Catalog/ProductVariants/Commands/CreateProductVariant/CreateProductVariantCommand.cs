namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.CreateProductVariant;

using OysterFx.AppCore.Shared.Commands;

public class CreateProductVariantCommand : ICommand<CreateProductVariantCommandResult>
{
    public Guid ProductRef { get; set; }
    public string VariantSku { get; set; } = string.Empty;
    public string? VariantName { get; set; }
    public string? Barcode { get; set; }
    public string TrackingPolicy { get; set; } = string.Empty;
    public Guid BaseUomRef { get; set; }
    public List<CreateVariantAttributeValueItem> AttributeValues { get; set; } = new();
    public List<CreateVariantUomConversionItem> UomConversions { get; set; } = new();
    public List<UpsertVariantImageItem> Images { get; set; } = new();
}

public class CreateVariantAttributeValueItem
{
    public Guid AttributeRef { get; set; }
    public string? Value { get; set; }
    public Guid? OptionRef { get; set; }
}

public class CreateVariantUomConversionItem
{
    public Guid FromUomRef { get; set; }
    public Guid ToUomRef { get; set; }
    public decimal Factor { get; set; }
    public string RoundingMode { get; set; } = string.Empty;
    public bool IsBasePath { get; set; }
}

public class UpsertVariantImageItem
{
    public string FileKey { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string OriginalUrl { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsPrimary { get; set; }
}
