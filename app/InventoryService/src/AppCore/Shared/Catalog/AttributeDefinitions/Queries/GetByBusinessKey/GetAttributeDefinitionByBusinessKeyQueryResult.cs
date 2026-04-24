namespace Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries.GetByBusinessKey;

public class GetAttributeDefinitionByBusinessKeyQueryResult
{
    public Guid AttributeDefinitionBusinessKey { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public string Scope { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public List<AttributeOptionResultItem> Options { get; set; } = new();
}

public class AttributeOptionResultItem
{
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}
