namespace Insurance.InventoryDashboard.Panel.Models;

public sealed class CategoryNodeModel
{
    public string Id { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? ParentCategoryId { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public List<CategoryNodeModel> Children { get; set; } = new();
}

public sealed class AttributeDefinitionModel
{
    public string Id { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public string Scope { get; set; } = string.Empty;
    public bool IsActive { get; set; }

    public bool IsRequired { get; set; }
    public bool IsVariant { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsInherited { get; set; }
    public string? SourceCategoryId { get; set; }
    public string? SourceCategoryName { get; set; }

    public List<AttributeOptionModel> Options { get; set; } = new();
}

public sealed class CategoryAttributeRuleModel
{
    public string RuleId { get; set; } = string.Empty;
    public string CategoryId { get; set; } = string.Empty;
    public string AttributeId { get; set; } = string.Empty;
    public string AttributeCode { get; set; } = string.Empty;
    public string AttributeName { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public string Scope { get; set; } = string.Empty;
    public bool AttributeIsActive { get; set; }
    public bool RuleIsRequired { get; set; }
    public bool RuleIsVariant { get; set; }
    public int RuleDisplayOrder { get; set; }
    public bool RuleIsOverridden { get; set; }
    public bool RuleIsActive { get; set; }
    public bool IsInherited { get; set; }
    public string SourceCategoryId { get; set; } = string.Empty;
    public string SourceCategoryCode { get; set; } = string.Empty;
    public string SourceCategoryName { get; set; } = string.Empty;
    public List<AttributeOptionModel> Options { get; set; } = new();
}

public sealed class AttributeOptionModel
{
    public string Id { get; set; } = string.Empty;
    public string OptionName { get; set; } = string.Empty;
    public string OptionValue { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}

public sealed class ProductSummaryModel
{
    public string Id { get; set; } = string.Empty;
    public string CategoryId { get; set; } = string.Empty;
    public string BaseSku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string DefaultUomRef { get; set; } = string.Empty;
    public string? TaxCategoryRef { get; set; }
    public bool IsActive { get; set; }

    public string? CategoryName { get; set; }
    public string? Description { get; set; }
}

public sealed class ProductDetailsModel
{
    public string Id { get; set; } = string.Empty;
    public string CategoryId { get; set; } = string.Empty;
    public string BaseSku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string DefaultUomRef { get; set; } = string.Empty;
    public string? TaxCategoryRef { get; set; }
    public bool IsActive { get; set; }

    public string? CategoryName { get; set; }
    public string? Description { get; set; }
    public List<ProductAttributeValueModel> Attributes { get; set; } = new();
}

public sealed class ProductAttributeValueModel
{
    public string AttributeId { get; set; } = string.Empty;
    public string? AttributeCode { get; set; }
    public string? AttributeName { get; set; }
    public string? DataType { get; set; }
    public string? Scope { get; set; }
    public string? Value { get; set; }
    public string? OptionId { get; set; }
    public string? OptionValue { get; set; }
    public bool IsRequired { get; set; }
}

public sealed class ProductVariantSummaryModel
{
    public string Id { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public string BaseUomRef { get; set; } = string.Empty;
    public string? BaseUom { get; set; }
    public string? TrackingPolicy { get; set; }
    public bool IsActive { get; set; }
    public bool InventoryMovementLocked { get; set; }
}

public sealed class ProductVariantSearchResultModel
{
    public int TotalCount { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public List<ProductVariantSummaryModel> Items { get; set; } = new();
}

public sealed class VariantInventoryTransactionModel
{
    public Guid TransactionBusinessKey { get; set; }
    public string TransactionNo { get; set; } = string.Empty;
    public string TransactionType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? ReferenceType { get; set; }
    public Guid? ReferenceBusinessId { get; set; }
    public Guid WarehouseRef { get; set; }
    public Guid SellerRef { get; set; }
    public DateTime OccurredAt { get; set; }
    public DateTime? PostedAt { get; set; }
}

public sealed class ProductVariantDetailsModel
{
    public string Id { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public string BaseUomRef { get; set; } = string.Empty;
    public string? BaseUom { get; set; }
    public string? TrackingPolicy { get; set; }
    public bool IsActive { get; set; }
    public bool InventoryMovementLocked { get; set; }

    public string? ProductName { get; set; }
    public string? ProductBaseSku { get; set; }
    public string? CategoryId { get; set; }
    public string? CategoryName { get; set; }

    public List<VariantAttributeValueModel> Attributes { get; set; } = new();
}

public sealed class VariantUomConversionModel
{
    public string ConversionId { get; set; } = string.Empty;
    public string VariantId { get; set; } = string.Empty;
    public string FromUomRef { get; set; } = string.Empty;
    public string ToUomRef { get; set; } = string.Empty;
    public decimal Factor { get; set; }
    public string RoundingMode { get; set; } = string.Empty;
    public bool IsBasePath { get; set; }
}

public sealed class VariantAttributeValueModel
{
    public string AttributeId { get; set; } = string.Empty;
    public string? AttributeCode { get; set; }
    public string? AttributeName { get; set; }
    public string? DataType { get; set; }
    public string? Scope { get; set; }
    public string? Value { get; set; }
    public string? OptionId { get; set; }
    public string? OptionValue { get; set; }
}

public sealed class UnitOfMeasureLookupModel
{
    public string Id { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public sealed class UpsertCategoryRequest
{
    public string? Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public string? ParentCategoryId { get; set; }
}

public sealed class CreateAttributeDefinitionRequest
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string DataType { get; set; } = "Text";
    public string Scope { get; set; } = "Both";
    public bool IsRequired { get; set; }
    public bool IsVariant { get; set; }
    public int DisplayOrder { get; set; }
}

public sealed class AddAttributeOptionRequest
{
    public string OptionName { get; set; } = string.Empty;
    public string OptionValue { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
}

public sealed class UpdateAttributeDefinitionRequest
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public string Scope { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}

public sealed class UpdateAttributeOptionRequest
{
    public string OptionName { get; set; } = string.Empty;
    public string OptionValue { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
}

public sealed class UpdateCategoryAttributeRuleRequest
{
    public bool IsRequired { get; set; }
    public bool IsVariant { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsOverridden { get; set; }
    public bool IsActive { get; set; }
}

public sealed class AddCategoryAttributeRuleRequest
{
    public string AttributeId { get; set; } = string.Empty;
    public bool IsRequired { get; set; }
    public bool IsVariant { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsOverridden { get; set; }
    public bool IsActive { get; set; } = true;
}

public sealed class UpsertProductRequest
{
    public string? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CategoryId { get; set; } = string.Empty;
    public string BaseSku { get; set; } = string.Empty;
    public string DefaultUomRef { get; set; } = string.Empty;
    public string? TaxCategoryRef { get; set; }
    public bool IsActive { get; set; } = true;
    public List<CatalogAttributeValueInputModel> AttributeValues { get; set; } = new();

    public string? Description { get; set; }
}

public sealed class CatalogAttributeValueInputModel
{
    public string AttributeId { get; set; } = string.Empty;
    public string? Value { get; set; }
    public string? OptionId { get; set; }
}

public sealed class CreateProductResultModel
{
    public string ProductId { get; set; } = string.Empty;
    public string BaseSku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public sealed class SetProductAttributeValueRequest
{
    public string AttributeId { get; set; } = string.Empty;
    public string? Value { get; set; }
    public string? OptionId { get; set; }
}

public sealed class ChangeProductCategoryRequest
{
    public string CategoryId { get; set; } = string.Empty;
}

public sealed class UpsertVariantRequest
{
    public string ProductId { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public string BaseUomRef { get; set; } = string.Empty;
    public string TrackingPolicy { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public List<CatalogAttributeValueInputModel> AttributeValues { get; set; } = new();
}

public sealed class SetVariantAttributeValueRequest
{
    public string AttributeId { get; set; } = string.Empty;
    public string? Value { get; set; }
    public string? OptionId { get; set; }
}

public sealed class ChangeVariantTrackingPolicyRequest
{
    public string TrackingPolicy { get; set; } = string.Empty;
}

public sealed class ChangeVariantBaseUomRequest
{
    public string BaseUomRef { get; set; } = string.Empty;
}

public sealed class UpsertVariantUomConversionRequest
{
    public string FromUomRef { get; set; } = string.Empty;
    public string ToUomRef { get; set; } = string.Empty;
    public decimal Factor { get; set; }
    public string RoundingMode { get; set; } = "None";
    public bool IsBasePath { get; set; }
}
