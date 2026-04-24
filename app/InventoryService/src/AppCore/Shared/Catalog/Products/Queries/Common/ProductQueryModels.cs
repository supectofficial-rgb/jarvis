namespace Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.Common;

public class ProductListItem
{
    public Guid ProductBusinessKey { get; set; }
    public Guid CategoryRef { get; set; }
    public Guid CategorySchemaVersionRef { get; set; }
    public string BaseSku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid DefaultUomRef { get; set; }
    public Guid? TaxCategoryRef { get; set; }
    public bool IsActive { get; set; }
}

public class ProductVariantListItem
{
    public Guid VariantBusinessKey { get; set; }
    public string VariantSku { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public string TrackingPolicy { get; set; } = string.Empty;
    public Guid BaseUomRef { get; set; }
    public bool IsActive { get; set; }
}

public class ProductAttributeValueViewItem
{
    public Guid ProductAttributeValueBusinessKey { get; set; }
    public Guid ProductRef { get; set; }
    public Guid AttributeRef { get; set; }
    public string? Value { get; set; }
    public Guid? OptionRef { get; set; }
}

public class ProductAttributeValueWithDefinitionItem
{
    public Guid ProductAttributeValueBusinessKey { get; set; }
    public Guid ProductRef { get; set; }
    public Guid AttributeRef { get; set; }
    public string AttributeCode { get; set; } = string.Empty;
    public string AttributeName { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public string Scope { get; set; } = string.Empty;
    public string? Value { get; set; }
    public Guid? OptionRef { get; set; }
    public string? OptionValue { get; set; }
}

public class MissingRequiredProductAttributeItem
{
    public Guid AttributeRef { get; set; }
    public string AttributeCode { get; set; } = string.Empty;
    public string AttributeName { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public string Scope { get; set; } = string.Empty;
}

public class ProductSummaryItem
{
    public Guid ProductBusinessKey { get; set; }
    public string BaseSku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int VariantCount { get; set; }
    public int ActiveVariantCount { get; set; }
    public int AttributeValueCount { get; set; }
}

public class ProductDetailsWithAttributesItem
{
    public ProductListItem Product { get; set; } = new();
    public List<ProductAttributeValueViewItem> AttributeValues { get; set; } = new();
}

public class ProductDetailsWithVariantsItem
{
    public ProductListItem Product { get; set; } = new();
    public List<ProductVariantListItem> Variants { get; set; } = new();
}

public class ProductFullDetailsItem
{
    public ProductListItem Product { get; set; } = new();
    public Guid? CategoryBusinessKey { get; set; }
    public string? CategoryCode { get; set; }
    public string? CategoryName { get; set; }
    public List<ProductAttributeValueWithDefinitionItem> ProductAttributes { get; set; } = new();
    public List<ProductVariantFullItem> Variants { get; set; } = new();
}

public class ProductVariantFullItem
{
    public ProductVariantListItem Variant { get; set; } = new();
    public List<VariantAttributeValueInlineItem> Attributes { get; set; } = new();
}

public class VariantAttributeValueInlineItem
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

public class ProductCompletionStatusItem
{
    public Guid ProductBusinessKey { get; set; }
    public int RequiredCount { get; set; }
    public int CompletedCount { get; set; }
    public int MissingCount { get; set; }
    public bool IsComplete { get; set; }
    public List<MissingRequiredProductAttributeItem> MissingAttributes { get; set; } = new();
}

public class ProductCatalogFormItem
{
    public Guid ProductBusinessKey { get; set; }
    public Guid CategoryBusinessKey { get; set; }
    public Guid CategorySchemaVersionRef { get; set; }
    public List<ProductFormFieldItem> Fields { get; set; } = new();
}

public class ProductFormFieldItem
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

public class ProductEditorDataItem
{
    public ProductFullDetailsItem ProductDetails { get; set; } = new();
    public ProductCatalogFormItem CatalogForm { get; set; } = new();
    public ProductCompletionStatusItem CompletionStatus { get; set; } = new();
}
