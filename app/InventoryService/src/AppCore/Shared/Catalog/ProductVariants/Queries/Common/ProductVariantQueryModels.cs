namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.Common;

public class VariantListItem
{
    public Guid VariantBusinessKey { get; set; }
    public Guid ProductRef { get; set; }
    public string VariantSku { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public string TrackingPolicy { get; set; } = string.Empty;
    public Guid BaseUomRef { get; set; }
    public bool IsActive { get; set; }
    public bool InventoryMovementLocked { get; set; }
}

public class VariantAttributeValueViewItem
{
    public Guid VariantAttributeValueBusinessKey { get; set; }
    public Guid VariantRef { get; set; }
    public Guid AttributeRef { get; set; }
    public string? Value { get; set; }
    public Guid? OptionRef { get; set; }
}

public class VariantAttributeValueWithDefinitionItem
{
    public Guid VariantAttributeValueBusinessKey { get; set; }
    public Guid VariantRef { get; set; }
    public Guid AttributeRef { get; set; }
    public string AttributeCode { get; set; } = string.Empty;
    public string AttributeName { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public string Scope { get; set; } = string.Empty;
    public string? Value { get; set; }
    public Guid? OptionRef { get; set; }
    public string? OptionValue { get; set; }
}

public class VariantUomConversionViewItem
{
    public Guid VariantUomConversionBusinessKey { get; set; }
    public Guid VariantRef { get; set; }
    public Guid FromUomRef { get; set; }
    public Guid ToUomRef { get; set; }
    public decimal Factor { get; set; }
    public string RoundingMode { get; set; } = string.Empty;
    public bool IsBasePath { get; set; }
}

public class MissingRequiredVariantAttributeItem
{
    public Guid AttributeRef { get; set; }
    public string AttributeCode { get; set; } = string.Empty;
    public string AttributeName { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public string Scope { get; set; } = string.Empty;
}

public class VariantSummaryItem
{
    public Guid VariantBusinessKey { get; set; }
    public string VariantSku { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public bool IsActive { get; set; }
    public int AttributeValueCount { get; set; }
}

public class VariantDetailsWithAttributesItem
{
    public VariantListItem Variant { get; set; } = new();
    public List<VariantAttributeValueViewItem> AttributeValues { get; set; } = new();
}

public class VariantDetailsWithProductContextItem
{
    public VariantListItem Variant { get; set; } = new();
    public Guid ProductBusinessKey { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductBaseSku { get; set; } = string.Empty;
    public Guid CategoryBusinessKey { get; set; }
    public string CategoryCode { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
}

public class VariantFullDetailsItem
{
    public VariantListItem Variant { get; set; } = new();
    public Guid ProductBusinessKey { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductBaseSku { get; set; } = string.Empty;
    public Guid CategoryBusinessKey { get; set; }
    public string CategoryCode { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public List<VariantAttributeValueWithDefinitionItem> AttributeValues { get; set; } = new();
}

public class VariantCatalogFormItem
{
    public Guid VariantBusinessKey { get; set; }
    public Guid ProductBusinessKey { get; set; }
    public Guid CategoryBusinessKey { get; set; }
    public List<VariantFormFieldItem> Fields { get; set; } = new();
}

public class VariantFormFieldItem
{
    public Guid AttributeRef { get; set; }
    public string AttributeCode { get; set; } = string.Empty;
    public string AttributeName { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public bool IsRequired { get; set; }
    public int DisplayOrder { get; set; }
    public string? Value { get; set; }
    public Guid? OptionRef { get; set; }
}

public class VariantCompletionStatusItem
{
    public Guid VariantBusinessKey { get; set; }
    public int RequiredCount { get; set; }
    public int CompletedCount { get; set; }
    public int MissingCount { get; set; }
    public bool IsComplete { get; set; }
    public List<MissingRequiredVariantAttributeItem> MissingAttributes { get; set; } = new();
}

public class VariantEditorDataItem
{
    public VariantFullDetailsItem VariantDetails { get; set; } = new();
    public VariantCatalogFormItem CatalogForm { get; set; } = new();
    public VariantCompletionStatusItem CompletionStatus { get; set; } = new();
}
