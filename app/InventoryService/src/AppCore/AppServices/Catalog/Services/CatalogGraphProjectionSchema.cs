namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Services;

using OysterFx.AppCore.Domain.ValueObjects;

public static class CatalogGraphProjectionSchema
{
    public const string CategoryNode = "Category";
    public const string AttributeDefinitionNode = "AttributeDefinition";
    public const string AttributeOptionNode = "AttributeOption";
    public const string ProductNode = "Product";
    public const string ProductVariantNode = "ProductVariant";
    public const string UnitOfMeasureNode = "UnitOfMeasure";
    public const string VariantUomConversionNode = "VariantUomConversion";
    public const string CategoryCatalogSummaryNode = "CategoryCatalogSummary";
    public const string ProductCatalogSummaryNode = "ProductCatalogSummary";
    public const string VariantCatalogSummaryNode = "VariantCatalogSummary";

    public const string ParentOfRelation = "PARENT_OF";
    public const string CategoryHasAttributeRelation = "CATEGORY_HAS_ATTRIBUTE";
    public const string AttributeHasOptionRelation = "ATTRIBUTE_HAS_OPTION";
    public const string ProductInCategoryRelation = "PRODUCT_IN_CATEGORY";
    public const string ProductDefaultUomRelation = "PRODUCT_DEFAULT_UOM";
    public const string ProductHasAttributeValueRelation = "PRODUCT_HAS_ATTRIBUTE_VALUE";
    public const string VariantOfProductRelation = "VARIANT_OF_PRODUCT";
    public const string VariantBaseUomRelation = "VARIANT_BASE_UOM";
    public const string VariantHasAttributeValueRelation = "VARIANT_HAS_ATTRIBUTE_VALUE";
    public const string VariantHasUomConversionRelation = "VARIANT_HAS_UOM_CONVERSION";
    public const string ConversionFromUomRelation = "CONVERSION_FROM_UOM";
    public const string ConversionToUomRelation = "CONVERSION_TO_UOM";
    public const string SummaryOfCategoryRelation = "SUMMARY_OF_CATEGORY";
    public const string SummaryOfProductRelation = "SUMMARY_OF_PRODUCT";
    public const string SummaryOfVariantRelation = "SUMMARY_OF_VARIANT";

    public static string ToNodeKey(BusinessKey key) => key.Value.ToString("D");

    public static string ToNodeKey(Guid key) => key.ToString("D");

    public static string ToNodeKey(Guid? key) => key.HasValue ? key.Value.ToString("D") : string.Empty;

    public static string ToCategoryCatalogSummaryKey(BusinessKey key) => $"category-summary:{ToNodeKey(key)}";

    public static string ToProductCatalogSummaryKey(BusinessKey key) => $"product-summary:{ToNodeKey(key)}";

    public static string ToVariantCatalogSummaryKey(BusinessKey key) => $"variant-summary:{ToNodeKey(key)}";
}
