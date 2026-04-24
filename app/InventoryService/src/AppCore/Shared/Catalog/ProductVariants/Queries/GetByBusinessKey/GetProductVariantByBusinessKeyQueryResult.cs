namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetByBusinessKey;

public class GetProductVariantByBusinessKeyQueryResult
{
    public Guid ProductVariantBusinessKey { get; set; }
    public Guid ProductRef { get; set; }
    public string VariantSku { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public string TrackingPolicy { get; set; } = string.Empty;
    public Guid BaseUomRef { get; set; }
    public bool IsActive { get; set; }
    public bool InventoryMovementLocked { get; set; }
    public List<VariantAttributeValueResultItem> AttributeValues { get; set; } = new();
    public List<VariantUomConversionResultItem> UomConversions { get; set; } = new();
}

public class VariantAttributeValueResultItem
{
    public Guid AttributeRef { get; set; }
    public string? Value { get; set; }
    public Guid? OptionRef { get; set; }
}

public class VariantUomConversionResultItem
{
    public Guid FromUomRef { get; set; }
    public Guid ToUomRef { get; set; }
    public decimal Factor { get; set; }
    public string RoundingMode { get; set; } = string.Empty;
    public bool IsBasePath { get; set; }
}
