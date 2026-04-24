namespace Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.GetByBusinessKey;

public class GetProductByBusinessKeyQueryResult
{
    public Guid ProductBusinessKey { get; set; }
    public Guid CategoryRef { get; set; }
    public Guid CategorySchemaVersionRef { get; set; }
    public string BaseSku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid DefaultUomRef { get; set; }
    public Guid? TaxCategoryRef { get; set; }
    public bool IsActive { get; set; }
    public List<ProductAttributeValueResultItem> AttributeValues { get; set; } = new();
}

public class ProductAttributeValueResultItem
{
    public Guid AttributeRef { get; set; }
    public string? Value { get; set; }
    public Guid? OptionRef { get; set; }
}
